using Godot;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

public partial class Player : CharacterBody3D
{
	[Export]
	public float InitialSpeed = 5.0f;
	[Export]
	public float Acceleration = 0.1f;
	[Export]
	public int anxieter = 0;
	public int Anxieter
	{
		get
		{
			return anxieter;
		}
		set
		{
			if (anxieter != value)
			{
				if (value < 0)
				{
					anxieter = 0;
				}
				else if (value > 100)
				{
					anxieter = 100;
				}
				else
					anxieter = value;
				OnAnxietyChange();
			}
		}
	}
	public bool tripping = false;
	public int trippingMeter = 0;
	public float SprintMaxSpeed = 20.0f;
	public float Speed;
	public const float JumpVelocity = 4.5f;
	[Export]
	public float rotationAmount = 0.01f;

	public override void _Ready()
	{
		Anxieter = 0;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (tripping)
		{

			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1 / (anxieter / 50f);
			GetNode<AnimationPlayer>("AnimationPlayer").Play("tripping");

		}
		else
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
				trippingMeter += 1;
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
			if (Input.IsKeyLabelPressed(Key.O))
			{
				Anxieter++;
				GD.Print($"class joueur : {anxieter}");
			}
			if (trippingMeter > 300)
			{
				Random rng = new Random();
				if (rng.Next(1, 101) <= anxieter)
				{
					tripping = true;
					Timer timer = GetNode<Timer>("Timer");
					timer.WaitTime = anxieter / 50f;
					timer.Start();

				}
				trippingMeter = 0;
			}
			Velocity = velocity;
			MoveAndSlide();
		}
	}
	public void OnDrink(int value)
	{
		Anxieter += value;
	}
	[Signal]
	public delegate void ThrowingEventHandler(Vector3 position, RigidBody3d body);
	public void OnObjectDetectorThrowing(RigidBody3d body)
	{
		body.chanceDisap = anxieter;
		EmitSignal(SignalName.Throwing, body);
	}
	[Signal]
	public delegate void GrabEventHandler(CharacterBody3D placeHolder);
	public void OnGrab(CharacterBody3D placeHolder)
	{
		EmitSignalGrab(placeHolder);
	}
	[Signal]
	public delegate void AnxietyChangeEventHandler(int anxiety);
	public void OnAnxietyChange()
	{
		EmitSignal(SignalName.AnxietyChange, anxieter);
	}

	public void OnTrippingTimeOut()
	{
		tripping = false;
	}
	public void OnEndGame()
	{
		GD.Print("FIN");
		QueueFree();
	}
}
