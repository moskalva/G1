using G1.Model;
using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public static readonly string GlobalIdString = "2b8786fa-7915-4fc9-9237-cf3dea9810a2";
	public WorldEntityId Id = new WorldEntityId() { Id = new Guid(GlobalIdString) };

	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	[Export]
	public Timer SyncTimer;

	[Signal]
	public delegate void PlayerStateChangedEventHandler(CharacterState state);

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		SyncTimer.Timeout += () => EmitSignal(SignalName.PlayerStateChanged, this.ExtractState());

		// wait for initial data from server 
		SetProcess(false);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void _OnDataReceived(CharacterState remoteState)
	{
		if (!remoteState.Id.Equals(this.Id))
			return;
			
		GD.Print($"Server state update: '{remoteState}'");
		this.Position = remoteState.Position;
		this.Velocity = remoteState.Velocity;
		this.SetProcess(true);
		if (SyncTimer.IsStopped())
		{
			GD.Print("Starting sync");
			SyncTimer.Start();
		}
	}
}
