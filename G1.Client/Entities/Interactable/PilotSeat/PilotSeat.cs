using Godot;
using System;

public partial class PilotSeat : ControlPlace, IInteractableObject
{

	[Export]
	public Engine Engine { get; set; }


	private PowerRegulators powerRegulators;

	public override Transform3D CharacterPosition => this.Transform.TranslatedLocal(new Vector3(0, 0, 1f));

	public override CharacterPosture CharacterPosture => CharacterPosture.Sitting;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var enginePowerRegulator = this.Engine.Power;
		this.powerRegulators = new PowerRegulators(enginePowerRegulator);
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Highlite()
	{
	}

	public void Interact()
	{
		GD.Print($"Interacted");
	}
}
