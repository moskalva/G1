using Godot;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata;


public partial class Player : Node
{

	[Export]
	public Camera3D Camera { get; set; }

	[Export]
	public Character Character { get; set; }

	private BaseState currentState;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		var rayCast = GetNode<RayCast3D>("RayCast");
		rayCast.AddException(Character);
		SetupStates(rayCast);
	}

	public override void _Input(InputEvent @event)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	private void SetupStates(RayCast3D rayCast)
	{
		var allStates = GetNode("State").GetChildren().Where(x => x is BaseState).Cast<BaseState>().ToList();
		GD.Print($"Setup states number {allStates.Count()}");
		foreach (var state in allStates)
		{
			state.Character = Character;
			state.Camera = Camera;
			state.RayCast = rayCast;
			state.Player = this;

			state.SetAllProcessing(false);
		}

		SetCurrentState(GetNode<Walking>("State/Walking"));
	}

	private void HandlePlayerStateChanged(BaseState newState)
	{
		currentState.SetAllProcessing(false);
		SetCurrentState(newState);
	}

	private void SetCurrentState(BaseState newState)
	{
		GD.Print($"Next state {newState.Name}");
		var expectedCameraPosition = newState.GetInitialCameraTransform();

		if (Camera.Transform.IsEqualApprox(expectedCameraPosition))
		{
			currentState = newState;
		}
		else
		{
			GD.Print("Transition required.");
			var transition = GetNode<Transition>("State/Transition");
			transition.ExpectedCameraTranfsorm = expectedCameraPosition.Orthonormalized();
			transition.NextState = newState;
			currentState = transition;
		}

		currentState.SetAllProcessing(true);
	}

	private void _OnControlModeRequested(ControlPlace controlPlace)
	{
		GD.Print("Control mode requested.");
		if (controlPlace.CharacterPosture == CharacterPosture.Sitting)
		{
			var sitting = GetNode<Sitting>("State/Sitting");
			sitting.CharacterPosition = controlPlace.CharacterPosition;
			HandlePlayerStateChanged(sitting);
		}
	}
	private void _OnTransitionCompleted(BaseState nextState)
	{
		GD.Print("Transition completed.");
		HandlePlayerStateChanged(nextState);
	}
	private void _OnLeave()
	{
		GD.Print("Leave.");
		var nextState = GetNode<Walking>("State/Walking");
		HandlePlayerStateChanged(nextState);
	}
}
