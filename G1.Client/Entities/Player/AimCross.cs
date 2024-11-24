using Godot;
using System;

public partial class AimCross : CenterContainer
{
	[Export]
	public float Radius { get; set; } = 5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, Radius, Colors.White, filled: true);
	}
}
