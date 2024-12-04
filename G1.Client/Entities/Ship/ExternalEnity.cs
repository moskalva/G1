using Godot;
using System;

public partial class ExternalEnity : Node
{
	public Timer Timer { get; private set; }
	[Export]
	public Exterier TrackedNode { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
