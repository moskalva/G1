using Godot;
using System;

public partial class Character : CharacterBody3D
{
	private Node3D head;
	private AnimationTree animation;

	public void SetHeadDirection(Vector3 direction)
	{
		head.Transform = head.Transform.LookTowards(direction);
	}

	public Vector3 HeadPosition => this.head.Position;

	public CharacterPosture Posture { get; set; }

	public override void _Ready()
	{
		this.head = GetNode<Node3D>("Head");
		this.animation = GetNode<AnimationTree>("AnimationTree");
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		var hVelocity = new Vector2(this.Velocity.X, -this.Velocity.Z).Rotated(-this.Rotation.Y);
		animation.Set("parameters/Standing/blend_position", hVelocity);
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
