using Godot;

public abstract partial class BaseDoor : Node3D
{
    private bool isAnimating = false;
    public bool IsOpened { get; private set; } = false;
    protected abstract void StartClosing();
    protected abstract void StartOpening();
    protected void SetState(bool isOpened)
    {
        isAnimating = false;
        IsOpened = isOpened;
    }

    public void Open()
    {
        if (!isAnimating && !IsOpened)
        {
            isAnimating = true;
            StartOpening();
        }
    }

    public void Close()
    {
        if (!isAnimating && IsOpened)
        {
            isAnimating = true;
            StartClosing();
        }
    }
}