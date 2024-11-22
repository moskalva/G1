using Godot;
using System;

public partial class DoorPanel : Node3D, IInteractableObject
{
	private BaseDoor door;

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
		GD.Print($"Interact DoorPanel '{this.door.IsOpened}'");
		if (this.door.IsOpened)
			this.door.Close();
		else
			this.door.Open();
	}
}
