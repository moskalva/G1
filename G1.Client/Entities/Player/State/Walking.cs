using Godot;

public partial class Walking : BaseState
{
	[Signal]
	public delegate void ControlModeRequestedEventHandler(ControlPlace state);

	[Export]
	public float Speed = 5.0f;


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
		else if (@event.IsActionPressed("SwitchCameraShiftSide"))
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


	private bool MoveCharacterViaControls()
	{
		var hasCharacterMoved = false;
		var velocity = Character.Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var direction = (Character.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			hasCharacterMoved = true;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Character.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Character.Velocity.Z, 0, Speed);
		}

		Character.Velocity = velocity;
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
		if (AimSensor.GetCollider() is IInteractableObject interactable)
		{
			interactable.Highlite();
			if (Input.IsActionJustPressed("Interact"))
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
