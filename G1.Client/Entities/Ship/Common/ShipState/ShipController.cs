using G1.Model;
using Godot;
using System;

public partial class ShipController : Node
{
	[Export]
	public Exterier Ship { get; set; }

	private WorldEntityId id;

	public override void _EnterTree()
	{
		var ship = ShipSystems.Register(this);
		this.id = ship.Id;
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
			Id = this.id,
			Type = WorldEntityType.Ship,
			Position = Ship.Position,
			Velocity = Ship.LinearVelocity,
			AngularVelocity = Ship.AngularVelocity,
		};



	public void SetState(ShipState remoteState)
	{
		if (this.id.Equals(remoteState.Id))
		{
			this.Ship.Position = remoteState.Position;
			this.Ship.LinearVelocity = remoteState.Velocity;
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}
