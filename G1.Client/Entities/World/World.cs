using Godot;
using System;
using G1.Model;

public partial class World : Node
{
	public WorldEntityId Id = new WorldEntityId() { Id = new Guid("2b8786fa-7915-4fc9-9237-cf3dea9810a2") };

	private Interier shipInterier;
	private Exterier shipExterier;

	[Export]
	public NavigationMap NavigationMap { get; set; }
	[Export]
	public ServerConnect ServerConnect { get; set; }

	[Export]
	public Timer SyncTimer { get; set; }

	[Signal]
	public delegate void PlayerStateChangedEventHandler(ShipState state);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var interierPack = GD.Load<PackedScene>("res://Entities/Ship/Mark1/Interier/Interier.tscn");
		shipInterier = interierPack.Instantiate<Interier>();
		shipInterier.Id = this.Id;
		AddChild(shipInterier);

		var exterierPack = GD.Load<PackedScene>("res://Entities/Ship/Mark1/Exterier/Exterier.tscn");
		var playerShipExterier = exterierPack.Instantiate<Exterier>();
		playerShipExterier.Id = this.Id;

		NavigationMap.AddChild(playerShipExterier);
		NavigationMap.PayerShip = playerShipExterier;

		ServerConnect.Init(Id);

		SyncTimer.Timeout += () => EmitSignal(SignalName.PlayerStateChanged, playerShipExterier.ExtractState());

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
			SyncTimer.Start();
			this.SetProcess(true);
		}
	}
}
