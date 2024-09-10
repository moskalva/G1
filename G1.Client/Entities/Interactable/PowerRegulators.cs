using System;
using Godot;
using System.Linq;

public class PowerRegulators
{
	private int currentIndex = 0;
	private readonly PowerRegulator[] controls;

	public PowerRegulators(params PowerRegulator[] controls)
	{
		if (controls == null || controls.Length == 0)
			throw new ArgumentException("Controls set cannot be empty");
		if(controls.Any(c => c is null))
			throw new ArgumentException("Control cannot be null");
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
			currentIndex = controls.Length - 1;
	}
}