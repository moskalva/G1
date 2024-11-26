using Godot;
using System;
using System.Collections.Generic;

public partial class PilotSeat : ControlPlace, IInteractableObject
{

	public override Transform3D CharacterPosition => this.Transform.TranslatedLocal(new Vector3(0, 0, 0.5f));

	public override CharacterPosture CharacterPosture => CharacterPosture.Sitting;

	private StaticBody3D frontScreen;
	private StaticBody3D leftScreen;
	private StaticBody3D rightScreen;
	private Dictionary<StaticBody3D, Node> managementTargetMap = new Dictionary<StaticBody3D, Node>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.frontScreen = GetNode<StaticBody3D>("FrontScreen");
		this.leftScreen = GetNode<StaticBody3D>("LeftScreen");
		this.rightScreen = GetNode<StaticBody3D>("RightScreen");
		var power = GetNode<PowerManagement>("PowerManagement");
		var flight = GetNode<FlightManagement>("FlightManagement");

		managementTargetMap[frontScreen] = flight;
		managementTargetMap[leftScreen] = power;
		foreach(var manager in managementTargetMap.Values)
			manager.SetAllProcessing(false);

		SetUpScreen(frontScreen.GetNode<MeshInstance3D>("Screen"), flight.Viewport);
		SetUpScreen(leftScreen.GetNode<MeshInstance3D>("Screen"), power.Viewport);  
	}

	public override void AimingAt(Node aimTarget)
	{
		foreach (var (target, manager) in managementTargetMap)
		{
			var enabled = target == aimTarget;
			manager.SetAllProcessing(enabled);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{

	}

	public void Highlite()
	{
	}

	public void Interact()
	{
		GD.Print($"Interacted");
	}

	private void SetUpScreen(MeshInstance3D screen, SubViewport viewport)
	{
		if (viewport is null) return;

		var texture = viewport.GetTexture();
		var material = (BaseMaterial3D)screen.MaterialOverride;
		// todo resize texture
		material.AlbedoTexture = texture;
	}
}
