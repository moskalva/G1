using Godot;

public partial class PowerRegulator : GodotObject
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