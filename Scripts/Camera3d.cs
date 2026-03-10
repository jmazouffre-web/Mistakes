using Godot;
using System;

public partial class Camera3d : Camera3D
{
	[Export]
	public float rotationAmount = 0.5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("look_up") && (GetRotationDegrees().X < 90))
		{
			RotateX(rotationAmount);
		}
		if (Input.IsActionPressed("look_down") && (GetRotationDegrees().X > -90))
		{
			RotateX(-rotationAmount);
		}
	}
}
