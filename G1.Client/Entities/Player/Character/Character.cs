using Godot;
using System;

public partial class Character : CharacterBody3D
{
	private Node3D head;

	public void SetHeadDirection(Vector3 direction)
	{
		head.Transform = head.Transform.LookTowards(direction);
	}

	public Vector3 HeadPosition => this.head.Position;

	public CharacterPosture Posture { get; set; }

	public override void _Ready()
	{
		this.head = GetNode<Node3D>("Head");
	}
	
	public override void _PhysicsProcess(double delta)
	{
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
