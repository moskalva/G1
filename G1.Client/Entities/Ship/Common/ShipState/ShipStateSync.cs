using Godot;
using System;

public partial class ShipStateSync : Node
{
	[Signal]
	public delegate void UpdateShipStateEventHandler(ShipState state);

	private ShipController stateProvider;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.stateProvider = ShipSystems.GetRegistered<ShipController>(this);

		var timer = GetNode<Timer>("Timer");
		timer.Timeout += RequestStateUpdate;
	}

	private void RequestStateUpdate()
	{
		if (stateProvider is null)
			return;

		var state = this.stateProvider.GetPlayerState();
		EmitSignal(SignalName.UpdateShipState, state);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
