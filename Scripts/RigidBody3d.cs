using Godot;
using System;
using System.Collections;
using System.Linq.Expressions;

public partial class RigidBody3d : RigidBody3D
{
	public float Offset = 0.3f;
	public Vector3 newPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetCollisionLayerValue(1, false);
		SetCollisionLayerValue(2, true);
		SetCollisionMaskValue(3, true);
		PhysicsMaterialOverride = new PhysicsMaterial();
		PhysicsMaterialOverride.Bounce = 0.6f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
	}

	public CharacterBody3D ToCharacterBody()
	{
		CharacterBody3D characterBody3D = new CharacterBody3D();
		foreach (var child in GetChildren())
		{
			RemoveChild(child);
			characterBody3D.AddChild(child);
		}
		characterBody3D.SetCollisionLayerValue(1, false);
		return characterBody3D;
	}
	public CharacterBody3D CopyToCharacterBody()
	{
		CharacterBody3D characterBody3D = new CharacterBody3D();
		foreach (var child in GetChildren())
		{
			var copy = child.Duplicate(1);
			characterBody3D.AddChild(copy);
		}
		characterBody3D.SetCollisionLayerValue(1, false);

		return characterBody3D;
	}
	public static RigidBody3d FromModel(CharacterBody3D character)
	{
		RigidBody3d rigidBody3d = new RigidBody3d();
		foreach (var child in character.GetChildren())
		{
			character.RemoveChild(child);
			rigidBody3d.AddChild(child);
		}
		return rigidBody3d;
	}

}
