using Godot;
using System;

public partial class NavigationMap : Node
{
	[Export]
	public float CameraMovementSpeed { get; set; }

	private Vector2 mouseMoveInput;

	private Camera3D camera;
	private Exterier payerShip;

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
		this.payerShip = GetNode<Exterier>("Exterier");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.camera.Transform = this.camera.Transform.LookingAt(this.payerShip.Position);
		mouseMoveInput = Vector2.Zero;
	}
}
