#if TOOLS
using System;
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class InterierBuilderLoader : EditorPlugin
{
	private EditorSelection selector;
	private InterierBuilderControlView control;
	private InterierBuilder builder;


	public override void _EnterTree()
	{
		var controlScene = GD.Load<PackedScene>("res://addons/InterierBuilder/InterierBuilderControlView.tscn");
		control = controlScene.Instantiate<InterierBuilderControlView>();
		control.Export += OnExport;

		selector = EditorInterface.Singleton.GetSelection();
		selector.SelectionChanged += OnSelectionChanged;

		var builder = GD.Load<Script>("res://addons/InterierBuilder/InterierBuilder.cs");
		var builderIcon = GD.Load<Texture2D>("res://Assets/icon.svg");
		AddCustomType("InterierBuilder", "Node", builder, builderIcon);
	}

	public override void _ExitTree()
	{
		selector.SelectionChanged -= OnSelectionChanged;
		if (control != null)
		{
			this.RemoveControlFromBottomPanel(control);
			control.Free();
		}
	}

	private void OnSelectionChanged()
	{
		OnSelectedNodeChanged(selector.GetSelectedNodes().FirstOrDefault());
	}

	private void OnSelectedNodeChanged(Node selectedNode)
	{
		if (selectedNode is InterierBuilder builder)
		{
			this.builder = builder;
			GD.Print($"Loading {nameof(InterierBuilderControlView)} into scene '{selectedNode.Name}'");
			if (builder.InterierMap?.Tiles is not null)
				control.SetTiles(builder.InterierMap.Tiles);
			this.AddControlToBottomPanel(control, "InterierEditor");
		}
		else if (control != null)
		{
			this.builder = null;
			this.RemoveControlFromBottomPanel(control);
		}
	}

	private void OnExport(Dictionary<Vector3I, InterierMapTile> tiles)
	{
		if (this.builder != null)
		{
			builder.UpdateTiles(tiles);
			builder.Build();
		}
	}

}
#endif
