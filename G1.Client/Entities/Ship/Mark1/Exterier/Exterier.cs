using Godot;
using System;
using G1.Model;

public partial class Exterier : CharacterBody3D
{
	public override void _EnterTree()
	{
	}

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void OnAccelerate(Vector3 direction, float magnitude)
	{
		if (!direction.IsNormalized())
			throw new InvalidOperationException("Acceleration direction should be normalized");
			
		GD.Print($"Accelerating '{magnitude}'. Current Velocity : '{this.Velocity}'");
		var velocity = this.Velocity;
		var deltaVelocity = this.Transform.Basis * direction * magnitude;
		velocity.X += deltaVelocity.X;
		velocity.Y += deltaVelocity.Y;
		velocity.Z += deltaVelocity.Z;
		this.Velocity = velocity;
	}
}
