

using System;
using System.Collections.Generic;
using Godot;

public partial class BaseShip : Node
{
    [Signal]
    public delegate void AccelerateEventHandler(Vector3 direction, float deltaVelocity);

    [Export]
    public SubViewport ExternalWorld { get; set; }

    [Export]
    public int ShipMass { get; set; } = 10_000;

    public ShipController Controller { get; private set; }

    private Queue<PushChainEntry> pushQueue = new Queue<PushChainEntry>();
    public override void _Ready()
    {
        Controller = this.FindNode<ShipController>();
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        while (pushQueue.TryDequeue(out var acceleration))
        {
            var deletaVelocity = Mathf.Sqrt(acceleration.Magnitude / ShipMass) * delta;
            EmitSignal(SignalName.Accelerate, acceleration.Direction, deletaVelocity);
        }
    }

    public void OnDragPush(float force)
    {
        pushQueue.Enqueue(new PushChainEntry { Direction = Vector3.Forward, Magnitude = force });
    }

    private struct PushChainEntry
    {
        public Vector3 Direction;
        public float Magnitude;
    }
}