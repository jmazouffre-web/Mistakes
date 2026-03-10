using Godot;
using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

public partial class ObjectDetector : RayCast3D
{
	public bool canGrab;
	RigidBody3d memoire;
	CharacterBody3D placeHolder = new CharacterBody3D();
	public float placeHolderOffset = 0;
	[Export]
	Material placeHolderMaterial;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		memoire = new RigidBody3d();
		canGrab = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (canGrab)
		{
			if (GetCollider() is RigidBody3d body)
			{
				if (Input.IsActionJustPressed("pick_up"))
				{
					Node3D hand = GetNode<Node3D>("../HoldedItem");
					hand.AddChild(body.ToCharacterBody());
					placeHolderOffset = body.Offset;
					CharacterBody3D characterBody = GetNode<Node3D>("../HoldedItem").GetChild<CharacterBody3D>(0);
					SetPlaceHolder(characterBody);
					body.QueueFree();
					canGrab = false;
					EmitSignal(SignalName.Grab, placeHolder);

				}
			}
		}
		else
		{

			if (Input.IsActionJustPressed("throw"))
			{
				Throw(20, GlobalPosition);
			}
			if (DetectPuttableSurface())
			{
				Vector3 place = GetCollisionPoint();
				if (Input.IsActionPressed("precise_place"))
				{
					placeHolder.Position = place + new Vector3(0, placeHolderOffset, 0);
				}
				else
				{
					placeHolder.Position = new Vector3(0, 10000, 0);
				}
				GD.Print(placeHolder.Position);
				if (Input.IsActionJustPressed("place"))
				{
					Throw(0, place + new Vector3(0, placeHolderOffset, 0));
				}
			}
			else
			{
				placeHolder.Position = new Vector3(0, 10000, 0);
			}
		}
	}
	public bool DetectPuttableSurface()
	{
		return (GetCollisionNormal() == Vector3.Up) && (GetCollider() is StaticBody3D);
	}
	public void SetPlaceHolder(CharacterBody3D body)
	{
		placeHolder = (CharacterBody3D)body.Duplicate();
		foreach (Node node in placeHolder.GetChildren())
		{
			if (node is MeshInstance3D mesh)
			{
				mesh.MaterialOverride = placeHolderMaterial;
			}
		}


	}
	[Signal]
	public delegate void ThrowingEventHandler(RigidBody3d body);
	public void Throw(float inpulseValue, Vector3 position)
	{
		canGrab = true;
		CharacterBody3D characterBody3D = GetNode<Node3D>("../HoldedItem").GetChild<CharacterBody3D>(0);
		RigidBody3d thrownObject = RigidBody3d.FromModel(characterBody3D);
		Node3D throwDirection = GetNode<Node3D>("AimPoint");
		thrownObject.Position = position;
		EmitSignal(SignalName.Throwing, thrownObject);
		thrownObject.ApplyCentralImpulse(inpulseValue * thrownObject.Position.DirectionTo(throwDirection.GlobalPosition));
		characterBody3D.QueueFree();
		placeHolder.QueueFree();

	}
	[Signal]
	public delegate void GrabEventHandler(CharacterBody3D placeHolder);

}

