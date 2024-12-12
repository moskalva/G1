using G1.Model;
using Godot;
using System;

public partial class ExternalEnity : Node
{
	[Signal]
	public delegate void OnEntityUpdateTimeoutEventHandler(EntityInfo entity);
	[Export]
	public Exterier TrackedNode { get; set; }
	public WorldEntityId Id { get; set; }
	public Timer Timer { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnTimeout()
	{
		this.Timer.Stop();
		EmitSignal(SignalName.OnEntityUpdateTimeout, new EntityInfo { Id = this.Id });
	}
}
