using System;
using Godot;

public partial class SmallDoorPanel : BaseDoorPanel
{
	private MeshInstance3D screen;

	public override void _Ready()
	{
		base._Ready();
		screen = this.GetNode<MeshInstance3D>("Screen");
		this.door.DoorStateChanged += OnDoorStateChanged;
		OnDoorStateChanged(this.door.DoorState);
	}

	private void OnDoorStateChanged(DoorState state)
	{
		var material = (BaseMaterial3D)this.screen.MaterialOverride;
		if (material is null)
		{
			material = new StandardMaterial3D();
			this.screen.MaterialOverride = material;
		}
		material.AlbedoColor = state switch
		{
			DoorState.Open => Colors.LightGreen,
			DoorState.Closed => Colors.Red,
			_ => Colors.Orange
		};
	}
}
