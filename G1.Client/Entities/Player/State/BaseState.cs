using Godot;

public abstract partial class BaseState : Node
{
	public Player Player { get; set; }

	public PointOfView PointOfView => this.Player.PointOfView;
	public Character Character => this.Player.Character;
	public RayCast3D AimSensor => this.Player.AimSensor;


	public abstract PlayerStateProperties InitialState { get; }
}
