using Godot;
using System;

public partial class FlightManagement : ShipManagement
{
	private ThrusterController thrusters;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		var ship = this.GetAccendant<BaseShip>();
		this.Viewport = ship.ExternalWorld;
		this.thrusters = ShipSystems.GetRegistered<ThrusterController>(this);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("PilotSeat.DragBurn"))
		{
			this.thrusters.BurnDragThruster();
		}
		else if (@event.IsAction("PilotSeat.MoveShipForward"))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Forward);
		}
		else if (@event.IsAction("PilotSeat.MoveShipBack"))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Back);
		}
		else if (@event.IsAction("PilotSeat.MoveShipLeft"))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Left);
		}
		else if (@event.IsAction("PilotSeat.MoveShipRight"))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Right);
		}
		else if (@event.IsAction("PilotSeat.MoveShipUp"))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Up);
		}
		else if (@event.IsAction("PilotSeat.MoveShipDown"))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Down);
		}
		// not implemented
		else if (@event.IsAction("PilotSeat.RotateShipUp"))
		{
			this.thrusters.Rotate(Vector3.Right);
		}
		else if (@event.IsAction("PilotSeat.RotateShipDown"))
		{
			this.thrusters.Rotate(Vector3.Left);
		}
		else if (@event.IsAction("PilotSeat.RotateShipLeft"))
		{
			this.thrusters.Rotate(Vector3.Up);
		}
		else if (@event.IsAction("PilotSeat.RotateShipRight"))
		{
			this.thrusters.Rotate(Vector3.Down);
		}
		else if (@event.IsAction("PilotSeat.RollShipLeft"))
		{
			this.thrusters.Rotate(Vector3.Back);
		}
		else if (@event.IsAction("PilotSeat.RollShipRight"))
		{
			this.thrusters.Rotate(Vector3.Forward);
		}
	}
}
