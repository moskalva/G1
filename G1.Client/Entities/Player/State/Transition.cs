using System;
using Godot;

public partial class Transition : BaseState
{
    [Signal]
    public delegate void TransitionComletedEventHandler(BaseState nextState);

    [Export]
    public float CameraRotationSpeed { get; set; } = 5f;
    [Export]
    public float CameraMoveSpeed { get; set; } = 8f;


    public BaseState NextState { get; set; }

    public override PlayerStateProperties InitialState { get; } = null;

    public override void _PhysicsProcess(double delta)
    {
        var expectedState = NextState.InitialState;
        if (TryTransformCamera(expectedState) &&
            TryMoveCharacter(expectedState))
            EmitSignal(SignalName.TransitionComleted, NextState);
    }

    private bool TryTransformCamera(PlayerStateProperties expectedState)
    {
        this.Player.SetViewType(expectedState.ViewType);
        return this.PointOfView.IsCameraIdle();
    }

    private bool TryMoveCharacter(PlayerStateProperties expectedState)
    {
        if (expectedState.CharacterPosition is { } expectedPosition)
            this.Character.GoTo(this.Player.Transform.AffineInverse() * expectedPosition);
        if (this.Character.IsIdle())
            this.Character.Posture = expectedState.CharacterPosture;
        return this.Character.IsIdle();
    }

    public override void OnEnterState() { }

    public override void OnLeaveState() { }
}