using Godot;
using System;

public partial class Character : CharacterBody3D
{
	[Export]
	public ulong ShootBoredAnimationAfterMs { get; set; } = (ulong)TimeSpan.FromSeconds(15).TotalMilliseconds;

	public CharacterPosture Posture { get; set; }

	private Node3D head;
	private AnimationTree animation;

	public Vector3 HeadPosition => this.head.Position;

	private ulong lastAnimationChangeMs = 0;
	public override void _Ready()
	{
		this.head = GetNode<Node3D>("Head");
		this.animation = GetNode<AnimationTree>("AnimationTree");
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateAnimation();
	}

	public void SetHeadDirection(Vector3 direction)
	{
		head.Transform = head.Transform.LookTowards(direction);
	}


	private void UpdateAnimation()
	{
		var currentPosture = (bool)animation.Get("parameters/conditions/IsSitting") ? CharacterPosture.Sitting : CharacterPosture.Standing;
		if (currentPosture != Posture)
		{
			if (Posture == CharacterPosture.Standing)
			{
				animation.Set("parameters/conditions/IsSitting", false);
				animation.Set("parameters/conditions/IsStanding", true);
			}
			else
			{
				animation.Set("parameters/conditions/IsSitting", true);
				animation.Set("parameters/conditions/IsStanding", false);
			}
			AnimationChanged();
		}
		if (Posture == CharacterPosture.Standing)
		{
			var hVelocity = new Vector2(this.Velocity.X, -this.Velocity.Z).Rotated(-this.Rotation.Y);
			animation.Set("parameters/Standing/blend_position", hVelocity);
		}

		if (IsBorded())
		{
			if (Posture == CharacterPosture.Sitting)
			{
				animation.Set("parameters/Sitting/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
			}
			AnimationChanged();
		}

		void AnimationChanged()
		{
			this.lastAnimationChangeMs = Time.GetTicksMsec();
		}
		bool IsBorded() => Time.GetTicksMsec() - lastAnimationChangeMs > ShootBoredAnimationAfterMs;
	}

	public void GoTo(Transform3D expectedPosition)
	{
		this.Transform = expectedPosition;
	}

	public bool IsIdle()
	{
		return true;
	}
}
