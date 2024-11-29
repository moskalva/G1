using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class NodeExtensions
{
    public static void SetAllProcessing(this Node node, bool enabled)
    {
        node.SetProcess(enabled);
        node.SetProcessInput(enabled);
        node.SetPhysicsProcess(enabled);
    }

    public static T GetAccendant<T>(this Node node)
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

    public static T FindNode<T>(this Node parent) where T : Node
    {
        return FindNodes<T>(parent).FirstOrDefault();
    }

    public static IEnumerable<T> FindNodes<T>(this Node parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is T expected)
            {
                yield return expected;
            }
            
            foreach (var subchild in FindNodes<T>(child))
                yield return subchild;
        }
    }
}