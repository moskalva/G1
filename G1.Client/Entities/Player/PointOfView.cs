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
	public float CameraDistanceSpeed = 10f;

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

		var expectedDistance = this.cameraDistance;
		this.cameraClipSensor.Transform = Transform3D.Identity
			.RotatedLocal(Vector3.Down, this.cameraRotation.X)
			.RotatedLocal(Vector3.Left, this.cameraRotation.Y);
		this.cameraClipSensor.TargetPosition = new Vector3(0, 0, expectedDistance);

		if (this.cameraClipSensor.IsColliding())
		{
			var collisonPoint = cameraClipSensor.GetCollisionPoint();
			expectedDistance = collisonPoint.DistanceTo(cameraClipSensor.GlobalPosition) + 0.1f;
		}

		var distance = Mathf.MoveToward(currentDistance, expectedDistance, (float)(CameraDistanceSpeed * delta));
		this.camera.Transform = this.cameraClipSensor.Transform
			.TranslatedLocal(new Vector3(0, 0, distance));
		this.cameraDistance = expectedDistance;
	}

	private void NormalizeCameraDistance()
	{
		if (this.cameraDistance > MaxCameraDistance)
			this.cameraDistance = MaxCameraDistance;
		if (this.cameraDistance < MinCameraDistance)
			this.cameraDistance = MinCameraDistance;
	}
}
