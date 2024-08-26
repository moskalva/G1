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

			state.SetAllProcessing(false);

			state.PlayerStateChanged += HandlePlayerStateChanged;
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
		var expectedCameraPosition = newState.GetInitialCameraTransform();

		if (Camera.Transform == expectedCameraPosition)
		{
			currentState = newState;
		}
		else
		{
			var transition = GetNode<Transition>("State/Transition");
			transition.ExpectedCameraTranfsorm = expectedCameraPosition;
			transition.NextState = newState;
			currentState = transition;
		}
		
		currentState.SetAllProcessing(true);
	}
}
