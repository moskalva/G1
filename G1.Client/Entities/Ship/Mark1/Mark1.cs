using G1.Model;
using Godot;
using System;

public partial class Mark1 : Node,IShipStateProvider
{
	public WorldEntityId Id { get; set; }
	private float pushForce;

	[Signal]
	public delegate void AccelerateEventHandler(float deltaVelocity);
	[Export]
	public int ShipMass { get; set; } = 10_000;

	private PilotSeat pilotSeat;
	private NavigationMap navigationMap;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var navigationMapViewPort = GetNode<SubViewport>("SubViewport");
		navigationMap = navigationMapViewPort.GetNode<NavigationMap>("NavigationMap");
		var interier = GetNode<Interier>("Interier");
		var engine = interier.GetNode<Engine>("Engine");
		engine.Push += _OnPush;
		pilotSeat = interier.GetNode<PilotSeat>("PilotSeat");
		pilotSeat.NavigationMapView = navigationMapViewPort;
		pilotSeat.Init();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (this.pushForce != 0)
		{
			var deletaVelocity = Mathf.Sqrt(this.pushForce / ShipMass) * delta;
			EmitSignal(SignalName.Accelerate, deletaVelocity);
			this.pushForce = 0;
		}
	}

	public ShipState GetShipState() => this.navigationMap.GetPlayerState();

	public void SetRemoteState(ShipState remoteState) => this.navigationMap.SetState(remoteState);

	private void _OnPush(float force)
	{
		this.pushForce = force;
	}
}
