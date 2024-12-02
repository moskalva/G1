using Godot;
using System;
using System.Linq;

[Tool]
public partial class FisheyeSpot : Node3D
{
	public enum FisheyeCameraType { Horizontal, Vertical }
	[Export]
	public FisheyeCameraType ControlType { get; set; } = FisheyeCameraType.Horizontal;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			foreach (var child in this.GetChildren().OfType<Node3D>())
			{
				child.Hide();
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
