using Godot;
using System;

public partial class Character : CharacterBody3D
{
	private Node3D head;

	public Basis HeadDirection
	{
		get => head.Transform.Basis;
		set => head.Transform = new Transform3D(value, head.Transform.Origin);
	}

	public override void _Ready()
	{
		this.head = GetNode<Node3D>("Head");
	}
	public override void _PhysicsProcess(double delta)
	{
	}
}
