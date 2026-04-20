using Godot;
using System;

public partial class Main : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player player = GetNode<Player>("Player");
		EndScreen endScreen = GetNode<EndScreen>("EndScreen");
		LooseScreen looseScreen = GetNode<LooseScreen>("LooseScreen");
		Area3D area3D = GetNode<Area3D>("Area3D");
		GameTimer timer = GetNode<GameTimer>("GameTimer");
		player.AnxietyChange += GetNode<AnxietyDebugLabel>("UserInterface/AnxietyDebugLabel").OnAnxietyChange;
		Connect(SignalName.EndGame, new Callable(player, nameof(player.OnEndGame)));
		Connect(SignalName.EndGame, new Callable(endScreen, nameof(endScreen.OnEndGame)));
		Connect(SignalName.EndGame, new Callable(timer, nameof(timer.OnEndGame)));
		Connect(SignalName.Timeout, new Callable(player, nameof(player.OnEndGame)));
		Connect(SignalName.Timeout, new Callable(looseScreen, nameof(looseScreen.OnTimeout)));

		area3D.BodyEntered += OnEnd;

		timer.Timeout += OnTimeout;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("reset"))
		{
			// This restarts the current scene.
			GetTree().ReloadCurrentScene();
		}
	}
	public void OnPlayerThrowing(RigidBody3d body)
	{
		AddChild(body);
	}
	public void OnGrab(CharacterBody3D placeHolder)
	{
		AddChild(placeHolder);
	}
	[Signal]
	public delegate void EndGameEventHandler();
	public void OnEnd(Node3D body)
	{
		EmitSignal(SignalName.EndGame);
	}
	[Signal]
	public delegate void TimeoutEventHandler();
	public void OnTimeout()
	{
		EmitSignal(SignalName.Timeout);
	}
}
