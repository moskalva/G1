
using Godot;

public enum WallType { None, Wall, Door }
[Tool]
[GlobalClass]
public partial class InterierMapTile : Resource
{
    [Export]
    public int X { get; set; }
    [Export]
    public int Y { get; set; }
    [Export]
    public bool Floor { get; set; }
    [Export]
    public bool Ceiling { get; set; }
    [Export]
    public WallType Front { get; set; }
    [Export]
    public WallType Back { get; set; }
    [Export]
    public WallType Left { get; set; }
    [Export]
    public WallType Right { get; set; }

    public bool IsEmpty => !Floor
                        && !Ceiling
                        && Front == WallType.None
                        && Back == WallType.None
                        && Left == WallType.None
                        && Right == WallType.None;
}