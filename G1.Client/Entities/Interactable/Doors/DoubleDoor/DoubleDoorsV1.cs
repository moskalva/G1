using Godot;
using System;

public partial class DoubleDoorsV1 : BaseDoor
{
	private AnimationTree animation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimationTree>("AnimationTree");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	protected override void StartClosing()
	{
		animation.Set("parameters/Transition/transition_request", "Closing");
	}

	protected override void StartOpening()
	{
		animation.Set("parameters/Transition/transition_request", "Opening");
	}

	public void AnimationFinished(StringName name)
	{
		if (name == "Door-colonlyAction")
		{
			var state = (string)animation.Get("parameters/Transition/current_state");
			if (state.Equals("Closing"))
				this.SetState(false);
			else if (state.Equals("Opening"))
				this.SetState(true);
		}
	}
}
