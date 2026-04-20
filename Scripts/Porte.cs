using Godot;
using System;

public partial class Porte : StaticBody3D
{
	[Export]
	public int Id;
	[Export]
	public bool unlock;
	public bool open = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
