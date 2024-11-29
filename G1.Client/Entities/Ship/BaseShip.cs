

using System;
using System.Collections.Generic;
using Godot;

public partial class BaseShip : Node
{
    [Signal]
    public delegate void AccelerateEventHandler(Vector3 direction, float deltaVelocity);

    [Export]
    public SubViewport NavigationMapView { get; set; }
    [Export]
    public SubViewport FishEyeView { get; set; }

    public ShipController Controller { get; private set; }
    public override void _Ready()
    {
        Controller = this.FindNode<ShipController>();
    }
}