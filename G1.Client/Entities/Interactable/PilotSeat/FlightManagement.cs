using Godot;
using System;

public partial class FlightManagement : ShipManagement
{
	private ThrusterController thrusters;
    private FisheyeCamera fisheye;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();
		this.GetAccendant<BaseShip>();
		this.thrusters = ShipSystems.GetRegistered<ThrusterController>(this);
		this.fisheye = ShipSystems.GetRegistered<FisheyeCamera>(this);
		this.Viewport = this.fisheye.View;
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
		// not implemented

		else if (@event.IsActionPressed("PilotSeat.Fisheye.Switch", true))
		{
			this.fisheye.SwitchCamera();
		}
		else if (@event.IsAction("PilotSeat.Fisheye.MoveUp", true))
		{
			this.fisheye.MoveCamera(Vector2.Up);
		}
		else if (@event.IsAction("PilotSeat.Fisheye.MoveDown", true))
		{
			this.fisheye.MoveCamera(Vector2.Down);
		}
		else if (@event.IsAction("PilotSeat.Fisheye.MoveRight", true))
		{
			this.fisheye.MoveCamera(Vector2.Right);
		}
		else if (@event.IsAction("PilotSeat.Fisheye.MoveLeft", true))
		{
			this.fisheye.MoveCamera(Vector2.Left);
		}
		else if (@event.IsAction("PilotSeat.Fisheye.ZoomIn", true))
		{
			this.fisheye.Zoom(true);
		}
		else if (@event.IsAction("PilotSeat.Fisheye.ZoomOut", true))
		{
			this.fisheye.Zoom(false);
		}
	}
}
