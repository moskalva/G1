using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class InterierBuilderControlView : Control
{
	[Signal]
	public delegate void ExportEventHandler(Dictionary<Vector2I, InterierMapTile> tiles);

	private InterierMap map;
	private SubViewport subviewPort;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode<InterierMap>("HBoxContainer/MarginContainer2/SubViewportContainer/SubViewport/InterierMap");
		subviewPort = GetNode<SubViewport>("HBoxContainer/MarginContainer2/SubViewportContainer/SubViewport");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		// input is not propagated through in editor, doing manually
		subviewPort.HandleInputLocally = true;
		subviewPort.PushInput(@event, true);
	}

	private void OnExportButtonClick()
	{
		GD.Print("Export clicked");
		EmitSignal(SignalName.Export, this.map.Tiles);
	}

	public void SetTiles(Dictionary<Vector2I, InterierMapTile> tiles)
	{
		GD.Print($"SetTiles {tiles.Count}");
		map.Tiles=tiles;
	}
}
