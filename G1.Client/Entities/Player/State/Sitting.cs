using Godot;


public partial class Sitting : BaseState
{
	[Signal]
	public delegate void LeaveEventHandler();
    public Transform3D CharacterPosition { get; set; }


    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("LeaveControlPlace"))
        {
            EmitSignal(SignalName.Leave);
        }
    }


    public override Transform3D GetInitialCameraTransform()
    {
        return CharacterPosition.TranslatedLocal(new Vector3(0, 1f, 0));
    }
}