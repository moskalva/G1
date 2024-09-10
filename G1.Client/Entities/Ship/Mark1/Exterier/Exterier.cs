using Godot;
using System;
using G1.Model;

public partial class Exterier : CharacterBody3D
{
	public WorldEntityId Id { get; set; }

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void Accelerate(float deltaVelocity)
	{
		GD.Print($"Accelerating '{deltaVelocity}'. Current Velocity : '{this.Velocity}'");
		var velocity = this.Velocity;
		var direction = this.Transform.Basis * new Vector3(0, 0, deltaVelocity);
		velocity.X += direction.X;
		velocity.Y += direction.Y;
		velocity.Z += direction.Z;
		this.Velocity = velocity;
	}
}
