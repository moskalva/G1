using System;
using Godot;

public partial class Transition : BaseState
{
    [Export]
    public float CameraRotationSpeed { get; set; } = 5f;
    [Export]
    public float CameraMoveSpeed { get; set; } = 3f;


    public Transform3D ExpectedCameraTranfsorm { get; set; }
    public BaseState NextState { get; set; }

    public override Transform3D GetInitialCameraTransform() => Camera.Transform;

    public override void _PhysicsProcess(double delta)
    {
        if (TryTransformCamera(delta))
            EmitSignal(SignalName.PlayerStateChanged, NextState);
    }

    private bool TryTransformCamera(double delta)
    {
        if (ExpectedCameraTranfsorm == Camera.Transform)
            return true;

        var transorm = Camera.Transform
            .Teleported(new Vector3(
                Mathf.MoveToward(Camera.Transform.Origin.X, ExpectedCameraTranfsorm.Origin.X, (float)(delta * CameraMoveSpeed)),
                Mathf.MoveToward(Camera.Transform.Origin.Y, ExpectedCameraTranfsorm.Origin.Y, (float)(delta * CameraMoveSpeed)),
                Mathf.MoveToward(Camera.Transform.Origin.Z, ExpectedCameraTranfsorm.Origin.Z, (float)(delta * CameraMoveSpeed))
            ))
            .Rotated(GetRotationBasis());
        Camera.Transform = transorm.Orthonormalized();

        return false;

        Basis GetRotationBasis()
        {
            var current = Camera.Transform.Basis.GetRotationQuaternion();
            var expected = ExpectedCameraTranfsorm.Basis.GetRotationQuaternion();
            var rotationWeight = current.AngleTo(expected) < 0.005f
                            ? 1
                            : (float)(delta * CameraRotationSpeed);
            return new Basis(current.Slerp(expected, rotationWeight));
        }
    }
}