using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class InterierMapResource : Resource
{
	[Export]
	public Dictionary<Vector2I, InterierMapTile> Tiles { get; set; }
}
