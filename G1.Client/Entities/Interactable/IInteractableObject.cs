using Godot;

public interface IInteractableObject
{
    public void Highlite();

    public void Interact();
}

public static class InteractableObjectExtensions
{
    public static bool TryGetInteractable(this GodotObject @object, out IInteractableObject interactable)
    {
        while (@object != null)
        {
            if (@object is IInteractableObject found)
            {
                interactable = found;
                return true;
            }
            else if (@object is Node node)
            {
                @object = node.GetParent();
            }
        }

        interactable = null;
        return false;
    }
}