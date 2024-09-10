using Godot;
using System;

public partial class NavigationMap : Node3D
{
	[Export]
	public Exterier PayerShip { get; set; }
	[Export]
	public float CameraMovementSpeed { get; set; }

	private Vector2 mouseMoveInput;

	private Camera3D camera;

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseMotion mouseMove)
		{
			mouseMoveInput = mouseMove.Relative;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.camera = GetNode<Camera3D>("Camera3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.camera.Transform = this.camera.Transform.LookingAt(this.PayerShip.Position);
		mouseMoveInput = Vector2.Zero;
	}

	private void _OnRemoteStateChanged(ShipState remoteState)
	{
		if (PayerShip.Id.Equals(remoteState.Id))
		{
			PayerShip.Position = remoteState.Position;
			PayerShip.Velocity = remoteState.Velocity;
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}
