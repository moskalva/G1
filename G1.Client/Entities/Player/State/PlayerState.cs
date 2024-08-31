using Godot;
using System;
using System.Linq;

public partial class PlayerState : Node
{
	private Player player;
	private BaseState currentState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.player = this.GetParent<Player>();
		SetupStates(player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SetupStates(Player player)
	{
		var allStates = this.GetChildren().Where(x => x is BaseState).Cast<BaseState>().ToList();
		GD.Print($"Setup states number {allStates.Count()}");
		foreach (var state in allStates)
		{
			state.Player = player;
			state.SetAllProcessing(false);
		}

		TransitionToState(GetNode<Walking>("Walking"));
	}

	private void TransitionToState(BaseState newState)
	{
		GD.Print($"Transition required {newState.Name}");
		var transition = GetNode<Transition>("Transition");
		transition.NextState = newState;
		SetCurrentState(transition);
	}

	private void SetCurrentState(BaseState newState)
	{
		currentState?.SetAllProcessing(false);
		GD.Print($"New state {newState.Name}");
		currentState = newState;
		currentState.SetAllProcessing(true);
	}

	private void _OnControlModeRequested(ControlPlace controlPlace)
	{
		GD.Print("Control mode requested.");
		if (controlPlace.CharacterPosture == CharacterPosture.Sitting)
		{
			var sitting = GetNode<Sitting>("Sitting");
			sitting.CharacterPosition = controlPlace.CharacterPosition;
			TransitionToState(sitting);
		}
	}

	private void _OnTransitionCompleted(BaseState nextState)
	{
		GD.Print("Transition completed.");
		SetCurrentState(nextState);
	}

	private void _OnLeave()
	{
		GD.Print("Leave.");
		var nextState = GetNode<Walking>("Walking");
		TransitionToState(nextState);
	}
}
