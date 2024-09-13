using Godot;
using System;
using G1.Model;

public partial class Exterier : CharacterBody3D
{
	public WorldEntityId Id { get; private set; }

	public override void _EnterTree()
	{
		var ship = this.GetAccendant<Mark1>();
		this.Id = ship.Id;
	}
	
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}
}
