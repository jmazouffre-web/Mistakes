using Godot;
using System;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

public partial class ObjectDetector : RayCast3D
{
	public bool canGrab;
	RigidBody3d memoire;
	CharacterBody3D placeHolder = new CharacterBody3D();
	int holdedId = -1;
	public float placeHolderOffset = 0;
	[Export]
	Material placeHolderMaterial;
	public Vector3 lastGrabObjectPosition = Vector3.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		memoire = new RigidBody3d();
		canGrab = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (GetCollider() is Cafe cafe)
		{
			if (Input.IsActionJustPressed("drink"))
			{
				cafe.QueueFree();
				EmitSignal(SignalName.Drink, -15);
			}
		}
		if (GetCollider() is Porte porte)
		{
			GD.Print($"Key : {holdedId}");
			GD.Print($"Port  {porte.Id}");
			if (Input.IsActionJustPressed("drink"))
			{
				if (porte.Id == holdedId)
				{
					porte.unlock = true;
				}
				else
				{
					EmitSignal(SignalName.Drink, 20);
				}
				if (porte.unlock)
				{
					if (porte.open)
					{
						porte.RotateY(3.14f / 2);
						porte.open = false;
					}
					else
					{
						porte.RotateY(-3.14f / 2);
						porte.open = true;
					}
				}

			}
		}
		if (canGrab)
		{
			if (GetCollider() is RigidBody3d body)
			{
				if (Input.IsActionJustPressed("pick_up"))
				{
					Node3D hand = GetNode<Node3D>("../HoldedItem");
					hand.AddChild(body.ToCharacterBody());
					lastGrabObjectPosition = body.GlobalPosition;
					placeHolderOffset = body.Offset;
					CharacterBody3D characterBody = GetNode<Node3D>("../HoldedItem").GetChild<CharacterBody3D>(0);
					SetPlaceHolder(characterBody);
					body.QueueFree();
					canGrab = false;
					holdedId = body.Id;
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
		RigidBody3d thrownObject = RigidBody3d.FromModel(characterBody3D, holdedId);
		Node3D throwDirection = GetNode<Node3D>("AimPoint");
		thrownObject.Position = position;
		thrownObject.respawnPosition = lastGrabObjectPosition;
		EmitSignal(SignalName.Throwing, thrownObject);
		thrownObject.ApplyCentralImpulse(inpulseValue * thrownObject.Position.DirectionTo(throwDirection.GlobalPosition));
		characterBody3D.QueueFree();
		placeHolder.QueueFree();
		holdedId = 0;

	}
	[Signal]
	public delegate void GrabEventHandler(CharacterBody3D placeHolder);
	[Signal]
	public delegate void DrinkEventHandler(int value);
}

