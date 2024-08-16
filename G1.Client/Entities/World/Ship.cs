using Godot;
using System;

public partial class Ship : Node
{
	[Signal]
	public delegate void SwitchViewEventHandler(int mode);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			EmitSignal(SignalName.SwitchView, (int)ViewMode.NavigationMap);
		}
	}
}
