using Godot;
using System;

public partial class PilotSeat : ControlPlace, IInteractableObject
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Highlite()
	{
		GD.Print($"Highlited");
	}

	public void Interact()
	{
		GD.Print($"Interacted");
	}
}
