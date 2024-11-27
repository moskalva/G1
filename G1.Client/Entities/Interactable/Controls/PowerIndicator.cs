using Godot;
using System;

[Tool]
public partial class PowerIndicator : Panel
{
	private bool isActive = true;
	[Export]
	public bool IsActive { get => isActive; set { isActive = value; QueueRedraw(); } }
	
	private uint maxValue = 5;
	[Export]
	public uint MaxValue { get => maxValue; set { maxValue = value; QueueRedraw(); } }

	private uint currentValue = 3;
	[Export]
	public uint CurrentValue { get => currentValue; set { currentValue = value; QueueRedraw(); } }

	private int borderSize = 10;
	[Export]
	public int BorderSize { get => borderSize; set { borderSize = value; QueueRedraw(); } }

	private int levelElementHeight = 10;
	[Export]
	public int LevelElementHeight { get => levelElementHeight; set { levelElementHeight = value; QueueRedraw(); } }

	private int levelElementDistance = 10;
	[Export]
	public int LevelElementDistance { get => levelElementDistance; set { levelElementDistance = value; QueueRedraw(); } }

	private Color activeColor = Colors.Aqua;
	[Export]
	public Color ActiveColor { get => activeColor; set { activeColor = value; QueueRedraw(); } }

	private Color passiveColor = Colors.DarkBlue;
	[Export]
	public Color PassiveColor { get => passiveColor; set { passiveColor = value; QueueRedraw(); } }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Draw()
	{
		var color = IsActive ? ActiveColor : PassiveColor;
		var height = BorderSize + LevelElementDistance + MaxValue * (LevelElementDistance + LevelElementHeight);

		var border = new Rect2(
			new Vector2(BorderSize / 2, this.Size.Y - height - BorderSize / 2),
			new Vector2(this.Size.X - BorderSize, height));
		this.DrawRect(border, color, filled: false, width: BorderSize);


		var levelSize = new Vector2(this.Size.X - BorderSize * 2 - LevelElementDistance * 2, LevelElementHeight);
		for (int i = 1; i <= CurrentValue; i++)
		{
			var position = new Vector2(BorderSize + LevelElementDistance, this.Size.Y - BorderSize - i * (LevelElementHeight + LevelElementDistance));
			var element = new Rect2(
				position,
				levelSize
			);
			DrawRect(element, color, filled: true);
		}
	}
}
