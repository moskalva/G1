using Godot;
using System;
using G1;

public partial class ThrusterController : Node
{
	[Signal]
	public delegate void PushEventHandler(Vector3 direction, float force);
	[Signal]
	public delegate void TorqueEventHandler(Vector3 direction, float force);

	[Export]
	public float EngineDragPower { get; set; } = WorldParameters.Thrusters.L1.Drag.Power;
	[Export]
	public uint MaxDragPowerLevel { get; set; } = WorldParameters.Thrusters.L1.Drag.Levels;
	[Export]
	public float EngineManeuverePower { get; set; } = WorldParameters.Thrusters.L1.Maneuvere.Power;
	[Export]
	public uint MaxManeuverePowerLevel { get; set; } = WorldParameters.Thrusters.L1.Maneuvere.Levels;

	public PowerRegulator DragPower { get; private set; }
	public PowerRegulator ManeuverePower { get; private set; }

	public override void _EnterTree()
	{
		ShipSystems.Register(this);
		this.DragPower = new PowerRegulator(MaxDragPowerLevel);
		this.ManeuverePower = new PowerRegulator(MaxManeuverePowerLevel);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void BurnDragThruster()
	{
		if (this.DragPower.CurrentLevel > 0)
		{
			var force = EngineDragPower / MaxDragPowerLevel * this.DragPower.CurrentLevel;
			this.EmitSignal(SignalName.Push, Vector3.Forward, force);
		}
	}

	public void BurnManeuvereThrusters(Vector3 direction)
	{
		if (this.ManeuverePower.CurrentLevel > 0)
		{
			var force = EngineManeuverePower / MaxManeuverePowerLevel * this.ManeuverePower.CurrentLevel;
			this.EmitSignal(SignalName.Push, direction, force);
		}
	}

	public void Rotate(Vector3 rotation)
	{
		if (this.ManeuverePower.CurrentLevel > 0)
		{
			var force = EngineManeuverePower / MaxManeuverePowerLevel * this.ManeuverePower.CurrentLevel;
			this.EmitSignal(SignalName.Torque, rotation, force);
		}
	}
}
