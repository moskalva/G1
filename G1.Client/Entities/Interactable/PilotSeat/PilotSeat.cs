using Godot;
using System;

public partial class PilotSeat : ControlPlace, IInteractableObject
{
	[Export]
	public Engine Engine { get; set; }

	public SubViewport NavigationMapView{get;set;}

	private Lazy<PowerRegulators> powerRegulators;

	public override Transform3D CharacterPosition => this.Transform.TranslatedLocal(new Vector3(0, 0, 1f));

	public override CharacterPosture CharacterPosture => CharacterPosture.Sitting;

	private MeshInstance3D navigationScreen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.navigationScreen = GetNode<MeshInstance3D>("FrontScreen");
		this.powerRegulators = new Lazy<PowerRegulators>(() =>
		{
			var enginePowerRegulator = this.Engine.Power;
			return new PowerRegulators(enginePowerRegulator);
		});
	}

	public override void _Input(InputEvent @event)
	{
		if (!this.IsActive)
			return;

		if (@event.IsActionPressed("PilotSeat.NextPowerRegulator"))
		{
			powerRegulators.Value.Next();
		}
		else if (@event.IsActionPressed("PilotSeat.PreviousPowerRegulator"))
		{
			powerRegulators.Value.Previous();
		}
		else if (@event.IsActionPressed("PilotSeat.IncreasePower"))
		{
			powerRegulators.Value.Current.Increase();
		}
		else if (@event.IsActionPressed("PilotSeat.DecreasePower"))
		{
			powerRegulators.Value.Current.Decrease();
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
		SetScreen(this.NavigationMapView);
	}

	public void Highlite()
	{
	}

	public void Interact()
	{
		GD.Print($"Interacted");
	}

	public void SetScreen(SubViewport viewport)
	{
		var texture = viewport.GetTexture();
		var material = (BaseMaterial3D)this.navigationScreen.MaterialOverride;
		material.AlbedoTexture = texture;
	}
}
