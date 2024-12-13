using Godot;
using System;
using G1.Model;
using System.Diagnostics;
using System.Collections.Generic;
using G1.Model.Serializers;
using System.Text.RegularExpressions;

public partial class ServerConnect : Node
{
	public static string WebSocketURLFormat { get; set; } = "ws://localhost:9080/ws/{0}/client";
	private static readonly TimeSpan ConnectionWaitTime = TimeSpan.FromSeconds(5);
	private static readonly HashSet<long> ExitAppCodes = new HashSet<long>{
		NotificationExitTree,
		NotificationCrash,
		NotificationWMCloseRequest,
		NotificationWMGoBackRequest,
	};
	private WebSocketPeer peer;

	private WorldEntityState? stateUpdate;

	[Signal]
	public delegate void OnRemoteStateChangedEventHandler(ShipState remoteState);
	[Signal]
	public delegate void OnRemoteEntityDisconnectedEventHandler(EntityInfo entity);
	private string userId;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.peer = new WebSocketPeer();
		this.SetProcess(false);
	}

	public void Init(WorldEntityId userId)
	{
		this.userId = userId.ToString();
		Connect(peer, this.userId);
		this.SetProcess(true);
	}

	public override void _Notification(int what)
	{
		if (ExitAppCodes.Contains(what))
		{
			peer.Close(1000, "Exit application");
			peer.Dispose();
		}

		base._Notification(what);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		peer.Poll();
		var state = peer.GetReadyState();

		if (state == WebSocketPeer.State.Closed)
		{
			var code = peer.GetCloseCode();
			var reason = peer.GetCloseReason();
			GD.Print($"Connection closed '{code}', '{reason}'");
			Connect(peer, userId);
		}

		if (state != WebSocketPeer.State.Open)
			return;

		if (this.stateUpdate != null)
		{
			var command = new StateChange(stateUpdate.Value);
			GD.Print($"Sending command '{command}'");
			var error = peer.Send(SerializerHelpers.Serialize(command));
			if (error != Error.Ok)
			{
				GD.Print($"Could not initiate communication '{error}'");
				return;
			}

			this.stateUpdate = null;
		}

		for (int i = 0; i < peer.GetAvailablePacketCount(); i++)
		{
			var response = SerializerHelpers.Deserialize<RemoteCommand>(peer.GetPacket());
			if (response is HeartBeat heartBeat)
			{
				GD.Print("Received HeartBeat");
			}
			else if (response is StateChange stateChange)
			{
				GD.Print($"Received state change '{stateChange.NewState.Id}'");
				var remoteState = stateChange.NewState.ToShipState();
				EmitSignal(SignalName.OnRemoteStateChanged, remoteState);
			}
			else if (response is NeighborLeft left)
			{
				GD.Print($"Received NeighborLeft '{left.Id}'");
				var entity = new EntityInfo { Id = left.Id };
				EmitSignal(SignalName.OnRemoteEntityDisconnected, entity);
			}
			else
			{
				throw new NotSupportedException($"Unknown command received :'{response}'");
			}
		}
	}

	private static bool Connect(WebSocketPeer peer, string userId)
	{
		GD.Print("Connecting");
		if (string.IsNullOrEmpty(userId))
		{
			GD.PrintErr("UserId is not set");
			return false;
		}

		var url = String.Format(WebSocketURLFormat, userId);
		var error = peer.ConnectToUrl(url);
		if (error != Error.Ok)
		{
			GD.PrintErr($"Could not connect '{error}'");
			return false;
		}

		var stowpwatch = Stopwatch.StartNew();
		do
		{
			if (stowpwatch.Elapsed > ConnectionWaitTime)
			{
				GD.PrintErr("Could not open connection");
				return false;
			}
			peer.Poll();
		} while (peer.GetReadyState() != WebSocketPeer.State.Open);

		GD.Print("Connection opened");
		return true;
	}

	public void OnPlayerShipStateChanged(ShipState newState)
	{
		this.stateUpdate = newState.ToWorldState();
	}
}
