using Godot;

public partial class Walking : BaseState
{
	[Signal]
	public delegate void ControlModeRequestedEventHandler(ControlPlace state);

	[Export]
	public Vector2 Speed { get; set; } = new Vector2(2f, 4f);


	[Export]
	public float CameraRotationSensitivity = 0.001f;

	[Export]
	public float CharacterRotationSpeed = 3f;

	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMove)
		{
			this.PointOfView.RotateCamera(mouseMove.Relative * CameraRotationSensitivity);
		}
		else if (@event.IsActionPressed("Walking.SwitchCameraShiftSide"))
		{
			this.Player.SwitchCameraShiftSide();
		}
		else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
					this.PointOfView.IncreaseCameraDistance();
					break;
				case MouseButton.WheelDown:
					this.PointOfView.DecreaseCameraDistance();
					break;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var hasCharacterMoved = MoveCharacterViaControls();
		if (hasCharacterMoved)
			RotateCharacterAsCamera(delta);

		Interact();
	}

	public override PlayerStateProperties InitialState { get; } = PlayerStateProperties.Free();

	public override void OnEnterState() { }

	public override void OnLeaveState() { }

	private bool MoveCharacterViaControls()
	{
		var hasCharacterMoved = false;

		var inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down") * Speed;
		if (inputDir != Vector2.Zero)
		{
			Character.Velocity = Character.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
			hasCharacterMoved = true;
		}
		else
		{
			Character.Velocity = Vector3.Zero;
		}
		
		Character.MoveAndSlide();
		return hasCharacterMoved;
	}

	private void RotateCharacterAsCamera(double delta)
	{
		var rotation = Character.Transform.LookTowards(this.PointOfView.CameraDirection).Basis.GetEuler();
		var angle = (float)Mathf.RotateToward(0, rotation.Y, CharacterRotationSpeed * delta);
		Character.Rotate(Vector3.Up, angle);
		PointOfView.RotateCamera(new Vector2(angle, 0));
	}

	private void Interact()
	{
		if (AimSensor.GetCollider().TryGetInteractable(out var interactable))
		{
			interactable.Highlite();
			if (Input.IsActionJustPressed("Walking.Interact"))
			{
				interactable.Interact();
				if (interactable is ControlPlace controlPlace)
				{
					EmitSignal(SignalName.ControlModeRequested, controlPlace);
				}
			}
		}
	}
}
