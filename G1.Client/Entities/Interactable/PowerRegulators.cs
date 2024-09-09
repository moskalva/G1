using System;

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