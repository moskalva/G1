using Godot;
using System;
using G1;

public partial class EmissionsController : Node
{
	[Export]
	public float MinimalThermalEmission { get; set; } = WorldParameters.Physics.MinimalThermalEmission;
	[Export]
	public float ThermalEmissionCoefficient { get; set; } = WorldParameters.Physics.ThermalEmissionCoefficient;
	[Export]
	public float ThermalEmissionDumpSpeed { get; set; } = WorldParameters.Physics.ThermalEmissionDumpSpeed;

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
