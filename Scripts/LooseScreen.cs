using Godot;
using System;

public partial class LooseScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnTimeout()
	{
		Show();
	}
}
