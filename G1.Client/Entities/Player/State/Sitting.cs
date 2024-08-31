using Godot;


public partial class Sitting : BaseState
{
	[Signal]
	public delegate void LeaveEventHandler();
    
	[Export]
	public float CameraRotationSensitivity = 0.001f;

    public Transform3D CharacterPosition { get; set; }


    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseMotion mouseMove)
		{
			this.PointOfView.RotateCamera(mouseMove.Relative * CameraRotationSensitivity);
		}
		else
        if (@event.IsActionPressed("LeaveControlPlace"))
        {
            EmitSignal(SignalName.Leave);
        }
    }

	public override PlayerStateProperties InitialState => PlayerStateProperties.WorkDesk(CharacterPosition);

}