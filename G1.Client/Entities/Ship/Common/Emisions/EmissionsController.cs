using Godot;
using System;

public partial class EmissionsController : Node
{
	[Export]
	public float MinimalThermalEmission { get; set; } = 300f;
	[Export]
	public float ThermalEmissionCoefficient { get; set; } = 0.0001f;
	[Export]
	public float ThermalEmissionDumpSpeed { get; set; } = 50f;

	public float ThermalEmission { get; private set; }
	public float EmEmission { get; private set; }
	public float ParticleEmission { get; private set; }


	public override void _EnterTree()
	{
		ShipSystems.Register(this);
	}

	public override void _Ready()
	{
		ThermalEmission = MinimalThermalEmission;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		this.ThermalEmission -= (float)(ThermalEmissionDumpSpeed * delta);
		if (this.ThermalEmission < MinimalThermalEmission)
			this.ThermalEmission = MinimalThermalEmission;
	}

	public void OnThrust(Vector3 _, float force)
	{
		this.ThermalEmission += force * ThermalEmissionCoefficient;
	}
}
