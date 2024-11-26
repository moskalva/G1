using G1.Model;
using Godot;
using System;

public partial class ShipController : Node
{
	public WorldEntityId Id { get; set; }

	[Export]
	public CharacterBody3D Ship { get; set; }

	public override void _EnterTree()
	{
		ShipSystems.Register(this);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public ShipState GetPlayerState() =>
		new ShipState
		{
			Id = Id,
			Type = WorldEntityType.Ship,
			Position = Ship.Position,
			Velocity = Ship.Velocity,
		};



	public void SetState(ShipState remoteState)
	{
		if (this.Id.Equals(remoteState.Id))
		{
			this.Ship.Position = remoteState.Position;
			this.Ship.Velocity = remoteState.Velocity;
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}
