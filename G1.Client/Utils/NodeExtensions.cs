using Godot;

public static class NodeExtensions
{
    public static void SetAllProcessing(this Node node, bool enabled)
    {
        node.SetProcess(enabled);
        node.SetProcessInput(enabled);
        node.SetPhysicsProcess(enabled);
    }
}