using Godot;
using System;
using System.Text;
using G1.Model;

public partial class ServerConnect : Node3D
{
	private WebSocketPeer peer;
	
	private bool hasStateChanged = false;
	private WorldEntityState state;

	[Export]
	public string WebSocketURL { get; set; } = "ws://localhost:9080/ws";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Connecting");
		var peer = new WebSocketPeer();
		var success = Connect(peer);
		if (!success)
		{
			SetProcess(false);
			return;
		}
		this.peer = peer;
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
		}

		if (state != WebSocketPeer.State.Open)
			return;

		if (hasStateChanged)
		{
			GD.Print("State is open");
			var error = peer.SendText("Hello");
			if (error != Error.Ok)
			{
				GD.Print($"Could not initiate communication '{error}'");
				return;
			}

			this.hasStateChanged = false;
		}

		for (int i = 0; i < peer.GetAvailablePacketCount(); i++)
		{
			var response = Encoding.UTF8.GetString(peer.GetPacket());
			GD.Print($"Received '{response}'");
		}
	}
	public void SetClientState(WorldEntityState state)
	{
		this.hasStateChanged = true;
		this.state = state;
	}

	private bool Connect(WebSocketPeer peer)
	{
		var error = peer.ConnectToUrl(WebSocketURL);
		if (error != Error.Ok)
		{
			GD.Print($"Could not connect '{error}'");
			return false;
		}

		GD.Print("Connection opened");
		return true;
	}
}
