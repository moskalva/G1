using Godot;
using System;
using G1.Model;

public partial class World : Node
{
	public WorldEntityId Id = new WorldEntityId() { Id = new Guid("2b8786fa-7915-4fc9-9237-cf3dea9810a2") };
	private Mark1 ship;

	[Export]
	public Timer SyncTimer { get; set; }

	[Signal]
	public delegate void PlayerShipStateChangedEventHandler(ShipState state);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var serverConnect = GetNode<ServerConnect>("ServerConnect");
		serverConnect.Init(Id);

		// wait for initial data from server 
		SetProcess(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _OnRemoteStateChanged(ShipState remoteState)
	{
		GD.Print($"Server state update: '{remoteState}'");
		if (SyncTimer.IsStopped())
		{
			GD.Print("Starting sync");
			ship = LoadShip();
			SyncTimer.Timeout += () => EmitSignal(SignalName.PlayerShipStateChanged, ship.Controller.GetPlayerState());
			SyncTimer.Start();
			this.SetProcess(true);
		}
		ship.Controller.SetState(remoteState);
	}

	private Mark1 LoadShip()
	{
		var shipScene = GD.Load<PackedScene>("res://Entities/Ship/Mark1/Mark1.tscn");
		var ship = shipScene.Instantiate<Mark1>();
		ship.Id = this.Id;
		this.AddChild(ship);
		return ship;
	}
}
