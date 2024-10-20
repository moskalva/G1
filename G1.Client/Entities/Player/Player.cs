using Godot;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata;


public partial class Player : Node3D
{

	[Export]
	public Vector3 FocusPoint3P { get; set; } = new Vector3(0.8f, 1.8f, 0f);
	[Export]
	public Vector3 FocusPoint1P { get; set; } = new Vector3(0f, 1.7f, -0.1f);

	[Export]
	public float MinCameraDistance = 2f;
	[Export]
	public float MaxCameraDistance = 8f;

	public PointOfView PointOfView;
	public RayCast3D AimSensor;
	public Character Character;
	private Vector3 focusPointShift;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		this.AimSensor = GetNode<RayCast3D>("AimSensor");
		this.PointOfView = GetNode<PointOfView>("PointOfView");
		this.Character = GetNode<Character>("Character");

		this.AimSensor.AddException(Character);
		PointOfView.ExcludeClipObject(Character);
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateFocusPoint();
	}
	private void UpdateFocusPoint()
	{
		var focusPoint = Character.Transform.TranslatedLocal(this.focusPointShift);
		this.PointOfView.Transform = focusPoint;
		Character.SetHeadDirection(this.PointOfView.CameraDirection);
		AimSensor.Transform = this.PointOfView.Transform.LookTowards(this.PointOfView.Basis * this.PointOfView.CameraDirection);
	}

	public void SwitchCameraShiftSide()
	{
		var shiftX = -1 * this.focusPointShift.X * 2;
		this.focusPointShift = new Vector3(this.focusPointShift.X + shiftX, this.focusPointShift.Y, this.focusPointShift.Z); ;
	}

	public void SetViewType(ViewType cameraType)
	{
		if (cameraType == ViewType.ThirdPerson)
		{
			this.focusPointShift = FocusPoint3P;
			this.PointOfView.MinCameraDistance = this.MinCameraDistance;
			this.PointOfView.MaxCameraDistance = this.MaxCameraDistance;
			this.PointOfView.Reset();
		}
		else if (cameraType == ViewType.FirstPerson)
		{
			this.focusPointShift = FocusPoint1P;
			this.PointOfView.MinCameraDistance = 0;
			this.PointOfView.MaxCameraDistance = 0;
			this.PointOfView.Reset();
		}
		else { throw new NotSupportedException($"Unknown cameraType '{cameraType}'"); }
	}
}
