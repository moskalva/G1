using System;
using Godot;

public abstract partial class ControlPlace : StaticBody3D
{
    [Signal]
    public delegate void ActivateControllerEventHandler(ControlPlace obj);

    [Signal]
    public delegate void DeactivateControllerEventHandler(ControlPlace obj);

    [Export]
    public bool Enabled { get; set; }

    [Export]
    public Camera3D Camera { get; set; }

    public override void _Input(InputEvent @event)
    {
        if (!this.Enabled)
            return;

        if (@event.IsActionPressed("LeaveControlPlace"))
        {
            EmitSignal(SignalName.DeactivateController, this);
        }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RegisterAsCharacterController();
    }

    private void RegisterAsCharacterController()
    {
        var parent = this.GetParent();
        while (!(parent is IControlPlaceManager))
        {
            if (parent == null)
                throw new InvalidOperationException("Could not find controller manager");

            parent = parent.GetParent();
        }
        ((IControlPlaceManager)parent).RegisterCharacterController(this);
    }
}

public interface IControlPlaceManager
{
    void RegisterCharacterController(ControlPlace controller);
}