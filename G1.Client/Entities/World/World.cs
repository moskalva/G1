using Godot;
using System;
using G1.Model;

public partial class World : Node
{
	public readonly WorldEntityId Id = new WorldEntityId() { Id = Guid.NewGuid() };
	private Mark1 ship;
	private ShipState previousShipState;

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
			SyncTimer.Timeout += PlayerStateChange;
			SyncTimer.Start();
			this.SetProcess(true);
		}
		ship.Controller.SetState(remoteState);
	}

	private void PlayerStateChange()
	{
		var currentState = ship.Controller.GetPlayerState();
		if (!currentState.Equals(previousShipState))
		{
			EmitSignal(SignalName.PlayerShipStateChanged, currentState);
			previousShipState = currentState;
		}
	}

	public void OnRemoteEntityDisconnected(EntityInfo entity) => ship.Controller.RemoveExternalEntity(entity.Id);
}
