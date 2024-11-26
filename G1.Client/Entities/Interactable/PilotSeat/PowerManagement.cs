using Godot;
using System;

public partial class PowerManagement : ShipManagement
{
	private PowerRegulators powerRegulators;
	private DragThruster dragThruster;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		var powerViewPort = this.GetNode<SubViewport>("SubViewport");
		var statsPanel = powerViewPort.GetNode<StatsPanel>("StatsPanel");
		var thrusterPowerIndicator = statsPanel.GetNode<PowerIndicator>("PowerIndicator");
		this.dragThruster = ShipSystems.GetRegistered<DragThruster>(this);
		var enginePowerRegulator = this.dragThruster.Power;
		thrusterPowerIndicator.MaxValue = enginePowerRegulator.MaxLevel;
		thrusterPowerIndicator.Init();
		enginePowerRegulator.PowerLevelChanged += (newLevel) => thrusterPowerIndicator.CurrentValue = newLevel;

		this.powerRegulators = new PowerRegulators((enginePowerRegulator, thrusterPowerIndicator));
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
