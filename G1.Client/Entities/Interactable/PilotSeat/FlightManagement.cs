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
		else if (@event.IsAction("PilotSeat.RollShipLeft"))
		{
			GD.Print($"RollShipLeft");
		}
		else if (@event.IsAction("PilotSeat.RollShipRight"))
		{
			GD.Print($"RollShipRight");
		}		
	}
}
