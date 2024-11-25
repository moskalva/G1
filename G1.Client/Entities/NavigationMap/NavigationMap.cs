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

    public ShipState GetPlayerState() => this.payerShip.ExtractState();

    public void SetState(ShipState remoteState)
    {
		if (this.payerShip.Id.Equals(remoteState.Id))
		{
			this.payerShip.Position = remoteState.Position;
			this.payerShip.Velocity = remoteState.Velocity;
		}
		else
		{
			throw new NotImplementedException();
		}
    }

	public void _OnAccelerate(float deltaVelocity)
	{
		GD.Print($"Accelerating '{deltaVelocity}'. Current Velocity : '{this.payerShip.Velocity}'");
		var velocity = this.payerShip.Velocity;
		var direction = this.payerShip.Transform.Basis * new Vector3(0, 0, deltaVelocity);
		velocity.X += direction.X;
		velocity.Y += direction.Y;
		velocity.Z += direction.Z;
		this.payerShip.Velocity = velocity;
	}
}
