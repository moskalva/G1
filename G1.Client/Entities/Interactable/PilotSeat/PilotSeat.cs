using Godot;
using System;

public partial class PilotSeat : ControlPlace, IInteractableObject
{
	[Export]
	public Engine Engine { get; set; }

	public SubViewport NavigationMapView { get; set; }

	private PowerRegulators powerRegulators;

	public override Transform3D CharacterPosition => this.Transform.TranslatedLocal(new Vector3(0, 0, 0.5f));

	public override CharacterPosture CharacterPosture => CharacterPosture.Sitting;

	private MeshInstance3D navigationScreen;
	private MeshInstance3D powerManagementScreen;
	private MeshInstance3D shipSchematicsScreen;
	private StatsPanel statsPanel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.navigationScreen = GetNode<MeshInstance3D>("FrontScreen");
		this.powerManagementScreen = GetNode<MeshInstance3D>("LeftScreen");
		this.shipSchematicsScreen = GetNode<MeshInstance3D>("RightScreen");
		if (GetParent() is null) Init();
	}

	public void Init()
	{
		var powerViewPort = powerManagementScreen.GetNode<SubViewport>("SubViewport");
		statsPanel = powerViewPort.GetNode<StatsPanel>("StatsPanel");
		var einginePowerIndicator = statsPanel.GetNode<PowerIndicator>("PowerIndicator");

		SetNavigationScreen(this.NavigationMapView);
		SetPowerScreen(powerViewPort);

		var enginePowerRegulator = this.Engine.Power;
		einginePowerIndicator.MaxValue = enginePowerRegulator.MaxLevel;
		einginePowerIndicator.Init();
		enginePowerRegulator.PowerLevelChanged += (newLevel) => einginePowerIndicator.CurrentValue = newLevel;
		
		powerRegulators = new PowerRegulators((enginePowerRegulator, einginePowerIndicator));
	}

	public override void _Input(InputEvent @event)
	{
		if (!this.IsActive)
			return;

		if (@event.IsActionPressed("PilotSeat.NextPowerRegulator"))
		{
			powerRegulators.Next();
		}
		else if (@event.IsActionPressed("PilotSeat.PreviousPowerRegulator"))
		{
			powerRegulators.Previous();
		}
		else if (@event.IsActionPressed("PilotSeat.IncreasePower"))
		{
			powerRegulators.Current.Increase();
		}
		else if (@event.IsActionPressed("PilotSeat.DecreasePower"))
		{
			powerRegulators.Current.Decrease();
		}
		else if (@event.IsAction("PilotSeat.EngineBurn"))
		{
			this.Engine.Burn();
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

	private void SetNavigationScreen(SubViewport viewport)
	{
		if (viewport is null) return;

		var texture = viewport.GetTexture();
		var material = (BaseMaterial3D)this.navigationScreen.MaterialOverride;
		material.AlbedoTexture = texture;
	}

	private void SetPowerScreen(SubViewport powerViewPort)
	{
		var texture = powerViewPort.GetTexture();
		var material = (BaseMaterial3D)powerManagementScreen.MaterialOverride;
		// todo resize texture
		material.AlbedoTexture = texture;
	}
}
