using G1.Model;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShipController : Node
{
	private Dictionary<WorldEntityId, ExternalEnity> externalEntities = new();

	private BaseShip ship;
	private EmissionsController emissions;

	public override void _EnterTree()
	{
		ship = ShipSystems.Register(this);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.emissions = ShipSystems.GetRegistered<EmissionsController>(this);
	}

	public ShipState GetPlayerState() =>
		new ShipState
		{
			Id = this.ship.Id,
			Type = WorldEntityType.Ship,
			Position = ship.Exterier.Position,
			Velocity = ship.Exterier.LinearVelocity,
			AngularVelocity = ship.Exterier.AngularVelocity,
			Rotation = ship.Exterier.Rotation,
			ThermalEmission = emissions.ThermalEmission,
			EmEmission = emissions.EmEmission,
			ParticleEmission = emissions.ParticleEmission,
		};

	public ShipState[] GetExternalEntitiesStates() =>
		externalEntities.Values.Select(e => e.State).ToArray();

	public void SetState(ShipState remoteState)
	{
		if (remoteState.Type == WorldEntityType.Ship && this.ship.Id.Equals(remoteState.Id))
		{
			GD.Print("Received remote state own ship");
			SetState(this.ship.Exterier, remoteState);
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
					AddExternalEntity(remoteState.Id, entity);
				}
				entity.SetState(remoteState);
			}
		}
	}

	public void RemoveExternalEntity(WorldEntityId entityId)
	{
		if (externalEntities.TryGetValue(entityId, out var entity))
		{
			GD.Print($"Removing external entity: '{entityId}'");
			this.ship.ExternalWorld.RemoveChild(entity);
			externalEntities.Remove(entityId);
			entity.OnEntityUpdateTimeout -= OnExternalEntityTimeout;
		}
	}

	private void AddExternalEntity(WorldEntityId entityId, ExternalEnity entity)
	{
		GD.Print($"Adding external entity: '{entityId}'");
		this.ship.ExternalWorld.AddChild(entity);
		externalEntities[entityId] = entity;
		entity.OnEntityUpdateTimeout += OnExternalEntityTimeout;
	}

	private void OnExternalEntityTimeout(IdWrap entity) => this.RemoveExternalEntity(entity.Id);

	private void SetState(Exterier entity, ShipState remoteState)
	{
		entity.SetState(remoteState.Position, remoteState.Velocity, remoteState.AngularVelocity, remoteState.Rotation);
	}
}
