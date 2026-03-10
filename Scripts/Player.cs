using Godot;
using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;

public partial class Player : CharacterBody3D
{
	[Export]
	public float InitialSpeed = 5.0f;
	[Export]
	public float Acceleration = 0.1f;
	public float SprintMaxSpeed = 20.0f;
	public float Speed;
	public const float JumpVelocity = 4.5f;
	[Export]
	public float rotationAmount = 0.01f;


	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (Input.IsActionPressed("sprint"))
		{
			if (Speed < SprintMaxSpeed)
				Speed += Acceleration * Speed;
			else
				Speed = SprintMaxSpeed;
		}
		else
		{
			Speed = InitialSpeed;
		}
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_right", "move_left", "move_front", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		if (Input.IsActionPressed("turn_left"))
		{
			RotateY(rotationAmount);
		}
		if (Input.IsActionPressed("turn_right"))
		{
			RotateY(-rotationAmount);
		}
		Velocity = velocity;
		MoveAndSlide();
	}
	[Signal]
	public delegate void ThrowingEventHandler(Vector3 position, RigidBody3d body);
	public void OnObjectDetectorThrowing(RigidBody3d body)
	{
		EmitSignal(SignalName.Throwing, body);
	}
	[Signal]
	public delegate void GrabEventHandler(CharacterBody3D placeHolder);
	public void OnGrab(CharacterBody3D placeHolder)
	{
		EmitSignalGrab(placeHolder);
	}
}
