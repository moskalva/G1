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
		if (!this.Enabled)
			return;
		base._Input(@event);
		GD.Print($"Got input");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!this.Enabled)
			return;

	}

	public void Highlite()
	{
		GD.Print($"Highlited");
	}

	public void Interact()
	{
		GD.Print($"Interacted");
		EmitSignal(SignalName.ActivateController, this);
	}
}
