using Godot;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Linq.Expressions;

public partial class RigidBody3d : RigidBody3D
{
	[Export]
	public int Id;
	public float Offset = 0.3f;
	[Export]
	public float bounce = 0.6f;
	public int chanceDisap = 0;
	public Vector3 newPosition;
	public VisibleOnScreenNotifier3D screen;
	public Vector3 respawnPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetCollisionLayerValue(1, false);
		SetCollisionLayerValue(2, true);
		SetCollisionMaskValue(3, true);
		PhysicsMaterialOverride = new PhysicsMaterial();
		PhysicsMaterialOverride.Bounce = bounce;
		screen = GetNode<VisibleOnScreenNotifier3D>("VisibleOnScreenNotifier3D");
		screen.ScreenExited += OnScreenExited;
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
	public static RigidBody3d FromModel(CharacterBody3D character, int id)
	{
		RigidBody3d rigidBody3d = new RigidBody3d();
		rigidBody3d.Id = id;
		foreach (var child in character.GetChildren())
		{
			character.RemoveChild(child);
			rigidBody3d.AddChild(child);
		}
		return rigidBody3d;
	}
	public void OnScreenExited()
	{
		Random rng = new Random();
		if (rng.Next(0, 101) < chanceDisap)
			Position = respawnPosition;
		GD.Print("je disprais");
	}

}
