using System;
using Godot;

public static class NodeExtensions
{
    public static void SetAllProcessing(this Node node, bool enabled)
    {
        node.SetProcess(enabled);
        node.SetProcessInput(enabled);
        node.SetPhysicsProcess(enabled);
    }

    public static T GetAccendant<T>(this Node node) where T : Node
    {
        do
        {
            var candidate = node.GetParent();
            if (candidate is T found)
            {
                return found;
            }
            node = candidate;
        } while (node != null);

        throw new InvalidOperationException($"Couldnot find parent node of type '{typeof(T).FullName}'");
    }

    public static T FindNode<T>(this Node parent, string name = null) where T : Node
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is T expected)
            {
                if (name == null || expected.Name == name)
                {
                    return expected;
                }
            }
            var subchild = FindNode<T>(child, name);
            if (subchild != null)
                return subchild;
        }
        return null;
    }
}