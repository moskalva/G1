using Godot;
using System;
using G1.Model;

public partial class Interier : Node3D, IControlPlaceManager
{
	public WorldEntityId Id { get; set; }

	private Player player;
	private Camera3D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = this.GetNode<Player>(nameof(Player));
		player.Enabled = true;
		camera = this.GetNode<Camera3D>("Camera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void IControlPlaceManager.RegisterCharacterController(ControlPlace controller)
	{
		controller.ActivateController += ActivateController;
		controller.DeactivateController += DeactivateController;
		controller.Camera = camera;
	}

	private void ActivateController(ControlPlace controller)
	{
		player.Enabled = false;
		controller.Enabled = true;
	}

	private void DeactivateController(ControlPlace controller)
	{
		controller.Enabled = false;
		player.Enabled = true;
	}
}
