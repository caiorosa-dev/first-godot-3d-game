using Godot;
using System;

public partial class Flutuar : StaticBody3D
{
	double test = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		test += delta;

		GlobalPosition += new Vector3(0, (float) Math.Sin(test) / 120, 0);
    }
}
