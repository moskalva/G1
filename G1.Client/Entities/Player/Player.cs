using Godot;
using System;
using System.Diagnostics.Contracts;


public partial class Player : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float CameraSpeed = 0.1f;
	[Export]
	public float CharacterRotationSpeed = 3f;
	[Export]
	public Node3D FocusPoint { get; set; }
	[Export]
	public float MinimalCameraDistance = 3f;

	[Export]
	public RayCast3D RayCast { get; set; }

	private Vector2 mouseMoveInput;
	private float cameraDistance;

	[Export]
	public Camera3D Camera { get; set; }

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		cameraDistance = MinimalCameraDistance;
		Camera.Transform = FocusPoint.GlobalTransform
			.Translated(new Vector3(0, 0, cameraDistance));
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseMotion mouseMove)
		{
			mouseMoveInput = mouseMove.Relative;
		}
		else if (@event.IsActionPressed("SwitchCameraShiftSide"))
		{
			SwitchCameraShiftSide();
		}
		else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelDown:
					cameraDistance -= 1;
					if (cameraDistance < MinimalCameraDistance)
						cameraDistance = MinimalCameraDistance;
					break;
				case MouseButton.WheelUp:
					cameraDistance += 1;
					break;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveCameraViaMouse(delta);

		var initialCameraPosition = this.Position;
		var hasCharacterMoved = MoveCharacterViaControls();
		if (hasCharacterMoved)
			RotateCharacterAsCamera(delta);

		MoveCameraBehindCharacter(initialCameraPosition);
		var interactable = RayCast.GetCollider() as IInteractableObject;
		if (interactable != null)
		{
			interactable.Highlite();
		}
	}

	private void MoveCameraViaMouse(double delta)
	{
		Camera.Transform = Camera.Transform
			.LookingAt(FocusPoint.GlobalPosition)
			.Teleport(FocusPoint.GlobalPosition)
			.RotatedLocal(Vector3.Down, (float)(mouseMoveInput.X * CameraSpeed * delta))
			.RotatedLocal(Vector3.Left, (float)(mouseMoveInput.Y * CameraSpeed * delta))
			.TranslatedLocal(new Vector3(0, 0, cameraDistance));

		RayCast.Transform = this.Transform.AffineInverse() *
			new Transform3D(Camera.Transform.Basis, FocusPoint.GlobalPosition);
		mouseMoveInput = Vector2.Zero;
	}

	private bool MoveCharacterViaControls()
	{
		var hasCharacterMoved = false;
		var velocity = this.Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			hasCharacterMoved = true;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		this.Velocity = velocity;
		MoveAndSlide();

		return hasCharacterMoved;
	}

	private void SwitchCameraShiftSide()
	{
		var translate = new Vector3(-1 * FocusPoint.Position.X * 2, 0, 0);
		FocusPoint.Transform = FocusPoint.Transform
			.TranslatedLocal(translate);
		Camera.Transform = Camera.Transform
			.Rotate(FocusPoint.GlobalBasis)
			.TranslatedLocal(translate)
			.Rotate(Camera.Transform.Basis);
	}

	private void RotateCharacterAsCamera(double delta)
	{
		var angle = Mathf.RotateToward(0, Camera.Rotation.Y - this.Rotation.Y, CharacterRotationSpeed * delta);
		this.Rotate(Vector3.Up, (float)angle);
	}

	private void MoveCameraBehindCharacter(Vector3 initialCameraPosition)
	{
		var movedDistance = this.Position - initialCameraPosition;
		Camera.Position += movedDistance;
	}
}
