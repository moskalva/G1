using Godot;
using System;
using G1.Model;
using System.Diagnostics;
using System.Collections.Generic;
using G1.Model.Serializers;
using System.Text.RegularExpressions;

public partial class ServerConnect : Node3D
{
	private static readonly TimeSpan ConnectionWaitTime = TimeSpan.FromSeconds(5);
	private static readonly HashSet<long> ExitAppCodes = new HashSet<long>{
		NotificationExitTree,
		NotificationCrash,
		NotificationExitWorld,
		NotificationWMCloseRequest,
		NotificationWMGoBackRequest,
	};
	private WebSocketPeer peer;

	private WorldEntityState? stateUpdate;

	[Signal]
	public delegate void DataReceivedEventHandler(CharacterState remoteState);

	public string WebSocketURL { get; set; } = $"ws://localhost:9080/ws/{Player.GlobalIdString}/client";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Connecting");
		var peer = new WebSocketPeer();
		Connect(peer);
		this.peer = peer;
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
			Connect(peer);
		}

		if (state != WebSocketPeer.State.Open)
			return;

		if (this.stateUpdate != null)
		{
			var command = new StateChange(stateUpdate.Value);
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
			if (response is StateChange stateChange)
			{
				var remoteState = stateChange.NewState.ToCharacterState();
				EmitSignal(SignalName.DataReceived, remoteState);
			}
			else
			{
				throw new NotSupportedException($"Unknown command received :'{response}'");
			}
		}
	}

	public void _OnPlayerStateChanged(CharacterState newState)
	{
		this.stateUpdate = newState.ToWorldState();
	}

	private bool Connect(WebSocketPeer peer)
	{
		var error = peer.ConnectToUrl(WebSocketURL);
		if (error != Error.Ok)
		{
			GD.Print($"Could not connect '{error}'");
			return false;
		}

		var stowpwatch = Stopwatch.StartNew();
		do
		{
			if (stowpwatch.Elapsed > ConnectionWaitTime)
			{
				GD.Print("Could not open connection");
				return false;
			}
			peer.Poll();
		} while (peer.GetReadyState() != WebSocketPeer.State.Open);

		GD.Print("Connection opened");
		return true;
	}

}

