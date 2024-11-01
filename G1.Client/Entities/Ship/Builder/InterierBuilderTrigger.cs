#if TOOLS
using Godot;
using System;

[Tool]
public partial class InterierBuilderTrigger : EditorScript
{
	// Called when the script is executed (using File -> Run in Script Editor).
	public override void _Run()
	{
		GD.Print("InterierBuilderTrigger Run");
		var builder = GetScene().GetNode<InterierBuilder>("InterierBuilder");
		if (builder is null)
		{
			GD.Print("Could not find InterierBuilder in a scene. Skip.");
			return;
		}
		builder.Build();
	}
}
#endif
