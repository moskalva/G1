using Godot;
using System;

public partial class DoubleDoorsV1 : BaseDoor
{
	private AnimationPlayer animation;
	private AnimationTree animationTree;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("Doors/AnimationPlayer");
		animationTree = GetNode<AnimationTree>("AnimationTree");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	protected override void StartClosing()
	{
		GD.Print($"StartClosing");
		animationTree.Set("parameters/Transition/transition_request", "Closing");
	}

	protected override void StartOpening()
	{
		GD.Print($"StartOpening");
		animationTree.Set("parameters/Transition/transition_request", "Opening");
	}

	public void AnimationFinished(StringName name)
	{
		if (name == "Door-colonlyAction")
		{
			var state = (string)animationTree.Get("parameters/Transition/current_state");
			if (state.Equals("Closing"))
				this.SetState(false);
			else if (state.Equals("Opening"))
				this.SetState(true);
		}
	}
}
