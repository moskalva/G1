using Godot;
using System;
using G1.Model;

public partial class Interier : Node3D
{
	public WorldEntityId Id { get; set; }
	private float pushForce;

	[Signal]
	public delegate void AccelerateEventHandler(double deltaVelocity);
	[Export]
	public int ShipMass { get; set; } = 10_000;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (this.pushForce != 0)
		{
			var deletaVelocity = Mathf.Sqrt(this.pushForce / ShipMass) * delta;
			EmitSignal(SignalName.Accelerate, deletaVelocity);
			this.pushForce = 0;
		}
	}

	private void _OnPush(float force)
	{
		this.pushForce = force;
	}
}
