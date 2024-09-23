using System;
using Godot;
using System.Linq;

public class PowerRegulators
{
	private int currentIndex = 0;
	private readonly (PowerRegulator, PowerIndicator)[] controls;

	public PowerRegulators(params (PowerRegulator, PowerIndicator)[] controls)
	{
		if (controls == null || controls.Length == 0)
			throw new ArgumentException("Controls set cannot be empty");
		if (controls.Any(c => c.Item1 is null || c.Item2 is null))
			throw new ArgumentException("Control cannot be null");
		this.controls = controls;
	}

	public PowerRegulator Current => controls[currentIndex].Item1;

	internal void Next()
	{
		currentIndex += 1;
		if (currentIndex == controls.Length)
			currentIndex = 0;
		UpdateIndicators();
	}

	internal void Previous()
	{
		currentIndex -= 1;
		if (currentIndex == -1)
			currentIndex = controls.Length - 1;
		UpdateIndicators();
	}

	private void UpdateIndicators()
	{
		for (int i = 0; i < controls.Length; i++)
		{
			var (_, indicator) = controls[i];
			indicator.IsActive = i == currentIndex;
		}
	}
}