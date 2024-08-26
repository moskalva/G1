using Godot;

public abstract partial class BaseState : Node
{
	[Signal]
	public delegate void PlayerStateChangedEventHandler(BaseState state);

	[Export]
	public Character Character { get; set; }

	[Export]
	public RayCast3D RayCast { get; set; }

	[Export]
	public Camera3D Camera { get; set; }

	[Export]
	public float CameraSpeed = 0.1f;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}