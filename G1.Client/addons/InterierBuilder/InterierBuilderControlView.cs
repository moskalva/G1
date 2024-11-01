using Godot;
using System;

public partial class InterierBuilderControlView : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnUpButtonClick(){
		GD.Print("Up");
	}
	public void OnDownButtonClick(){
		GD.Print("Down");
	}
	public void OnLeftButtonClick(){
		GD.Print("Left");
	}
	public void OnRightButtonClick(){
		GD.Print("Right");
	}
	public void OnFloorButtonClick(){
		GD.Print("Floor");
	}
	public void OnCielingButtonClick(){
		GD.Print("Cieling");
	}
}
