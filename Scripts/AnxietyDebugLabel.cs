using Godot;
using System;

public partial class AnxietyDebugLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = $"Anxiété = {0}";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnAnxietyChange(int anxiety)
	{
		GD.Print(anxiety);
		Text = $"Anxiété = {anxiety}";
	}
}
