using Godot;
using System;

public abstract partial class BaseDoorPanel : Node3D, IInteractableObject
{
	protected BaseDoor door;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		door = this.GetAccendant<BaseDoor>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Highlite()
	{
	}

	public void Interact()
	{
		if (this.door.IsOpened)
			this.door.Close();
		else
			this.door.Open();
	}
}
