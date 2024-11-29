using Godot;
using System;
using G1.Model;
using System.Collections.Generic;
using System.Linq;

public partial class Exterier : RigidBody3D
{
	private Queue<PushChainEntry> pushQueue = new Queue<PushChainEntry>();

	public FisheyeSpot[] FisheyeSpots { get; private set; }
	public override void _EnterTree()
	{
		this.FisheyeSpots = this.FindNodes<FisheyeSpot>().ToArray();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		while (pushQueue.TryDequeue(out var acceleration))
		{
			var force = this.Transform.Basis * (acceleration.Direction * acceleration.Magnitude);
			GD.Print($"ApplyForce '{force}'");
			if (acceleration.IsForce)
				state.ApplyForce(force);
			else
				state.ApplyTorque(force);
		}
	}

	public void OnPush(Vector3 direction, float magnitude)
	{
		GD.Print($"OnPush direction: '{direction}', magnitude: '{magnitude}'");
		pushQueue.Enqueue(new PushChainEntry { IsForce = true, Direction = direction, Magnitude = magnitude });
	}

	public void OnTorque(Vector3 direction, float magnitude)
	{
		GD.Print($"OnTorque direction: '{direction}', magnitude: '{magnitude}'");
		pushQueue.Enqueue(new PushChainEntry { IsForce = false, Direction = direction, Magnitude = magnitude });
	}

	private struct PushChainEntry
	{
		public bool IsForce;
		public Vector3 Direction;
		public float Magnitude;
	}
}
