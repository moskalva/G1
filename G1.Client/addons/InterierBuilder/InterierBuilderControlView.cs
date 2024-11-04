using Godot;
using System;

[Tool]
public partial class InterierBuilderControlView : Control
{
	[Signal]
	public delegate void ExportEventHandler();

	private Node map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode("HBoxContainer/MarginContainer2/SubViewportContainer/SubViewport/InterierMap");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		// input is not propagated through in editor, doing manually
		map._Input(@event);
	}

	public void OnUpButtonClick()
	{
		GD.Print("Up");
	}
	public void OnDownButtonClick()
	{
		GD.Print("Down");
	}
	public void OnLeftButtonClick()
	{
		GD.Print("Left");
	}
	public void OnRightButtonClick()
	{
		GD.Print("Right");
	}
	public void OnFloorButtonClick()
	{
		GD.Print("Floor");
	}
	public void OnCielingButtonClick()
	{
		GD.Print("Cieling");
	}
	public void OnExportButtonClick()
	{
		GD.Print("Export clicked");
		EmitSignal(SignalName.Export);
	}
}
