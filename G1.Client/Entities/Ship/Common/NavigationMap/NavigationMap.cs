using Godot;
using System;

public partial class NavigationMap : Node
{
	[Export]
	public float CameraMovementSpeed { get; set; } = 0.1f;
	[Export]
	public float CameraZoomStep { get; set; } = 100;
	[Export]
	public float MinCameraZoom { get; set; } = 10;

	public SubViewport View { get; private set; }

	private Node3D center;
	private Camera3D camera;
	private Vector2 cameraAngle;
	private float cameraDistance;
	private BaseShip ship;

	public override void _EnterTree()
	{
		ship = ShipSystems.Register(this);
		this.View = GetNode<SubViewport>("SubViewport");
		this.center = View.GetNode<Node3D>("Center");
		this.camera = center.GetNode<Camera3D>("Camera3D");
	}

	public override void _Ready()
	{
		cameraDistance = MinCameraZoom;
		cameraAngle = new Vector2(0, Mathf.Pi / 4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveCenter();
		MoveCamera();
	}

	public void MoveCamera(Vector2 direction)
	{
		this.cameraAngle += direction * this.CameraMovementSpeed;
	}

	public void ZoomCamera(bool closer)
	{
		this.cameraDistance += closer ? -this.CameraZoomStep : this.CameraZoomStep;
		if (this.cameraDistance < this.MinCameraZoom)
			this.cameraDistance = this.MinCameraZoom;
	}

	private void MoveCamera()
	{
		this.camera.Transform = Transform3D.Identity
			.RotatedLocal(Vector3.Up, cameraAngle.X)
			.RotatedLocal(Vector3.Left, cameraAngle.Y)
			.TranslatedLocal(Vector3.Back * this.cameraDistance);
	}

	private void MoveCenter()
	{
		center.Transform = center.Transform.Teleported(this.ship.Exterier.Position);
	}
}
