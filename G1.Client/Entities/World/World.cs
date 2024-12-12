using Godot;
using System;
using G1.Model;

public partial class World : Node
{
	public readonly WorldEntityId Id = new WorldEntityId() { Id = Guid.NewGuid() };
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

	public void OnRemoteStateChanged(ShipState remoteState)
	{
		GD.Print($"Server state update: '{remoteState}'");
		if (SyncTimer.IsStopped())
		{
			if (!remoteState.Id.Equals(this.Id))
				throw new InvalidOperationException($"Invalid Id returned from server. Expected: '{this.Id}', received: '{remoteState.Id}'");
			GD.Print("Starting sync");
			ship = Loader.LoadShip(remoteState);
			this.AddChild(ship);
			SyncTimer.Timeout += () => EmitSignal(SignalName.PlayerShipStateChanged, ship.Controller.GetPlayerState());
			SyncTimer.Start();
			this.SetProcess(true);
		}
		ship.Controller.SetState(remoteState);
	}

	public void OnRemoteEntityDisconnected(EntityInfo entity) => ship.Controller.RemoveExternalEntity(entity.Id);
}
