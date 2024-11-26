using Godot;
using System;

public partial class FlightManagement : ShipManagement
{
	private DragThruster dragThruster;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		var ship = this.GetAccendant<BaseShip>();
		this.Viewport = ship.ExternalWorld;
		this.dragThruster = ShipSystems.GetRegistered<DragThruster>(this);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("PilotSeat.EngineBurn"))
		{
			this.dragThruster.Burn();
		}
		else if (@event.IsAction("PilotSeat.RotateShipUp"))
		{
			GD.Print($"RotateShipUp");
		}
		else if (@event.IsAction("PilotSeat.RotateShipDown"))
		{
			GD.Print($"RotateShipDown");
		}
		else if (@event.IsAction("PilotSeat.RotateShipLeft"))
		{
			GD.Print($"RotateShipLeft");
		}
		else if (@event.IsAction("PilotSeat.RotateShipRight"))
		{
			GD.Print($"RotateShipRight");
		}
		else if (@event.IsActionPressed("PilotSeat.ToggleNavigationMap"))
		{
			GD.Print($"ToggleNavigationMap");
		}
	}
}
