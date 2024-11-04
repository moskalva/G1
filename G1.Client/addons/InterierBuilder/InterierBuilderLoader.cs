#if TOOLS
using System;
using System.Linq;
using Godot;

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
	}

	private void OnSelectionChanged()
	{
		OnSelectedNodeChanged(selector.GetSelectedNodes().FirstOrDefault());
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

	private void OnSelectedNodeChanged(Node selectedNode)
	{
		if (selectedNode is InterierBuilder builder)
		{
			this.builder = builder;
			GD.Print($"Loading {nameof(InterierBuilderControlView)} into scene '{selectedNode.Name}'");
			this.AddControlToBottomPanel(control, "InterierEditor");
		}
		else if (control != null)
		{
			this.builder = null;
			this.RemoveControlFromBottomPanel(control);
		}
	}

	private void OnExport()
	{
		if (this.builder != null)
		{
			builder.Build();
		}
	}

}
#endif
