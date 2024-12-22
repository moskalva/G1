using G1.Model;
using Godot;
using System;

public partial class ExternalEnity : Node
{
	private ShipState state;
	public ShipState State => RefreshState();

	[Signal]
	public delegate void OnEntityUpdateTimeoutEventHandler(IdWrap entity);
	[Export]
	public Exterier TrackedNode { get; set; }
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
		EmitSignal(SignalName.OnEntityUpdateTimeout, new IdWrap { Id = this.state.Id });
	}

	public void SetState(ShipState remoteState)
	{
		this.TrackedNode.SetState(remoteState.Position, remoteState.Velocity, remoteState.AngularVelocity, remoteState.Rotation);
		this.state = remoteState;
	}

	private ShipState RefreshState()
	{
		this.state.Position = this.TrackedNode.Position;
		this.state.Velocity = this.TrackedNode.LinearVelocity;
		this.state.AngularVelocity = this.TrackedNode.AngularVelocity;
		this.state.Rotation = this.TrackedNode.Rotation;
		return this.state;
	}
}
