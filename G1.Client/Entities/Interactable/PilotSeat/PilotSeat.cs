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
	private DragThruster dragThruster;
	private PowerManagement powerManagement;

	private Dictionary<StaticBody3D, Node> managementTargetMap = new Dictionary<StaticBody3D, Node>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var ship = this.GetAccendant<BaseShip>();
		this.frontScreen = GetNode<StaticBody3D>("FrontScreen");
		this.leftScreen = GetNode<StaticBody3D>("LeftScreen");
		this.rightScreen = GetNode<StaticBody3D>("RightScreen");
		this.dragThruster = ShipSystems.GetRegistered<DragThruster>(this);
		this.powerManagement = GetNode<PowerManagement>("PowerManagement");

		managementTargetMap[leftScreen] = powerManagement;
		foreach(var manager in managementTargetMap.Values)
			manager.SetAllProcessing(false);

		SetUpScreen(frontScreen.GetNode<MeshInstance3D>("Screen"), ship.ExternalWorld);
		SetUpScreen(leftScreen.GetNode<MeshInstance3D>("Screen"), powerManagement.Viewport);
	}

	public override void AimingAt(Node aimTarget)
	{
		foreach (var (target, manager) in managementTargetMap)
		{
			var enabled = target == aimTarget;
			manager.SetAllProcessing(enabled);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (!this.IsActive)
			return;

		if (@event.IsAction("PilotSeat.EngineBurn"))
		{
			this.dragThruster.Burn();
		}
		else if (@event.IsAction("PilotSeat.RotateShipUp"))
		{
			GD.Print($"RotateShipUp");
		}
		else if (@event.IsAction("PilotSeat.RotateShipDown"))
		{
			GD.Print($"RotateShipDown");
		}
		else if (@event.IsAction("PilotSeat.RotateShipLeft"))
		{
			GD.Print($"RotateShipLeft");
		}
		else if (@event.IsAction("PilotSeat.RotateShipRight"))
		{
			GD.Print($"RotateShipRight");
		}
		else if (@event.IsActionPressed("PilotSeat.ToggleNavigationMap"))
		{
			GD.Print($"ToggleNavigationMap");
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
