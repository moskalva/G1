using Godot;

public abstract partial class ShipManagement : Node
{

    public SubViewport Viewport { get; protected set; }

    public override void _Ready()
    {
        this.SetProcessInput(false);
    }
}
