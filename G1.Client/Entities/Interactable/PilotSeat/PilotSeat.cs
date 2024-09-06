using Godot;
using System;


public class PowerRegulator
{
	private readonly uint maxPower;
	private uint currentLevel;

	public PowerRegulator(uint maxPower)
	{
		this.maxPower = maxPower;
	}

	public uint CurrentLevel => currentLevel;

	public void Increase()
	{
		if (currentLevel < maxPower)
			currentLevel += 1;
	}

	public void Decrease()
	{
		if (currentLevel > 0)
			currentLevel -= 1;
	}
}
public class PowerRegulators
{
	private int currentIndex = 0;
	private readonly PowerRegulator[] controls;

	public PowerRegulators(params PowerRegulator[] controls)
	{
		if (controls == null || controls.Length == 0)
			throw new ArgumentException("Controls set cannot be empty");
		this.controls = controls;
	}

	public PowerRegulator Current => controls[currentIndex];

	internal void Next()
	{
		currentIndex += 1;
		if (currentIndex == controls.Length)
			currentIndex = 0;
	}

	internal void Previous()
	{
		currentIndex -= 1;
		if (currentIndex == -1)
			currentIndex = controls.Length;
	}
}
public partial class PilotSeat : ControlPlace, IInteractableObject
{
	private PowerRegulator enginePowerRegulator;
	private PowerRegulators powerRegulators;

	public override Transform3D CharacterPosition => this.Transform.TranslatedLocal(new Vector3(0, 0, 1f));

	public override CharacterPosture CharacterPosture => CharacterPosture.Sitting;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.enginePowerRegulator = new PowerRegulator(5);
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
			GD.Print($"BurnEngine");
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
