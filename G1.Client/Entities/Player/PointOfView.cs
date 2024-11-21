using Godot;
using System;

public partial class PointOfView : Node3D
{
	private Camera3D camera;
	private RayCast3D cameraClipSensor;
	private float cameraDistance;
	private Vector2 cameraRotation;

	public float MinCameraDistance = 2f;
	public float MaxCameraDistance = 8f;
	[Export]
	public float CameraDistanceStep = 0.5f;
	[Export]
	public float CameraDistanceSpeed = 20f;
	[Export]
	public float CameraMinDistanceToWalls = 0.1f;

	[Export]
	public float MaxCameraRotationX = Mathf.Pi / 2 - 0.3f;
	[Export]
	public float MaxCameraRotationY = Mathf.Pi / 2 - 0.6f;

	public override void _Ready()
	{
		this.camera = GetNode<Camera3D>("Camera");
		this.cameraClipSensor = GetNode<RayCast3D>("CameraClipSensor");
		Reset();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		NormalizeCameraDistance();
		UpdateCameraPosition(delta);
	}

	public void RotateCamera(Vector2 angle)
	{
		var x = this.cameraRotation.X + angle.X;
		if (x < -MaxCameraRotationX)
			x = -MaxCameraRotationX;
		if (x > MaxCameraRotationX)
			x = MaxCameraRotationX;

		var y = this.cameraRotation.Y + angle.Y;
		if (y < -MaxCameraRotationY)
			y = -MaxCameraRotationY;
		if (y > MaxCameraRotationY)
			y = MaxCameraRotationY;

		this.cameraRotation = new Vector2(x, y);
	}

	public void IncreaseCameraDistance()
	{
		this.cameraDistance += CameraDistanceStep;
	}

	public void DecreaseCameraDistance()
	{
		this.cameraDistance -= CameraDistanceStep;
	}

	public void Reset()
	{
		cameraDistance = MinCameraDistance;
		cameraRotation = Vector2.Zero;
	}

	public void ExcludeClipObject(CollisionObject3D obj)
	{
		this.cameraClipSensor.AddException(obj);
	}

	public Vector3 CameraDirection => this.camera.Transform.Basis.Z * -1;

	public bool IsCameraIdle()
	{
		var currentDistance = this.camera.Transform.Origin.DistanceTo(Vector3.Zero);
		return Mathf.IsEqualApprox(currentDistance, cameraDistance);
	}

	private void UpdateCameraPosition(double delta)
	{
		var currentDistance = this.camera.Transform.Origin.DistanceTo(Vector3.Zero);

		var pointOfView = Transform3D.Identity
			.RotatedLocal(Vector3.Down, this.cameraRotation.X)
			.RotatedLocal(Vector3.Left, this.cameraRotation.Y);

		var expectedDistance = GetExpectedCameraDistance(pointOfView);

		var distance = Mathf.MoveToward(currentDistance, expectedDistance, (float)(CameraDistanceSpeed * delta));
		this.camera.Transform = pointOfView
			.TranslatedLocal(new Vector3(0, 0, distance));
	}

	private float GetExpectedCameraDistance(Transform3D pointOfView)
	{
		var sensor = this.cameraClipSensor;

		// start with distance set by player
		var expectedDistance = this.cameraDistance;

		// check if something in between camera and point of view
		sensor.Transform = pointOfView;
		sensor.TargetPosition = new Vector3(0, 0, expectedDistance);
		sensor.ForceRaycastUpdate();

		if (sensor.IsColliding())
		{
			var collisonPoint = sensor.GetCollisionPoint();
			expectedDistance = collisonPoint.DistanceTo(sensor.GlobalPosition);
		}

		// check if camera is too close to walls 
		while (IsColliding(new Vector3(0, CameraMinDistanceToWalls, expectedDistance)) ||
			   IsColliding(new Vector3(0, -CameraMinDistanceToWalls, expectedDistance)) ||
			   IsColliding(new Vector3(CameraMinDistanceToWalls, 0, expectedDistance)) ||
			   IsColliding(new Vector3(-CameraMinDistanceToWalls, 0, expectedDistance)))
		{
			if (expectedDistance <= 0)
				break;
			expectedDistance -= 0.01f;
		}

		return expectedDistance;

		bool IsColliding(Vector3 target)
		{
			sensor.TargetPosition = target;
			sensor.ForceRaycastUpdate();
			return sensor.IsColliding();
		}
	}

	private void NormalizeCameraDistance()
	{
		if (this.cameraDistance > MaxCameraDistance)
			this.cameraDistance = MaxCameraDistance;
		if (this.cameraDistance < MinCameraDistance)
			this.cameraDistance = MinCameraDistance;
	}
}
