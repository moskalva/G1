using Godot;

public partial class WalkingState : BaseState
{
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float CharacterRotationSpeed = 3f;
    [Export]
    public float MinimalCameraDistance = 3f;

    [Export]
    public Vector3 FocusPointShift { get; set; }

    private float cameraDistance;

    private Vector2 mouseMoveInput;

    public override void _Ready()
    {
        cameraDistance = MinimalCameraDistance;
    }

    public override void _Input(InputEvent @event)
    {
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
                case MouseButton.WheelUp:
                    cameraDistance -= 1;
                    if (cameraDistance < MinimalCameraDistance)
                        cameraDistance = MinimalCameraDistance;
                    break;
                case MouseButton.WheelDown:
                    cameraDistance += 1;
                    break;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveCameraViaMouse(delta);

        var initialCameraPosition = Character.Position;
        var hasCharacterMoved = MoveCharacterViaControls();
        if (hasCharacterMoved)
            RotateCharacterAsCamera(delta);

        MoveCameraBehindCharacter(initialCameraPosition);
        Interact();
    }

    private void MoveCameraViaMouse(double delta)
    {
        var focusPoint = Character.Transform.TranslatedLocal(FocusPointShift).Origin;
        Camera.Transform = Camera.Transform
            .LookingAt(focusPoint)
            .Teleport(focusPoint)
            .RotatedLocal(Vector3.Down, (float)(mouseMoveInput.X * CameraSpeed * delta))
            .RotatedLocal(Vector3.Left, (float)(mouseMoveInput.Y * CameraSpeed * delta))
            .TranslatedLocal(new Vector3(0, 0, cameraDistance));

        RayCast.Transform =
            new Transform3D(Camera.Transform.Basis, focusPoint);
        mouseMoveInput = Vector2.Zero;
    }

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

    private void SwitchCameraShiftSide()
    {
        var shiftX = -1 * FocusPointShift.X * 2;
        FocusPointShift = new Vector3(FocusPointShift.X + shiftX, FocusPointShift.Y, FocusPointShift.Z);
        Camera.Transform = Camera.Transform
            .Rotate(Character.Basis)
            .TranslatedLocal(new Vector3(shiftX, 0, 0))
            .Rotate(Camera.Transform.Basis);
    }

    private void RotateCharacterAsCamera(double delta)
    {
        var angle = Mathf.RotateToward(0, Camera.Rotation.Y - Character.Rotation.Y, CharacterRotationSpeed * delta);
        Character.Rotate(Vector3.Up, (float)angle);
    }

    private void MoveCameraBehindCharacter(Vector3 initialCameraPosition)
    {
        var movedDistance = Character.Position - initialCameraPosition;
        Camera.Position += movedDistance;
    }

    private void Interact()
    {
        if (RayCast.GetCollider() is IInteractableObject interactable)
        {
            interactable.Highlite();
            if (Input.IsActionJustPressed("Interact"))
                interactable.Interact();
        }
    }
}