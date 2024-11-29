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
		this.Viewport = ship.FishEyeView;
		this.thrusters = ShipSystems.GetRegistered<ThrusterController>(this);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("PilotSeat.DragBurn", true))
		{
			this.thrusters.BurnDragThruster();
		}
		else if (@event.IsAction("PilotSeat.MoveShipForward", true))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Forward);
		}
		else if (@event.IsAction("PilotSeat.MoveShipBack", true))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Back);
		}
		else if (@event.IsAction("PilotSeat.MoveShipLeft", true))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Left);
		}
		else if (@event.IsAction("PilotSeat.MoveShipRight", true))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Right);
		}
		else if (@event.IsAction("PilotSeat.MoveShipUp", true))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Up);
		}
		else if (@event.IsAction("PilotSeat.MoveShipDown", true))
		{
			this.thrusters.BurnManeuvereThrusters(Vector3.Down);
		}
		else if (@event.IsAction("PilotSeat.RotateShipUp", true))
		{
			this.thrusters.Rotate(Vector3.Right);
		}
		else if (@event.IsAction("PilotSeat.RotateShipDown", true))
		{
			this.thrusters.Rotate(Vector3.Left);
		}
		else if (@event.IsAction("PilotSeat.RotateShipLeft", true))
		{
			this.thrusters.Rotate(Vector3.Up);
		}
		else if (@event.IsAction("PilotSeat.RotateShipRight", true))
		{
			this.thrusters.Rotate(Vector3.Down);
		}
		else if (@event.IsAction("PilotSeat.RollShipLeft", true))
		{
			this.thrusters.Rotate(Vector3.Back);
		}
		else if (@event.IsAction("PilotSeat.RollShipRight", true))
		{
			this.thrusters.Rotate(Vector3.Forward);
		}
	}
}
