using Godot;


public partial class Controlling : BaseState
{
	[Signal]
	public delegate void LeaveEventHandler();

	[Export]
	public float CameraRotationSensitivity = 0.001f;

	public ControlPlace ControlPlace { get; set; }

	public override PlayerStateProperties InitialState
		=> this.ControlPlace.CharacterPosture == CharacterPosture.Sitting
		 ? PlayerStateProperties.WorkDesk(this.ControlPlace.CharacterPosition)
		 : PlayerStateProperties.ControlPanel(this.ControlPlace.CharacterPosition);

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMove)
		{
			this.PointOfView.RotateCamera(mouseMove.Relative * CameraRotationSensitivity);
		}
		else if (@event.IsActionPressed("Controlling.LeaveControlPlace"))
		{
			EmitSignal(SignalName.Leave);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var collider = this.AimSensor.GetCollider();
		this.ControlPlace.AimingAt(collider as Node);
	}

	public override void OnEnterState()
	{
		ControlPlace.SetProcessInput(true);
		ControlPlace.IsActive = true;
	}

	public override void OnLeaveState()
	{
		ControlPlace.SetProcessInput(false);
		ControlPlace.IsActive = false;
	}
}
