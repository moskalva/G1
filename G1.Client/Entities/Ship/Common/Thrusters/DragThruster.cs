using Godot;
using System;

public partial class DragThruster : Node
{
	[Signal]
	public delegate void PushEventHandler(float force);

	[Export]
	public float EnginePower { get; set; } = 100_000;
	[Export]
	public uint MaxPowerLevel { get; set; } = 5;

	public PowerRegulator Power { get; private set; }

	private BaseShip ship;
	public override void _EnterTree()
	{
		this.ship = ShipSystems.Register(this);
		this.Power = new PowerRegulator(MaxPowerLevel);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Burn()
	{
		var force = EnginePower / MaxPowerLevel * this.Power.CurrentLevel;
		this.EmitSignal(SignalName.Push, force);
	}

	public static DragThruster GetDragThruster(Node node) => ShipSystems.GetRegistered<DragThruster>(node);
}
