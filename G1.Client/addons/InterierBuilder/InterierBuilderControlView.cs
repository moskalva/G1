using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class InterierBuilderControlView : Control
{
	[Signal]
	public delegate void ExportEventHandler(Dictionary<Vector2I, InterierMapTile> tiles);

	private InterierMap map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode<InterierMap>("HBoxContainer/MarginContainer2/SubViewportContainer/SubViewport/InterierMap");
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

	public override void _UnhandledInput(InputEvent @event)
	{
		map._UnhandledInput(@event);
	}

	private void OnExportButtonClick()
	{
		GD.Print("Export clicked");
		EmitSignal(SignalName.Export, this.map.Tiles);
	}

	private void OnCellChanged(Vector2I cellIndex, CellChangeTarget target, CellChangeType type)
	{
		GD.Print($"Cell '{cellIndex}' changed. Target: '{target}', type: '{type}'");
	}

}
