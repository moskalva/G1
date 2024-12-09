using G1.Model;
using Godot;
using System;
using System.Collections.Generic;

public partial class ShipController : Node
{
	[Export]
	public Exterier Ship { get; set; }

	private BaseShip ship;

	public override void _EnterTree()
	{
		ship = ShipSystems.Register(this);
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
			Id = this.ship.Id,
			Type = WorldEntityType.Ship,
			Position = Ship.Position,
			Velocity = Ship.LinearVelocity,
			AngularVelocity = Ship.AngularVelocity,
			Rotation = Ship.Rotation,
		};

	private Dictionary<WorldEntityId, ExternalEnity> externalEntities = new();

	public void SetState(ShipState remoteState)
	{
		if (remoteState.Type == WorldEntityType.Ship && this.ship.Id.Equals(remoteState.Id))
		{
			GD.Print("Received remote state own ship");
			SetState(this.Ship, remoteState);
		}
		else
		{
			GD.Print($"Received remote state external entity '{remoteState.Id}'");
			if (remoteState.Type == WorldEntityType.Ship)
			{
				if (!externalEntities.TryGetValue(remoteState.Id, out var entity))
				{
					entity = Loader.LoadExternalEntity();
					entity.TrackedNode = Loader.LoadExterior(remoteState.Type);
					entity.AddChild(entity.TrackedNode);
					AddExternalEntity(remoteState, entity);
				}
				SetState(entity.TrackedNode, remoteState);
			}
		}
	}

	private void AddExternalEntity(ShipState remoteState, ExternalEnity entity)
	{
		this.ship.ExternalWorld.AddChild(entity);
		externalEntities.Add(remoteState.Id, entity);
	}

	private void SetState(Exterier exterier, ShipState remoteState)
	{
		exterier.Position = remoteState.Position;
		exterier.LinearVelocity = remoteState.Velocity;
		exterier.AngularVelocity = remoteState.AngularVelocity;
		exterier.Rotation = remoteState.Rotation;
	}
}
