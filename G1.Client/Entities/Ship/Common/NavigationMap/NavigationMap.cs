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

	public SubViewport View {get;private set;}

	private Camera3D camera;
	private Vector2 cameraAngle;
	private float cameraDistance;
	private BaseShip ship;

	public override void _EnterTree()
	{
		ship = ShipSystems.Register(this);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.View = GetNode<SubViewport>("SubViewport");
		this.camera = GetNode<Camera3D>("SubViewport/Camera3D");
		cameraDistance = MinCameraZoom;
		cameraAngle = new Vector2(0, Mathf.Pi / 4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
			.Teleported(this.ship.Exterier.Position)
			.RotatedLocal(Vector3.Up, cameraAngle.X)
			.RotatedLocal(Vector3.Left, cameraAngle.Y)
			.TranslatedLocal(Vector3.Back * this.cameraDistance);
	}
}
