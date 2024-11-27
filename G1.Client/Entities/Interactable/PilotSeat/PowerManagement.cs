using Godot;
using System;

public partial class PowerManagement : ShipManagement
{
	private PowerRegulators powerRegulators;
	private ThrusterController thrusters;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		var powerViewPort = this.GetNode<SubViewport>("SubViewport");
		var statsPanel = powerViewPort.GetNode<StatsPanel>("StatsPanel");
		this.thrusters = ShipSystems.GetRegistered<ThrusterController>(this);

		var dragPowerIndicator = statsPanel.GetNode<PowerIndicator>("DragIndicator");
		var dragPowerRegulator = this.thrusters.DragPower;
		dragPowerIndicator.MaxValue = dragPowerRegulator.MaxLevel;
		dragPowerIndicator.CurrentValue = dragPowerRegulator.CurrentLevel;
		dragPowerRegulator.PowerLevelChanged += (newLevel) => dragPowerIndicator.CurrentValue = newLevel;

		var maneuverePowerIndicator = statsPanel.GetNode<PowerIndicator>("ManeuvereIndicator");
		var maneuverePowerRegulator = this.thrusters.ManeuverePower;
		maneuverePowerIndicator.MaxValue = maneuverePowerRegulator.MaxLevel;
		maneuverePowerIndicator.CurrentValue = maneuverePowerRegulator.CurrentLevel;
		maneuverePowerRegulator.PowerLevelChanged += (newLevel) => maneuverePowerIndicator.CurrentValue = newLevel;

		this.powerRegulators = new PowerRegulators(
			(dragPowerRegulator, dragPowerIndicator),
			(maneuverePowerRegulator, maneuverePowerIndicator));
		this.Viewport = powerViewPort;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("PilotSeat.NextPowerRegulator"))
		{
			powerRegulators.Next();
		}
		else if (@event.IsActionPressed("PilotSeat.PreviousPowerRegulator"))
		{
			powerRegulators.Previous();
		}
		else if (@event.IsActionPressed("PilotSeat.IncreasePower"))
		{
			powerRegulators.Current.Increase();
		}
		else if (@event.IsActionPressed("PilotSeat.DecreasePower"))
		{
			powerRegulators.Current.Decrease();
		}
	}
}
