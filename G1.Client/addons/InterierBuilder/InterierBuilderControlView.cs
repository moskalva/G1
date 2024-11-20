using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class InterierBuilderControlView : Control
{
	[Signal]
	public delegate void ExportEventHandler(Dictionary<Vector3I, InterierMapTile> tiles);
	[Signal]
	public delegate void ReloadEventHandler();

	private InterierMap map;
	private SubViewport subviewPort;
	private Label levelLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode<InterierMap>("HBoxContainer/MarginContainer2/SubViewportContainer/SubViewport/InterierMap");
		subviewPort = GetNode<SubViewport>("HBoxContainer/MarginContainer2/SubViewportContainer/SubViewport");
		levelLabel = GetNode<Label>("HBoxContainer/MarginContainer/VBoxContainer2/HBoxContainer/LevelLable");
		UpdateLevelLabel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (Godot.Engine.IsEditorHint())
		{
			// input is not propagated through in editor, doing manually
			subviewPort.HandleInputLocally = true;
			subviewPort.PushInput(@event, true);
		}
	}

	private void OnExportButtonClick()
	{
		EmitSignal(SignalName.Export, this.map.Tiles);
	}

	public void SetTiles(Dictionary<Vector3I, InterierMapTile> tiles)
	{
		GD.Print($"SetTiles {tiles.Count}");
		GD.Print($"map.Tiles {map}");
		this.map.Tiles = tiles;
	}

	public void OnLevelUpButtonClick()
	{
		this.map.CurrentFloorIndex = this.map.CurrentFloorIndex + 1;
		UpdateLevelLabel();
	}

	public void OnLevelDownButtonClick()
	{
		this.map.CurrentFloorIndex = this.map.CurrentFloorIndex - 1;
		UpdateLevelLabel();
	}

	public void OnClearButtonClick()
	{
		this.map.Clear();
		UpdateLevelLabel();
	}

	public void OnShiftLeftButtonClick() => this.map.ShiftLeft();
	public void OnShiftRightButtonClick() => this.map.ShiftRight();
	public void OnShiftFrontButtonClick() => this.map.ShiftFront();
	public void OnShiftBackButtonClick() => this.map.ShiftBack();
	public void OnShiftUpButtonClick()
	{
		OnLevelUpButtonClick();
		this.map.ShiftUp();
	}
	public void OnShiftDownButtonClick()
	{
		OnLevelDownButtonClick();
		this.map.ShiftDown();
	}

	public void OnReloadButtonClick() => EmitSignal(SignalName.Reload);

	private void UpdateLevelLabel()
	{
		levelLabel.Text = $"Level: {this.map.CurrentFloorIndex}";
	}
}
