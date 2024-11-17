using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class InterierMapResource : Resource
{
	public Dictionary<Vector2I, InterierMapTile> Tiles { get; set; } = new();
}


public enum WallType { None, Wall, Door }
[Tool]
[GlobalClass]
public partial class InterierMapTile : GodotObject
{
	public int X { get; set; }
	public int Y { get; set; }
	public bool Floor { get; set; }
	public bool Ceiling { get; set; }
	public WallType Front { get; set; }
	public WallType Back { get; set; }
	public WallType Left { get; set; }
	public WallType Right { get; set; }

	public bool IsEmpty => !Floor
						&& !Ceiling
						&& Front == WallType.None
						&& Back == WallType.None
						&& Left == WallType.None
						&& Right == WallType.None;
}