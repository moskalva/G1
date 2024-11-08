using Godot;
using System;

[Tool]
public partial class Stencil : Node2D
{
	[Export]
	public float CellSize { get; set; } = 50;

	[Export]
	public Color Color { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		var rect = new Rect2(new Vector2(CellSize / 2, CellSize / 2), new Vector2(CellSize, CellSize));
		this.DrawRect(rect, this.Color, filled: true);
	}
}
