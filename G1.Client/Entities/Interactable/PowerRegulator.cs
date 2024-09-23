using Godot;

public partial class PowerRegulator : GodotObject
{
	[Signal]
	public delegate void PowerLevelChangedEventHandler(uint newLevel);
	
	private readonly uint maxLevel;
	private uint currentLevel;

	public PowerRegulator(uint maxLevel)
	{
		this.maxLevel = maxLevel;
	}

	public uint CurrentLevel => currentLevel;
	public uint MaxLevel => maxLevel;

	public void Increase()
	{
		if (currentLevel < maxLevel)
		{
			currentLevel += 1;
			EmitSignal(SignalName.PowerLevelChanged, currentLevel);
		}
	}

	public void Decrease()
	{
		if (currentLevel > 0)
		{
			currentLevel -= 1;
			EmitSignal(SignalName.PowerLevelChanged, currentLevel);
		}
	}
}