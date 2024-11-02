#if TOOLS
using Godot;

[Tool]
public partial class InterierBuilderLoader : EditorPlugin
{
	private Control control;


	public override void _EnterTree()
	{
		var controlScene = GD.Load<PackedScene>("res://addons/InterierBuilder/InterierBuilderControlView.tscn");
		control = controlScene.Instantiate<Control>();
		this.SceneChanged += OnSceneChanged;
	}

	public override void _ExitTree()
	{
		this.SceneChanged += OnSceneChanged;
		if (control != null)
		{
			this.RemoveControlFromBottomPanel(control);
			control.Free();
		}
	}

	private void OnSceneChanged(Node scene)
	{
		GD.Print($"Loading {nameof(InterierBuilderControlView)} into scene '{scene.Name}'");
		if (scene.Name == "Hull")
		{
			this.AddControlToBottomPanel(control, "InterierEditor");
		}
		else if (control != null)
		{
			this.RemoveControlFromBottomPanel(control);
		}
	}

	public override bool _Handles(GodotObject @object)
	{
		GD.Print($"Hndle input '{@object}'. Result: '{@object == control}'");
		return @object == control;
	}

}
#endif
