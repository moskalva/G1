using Godot;
using System;

public partial class StatsPanel : Node2D
{
	private Label shipPositionLabel;
	private Label shipSpeedLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.shipPositionLabel = GetNode<Label>("ShipPositionLabel");
		this.shipSpeedLabel = GetNode<Label>("ShipSpeedLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
	}

	public void DisplayShipSpeed(Vector3 velocity, Vector3 angularVelocity)
	{
		var magnitude = velocity.Length();
		this.shipSpeedLabel.Text = $"Speed: {magnitude:C} m/s, Rotation: {angularVelocity.Length()}";
	}

	public void OnUpdateStats(ShipState shipState)
	{
		DisplayShipSpeed(shipState.Velocity, shipState.AngularVelocity);
	}
}
