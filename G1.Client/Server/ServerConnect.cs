using Godot;
using System;
using G1.Model;
using System.Diagnostics;
using System.Collections.Generic;
using G1.Model.Serializers;
using System.Text.RegularExpressions;
using ProtoBuf.WellKnownTypes;

public partial class ServerConnect : Node
{
	private static readonly HashSet<long> ExitAppCodes = new HashSet<long>{
		NotificationExitTree,
		NotificationCrash,
		NotificationWMCloseRequest,
		NotificationWMGoBackRequest,
	};

	[Signal]
	public delegate void OnRemoteStateChangedEventHandler(ShipState remoteState);
	[Signal]
	public delegate void OnRemoteEntityDisconnectedEventHandler(EntityInfo entity);

	private WebSocketPeer peer;

	private ClientCommand? stateUpdate;
	private Stopwatch serverUpdateTimer = new Stopwatch();
	private WorldEntityId userId;
	private byte[] heartBeatMessage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.peer = new WebSocketPeer();
		this.SetProcess(false);
	}

	public void Init(WorldEntityId userId)
	{
		this.userId = userId;
		this.heartBeatMessage = SerializerHelpers.Serialize(new ClientHeartBeat() { Id = this.userId });
		Connect();
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
			serverUpdateTimer.Stop();
			var code = peer.GetCloseCode();
			var reason = peer.GetCloseReason();
			GD.Print($"Connection closed '{code}', '{reason}'");
			Connect();
		}

		if (state != WebSocketPeer.State.Open)
			return;

		if (ShouldSendCommandToServer(out var message))
		{
			var error = peer.Send(message);
			this.serverUpdateTimer.Restart();
			if (error != Error.Ok)
			{
				GD.Print($"Could not send command '{error}'");
				return;
			}
		}

		for (int i = 0; i < peer.GetAvailablePacketCount(); i++)
		{
			var response = SerializerHelpers.Deserialize<ServerCommand>(peer.GetPacket());
			HandleServerCommand(response);
		}
	}

	public void OnPlayerShipStateChanged(ShipState newState)
	{
		this.stateUpdate = newState.ToWorldState();
	}

	private bool ShouldSendCommandToServer(out byte[] message)
	{
		if (this.stateUpdate != null)
		{
			var command = this.stateUpdate;
			GD.Print($"Sending state update '{command}'");
			message = SerializerHelpers.Serialize(command);
			this.stateUpdate = null;
			return true;
		}
		else if (this.serverUpdateTimer.Elapsed > Settings.HeartBeatInterval)
		{
			GD.Print($"Sending heart beat");
			message = heartBeatMessage;
			return true;
		}

		message = null;
		return false;
	}

	private void HandleServerCommand(ServerCommand response)
	{
		if (response is ServerHeartBeat heartBeat)
		{
			GD.Print("Received HeartBeat");
		}
		else if (response is ServerStateChange stateChange)
		{
			GD.Print($"Received state change '{stateChange.Id}'");
			var remoteState = stateChange.ToShipState();
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

	private bool Connect()
	{
		GD.Print("Connecting");
		if (userId.Id == default)
		{
			GD.PrintErr("UserId is not set");
			return false;
		}

		var url = String.Format(Settings.WebSocketURLFormat, userId);
		var error = peer.ConnectToUrl(url);
		if (error != Error.Ok)
		{
			GD.PrintErr($"Could not connect '{error}'");
			return false;
		}

		var stopwatch = Stopwatch.StartNew();
		do
		{
			if (stopwatch.Elapsed > Settings.ConnectionWaitTime)
			{
				GD.PrintErr("Could not open connection");
				return false;
			}
			peer.Poll();
		} while (peer.GetReadyState() != WebSocketPeer.State.Open);

		GD.Print("Connection opened");
		serverUpdateTimer.Start();
		return true;
	}
}
