using Godot;

public enum DoorState { Open, Closed, Opeining, Closing }
public abstract partial class BaseDoor : Node3D
{
    [Signal]
    public delegate void DoorStateChangedEventHandler(DoorState state);

    public DoorState DoorState => isAnimating
        ? IsOpened ? DoorState.Closing : DoorState.Opeining
        : IsOpened ? DoorState.Open : DoorState.Closed;

    private bool isAnimating = false;
    public bool IsOpened { get; private set; } = false;
    protected abstract void StartClosing();
    protected abstract void StartOpening();
    protected void SetState(bool isOpened)
    {
        isAnimating = false;
        IsOpened = isOpened;
        EmitSignal(SignalName.DoorStateChanged, (int)this.DoorState);
    }

    public void Open()
    {
        if (!isAnimating && !IsOpened)
        {
            isAnimating = true;
            StartOpening();
            EmitSignal(SignalName.DoorStateChanged, (int)this.DoorState);
        }
    }

    public void Close()
    {
        if (!isAnimating && IsOpened)
        {
            isAnimating = true;
            StartClosing();
            EmitSignal(SignalName.DoorStateChanged, (int)this.DoorState);
        }
    }
}