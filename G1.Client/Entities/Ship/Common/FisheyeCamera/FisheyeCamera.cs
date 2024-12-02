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
		this.spots = this.Exterier.FisheyeSpots.Select(s => new SpotCameraState(s)).ToArray();
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
		if (spot.ControlType == FisheyeSpot.FisheyeCameraType.Horizontal)
		{
			if (spot.direction.Y > Mathf.Pi / 2)
				spot.direction.Y = Mathf.Pi / 2;
			else if (spot.direction.Y < 0)
				spot.direction.Y = 0;
		}
		else if (spot.ControlType == FisheyeSpot.FisheyeCameraType.Vertical)
		{
			if (spot.direction.X > Mathf.Pi / 2)
				spot.direction.X = Mathf.Pi / 2;
			else if (spot.direction.X < -Mathf.Pi / 2)
				spot.direction.X = -Mathf.Pi / 2;

			if (spot.direction.Y > Mathf.Pi / 2)
				spot.direction.Y = Mathf.Pi / 2;
			else if (spot.direction.Y < -Mathf.Pi / 2)
				spot.direction.Y = -Mathf.Pi / 2;
		}
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
		var transform = spot.SpotLocation;
		var rotated = spot.ControlType == FisheyeSpot.FisheyeCameraType.Horizontal
					? transform.RotatedLocal(Vector3.Left, spot.direction.Y).Rotated(Vector3.Down, spot.direction.X)
					: transform.RotatedLocal(Vector3.Down, spot.direction.X).RotatedLocal(Vector3.Left, spot.direction.Y);
		var zoomed = rotated
			.TranslatedLocal(Vector3.Forward * spot.Zoom);
		this.camera.Transform = zoomed;
	}

	private class SpotCameraState
	{
		public SpotCameraState(FisheyeSpot spot)
		{
			Name = spot.Name;
			SpotLocation = spot.Transform;
			ControlType = spot.ControlType;
		}

		public string Name { get; }
		public FisheyeSpot.FisheyeCameraType ControlType { get; }
		public Transform3D SpotLocation { get; }
		public float Zoom = 0;
		public Vector2 direction = Vector2.Zero;
	}
}
