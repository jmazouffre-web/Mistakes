using Godot;
using System;

public partial class GameTimer : Timer
{
	public bool started = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Label label = GetChild<Label>(0);
		label.Text = $"Temps restant = {Convert.ToInt32(TimeLeft)}";
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (!started)
		{
			Start();
			started = true;
		}
	}
	public void OnEndGame()
	{
		Stop();
	}

}
