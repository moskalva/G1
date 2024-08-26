using System;
using Godot;

public abstract partial class ControlPlace : StaticBody3D
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("LeaveControlPlace"))
        {
        }
    }
}