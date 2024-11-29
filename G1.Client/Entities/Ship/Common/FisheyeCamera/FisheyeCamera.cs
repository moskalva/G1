using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FisheyeCamera : Node
{
	[Export]
	public Exterier Exterier { get; set; }
	[Export]
	public float CameraMoveSpeed { get; set; } = 0.01f;
	[Export]
	public float CameraZoomSpeed { get; set; } = 0.1f;
	[Export]
	public float MinCameraZoom { get; set; } = 0.1f;
	private Camera3D camera;
	private SpotCameraState[] spots;
	private int currentSpot = 0;

	public override void _EnterTree()
	{
		ShipSystems.Register(this);
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.camera = GetNode<Camera3D>("Camera3D");
		this.spots = this.Exterier.FisheyeSpots.Select(s => new SpotCameraState(s.Name, s.Transform)).ToArray();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveCamera();
	}

	public void SwitchCamera()
	{
		currentSpot += 1;
		if (currentSpot >= spots.Length)
			currentSpot = 0;
		GD.Print($"Current camera name is '{this.spots[currentSpot].Name}'");
	}

	public void MoveCamera(Vector2 direction)
	{
		var spot = spots[currentSpot];
		spot.direction += direction * CameraMoveSpeed;
		if (spot.direction.Y > Mathf.Pi / 2)
			spot.direction.Y = Mathf.Pi / 2;
		else if (spot.direction.Y < -Mathf.Pi / 2)
			spot.direction.Y = -Mathf.Pi / 2;
	}

	public void Zoom(bool closer)
	{
		var spot = spots[currentSpot];
		spot.Zoom += closer ? -CameraZoomSpeed : CameraZoomSpeed;
		if (spot.Zoom < MinCameraZoom)
			spot.Zoom = MinCameraZoom;
	}
	private void MoveCamera()
	{
		var spot = spots[currentSpot];
		var transform = spot.SpotLocation
			.RotatedLocal(Vector3.Left, spot.direction.Y)
			.RotatedLocal(Vector3.Down, spot.direction.X)
			.TranslatedLocal(Vector3.Forward * spot.Zoom);
		this.camera.Transform = transform;
	}

	private class SpotCameraState
	{
		public SpotCameraState(string name, Transform3D location)
		{
			Name = name;
			SpotLocation = location;
		}

		public string Name { get; }
		public Transform3D SpotLocation { get; }
		public float Zoom = 0;
		public Vector2 direction = Vector2.Zero;
	}
}
