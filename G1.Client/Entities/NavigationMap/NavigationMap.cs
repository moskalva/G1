using Godot;
using System;

public partial class NavigationMap : Node3D
{
	[Export]
	public Exterier PayerShip { get; set; }
	[Export]
	public float CameraMovementSpeed { get; set; }

	private Vector2 mouseMoveInput;

	public Camera3D camera;

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.camera.RotateObjectLocal(Vector3.Down, mouseMoveInput.X * CameraMovementSpeed);
		this.camera.RotateObjectLocal(Vector3.Right, mouseMoveInput.Y * CameraMovementSpeed);
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
