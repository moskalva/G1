using G1.Model;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TargetDirectionController : Node3D
{
	private ShipController shipController;
	private Dictionary<WorldEntityId, DirectionMarker> markers = new();

	public override void _Ready()
	{
		this.shipController = ShipSystems.GetRegistered<ShipController>(this);
	}

	public void OnRefresh()
	{
		var ourPossition = this.shipController.GetPlayerState().Position;
		var entities = shipController.GetExternalEntitiesStates().ToDictionary(s => s.Id);
		foreach (var id in markers.Keys)
		{
			if (!entities.ContainsKey(id))
			{
				GD.Print($"Removing marker for '{id}'");
				this.RemoveChild(markers[id]);
				markers.Remove(id);
			}
		}

		foreach (var (id, entity) in entities)
		{
			if (!markers.TryGetValue(id, out var marker))
			{
				GD.Print($"Creating new marker for '{id}'");
				marker = Loader.LoadMarkerScene();
				markers[id] = marker;
				this.AddChild(marker);
			}
			marker.Update(ourPossition.DirectionTo(entity.Position), entity.Position.DistanceTo(ourPossition));
		}
	}
}
