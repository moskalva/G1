using Godot;
using G1.Model;

public static class ServerConnectHelpers
{

	public static ShipState ToShipState(this ServerStateChange state)
		=> new ShipState
		{
			Id = state.Id,
			Type = state.Type,
			SystemId = state.SystemId,
			ReferencePoint = ToVector(state.ReferencePoint),
			Position = ToVector(state.PositionAndSpeed.Position),
			Velocity = ToVector(state.PositionAndSpeed.Velocity),
			Rotation = ToVector(state.PositionAndSpeed.Rotation),
			AngularVelocity = ToVector(state.PositionAndSpeed.AngularVelocity),
			ThermalEmission = state.Emissions.ThermalEmission,
			EmEmission = state.Emissions.EmEmission,
			ParticleEmission = state.Emissions.ParticleEmission,
		};

	public static ClientStateChange ToWorldState(this ShipState state)
		=> new ClientStateChange
		{
			Id = state.Id,
			PositionAndSpeed = new WorldEntityLocationAndSpeed
			{
				Position = state.Position.ToWorldVector(),
				Velocity = state.Velocity.ToWorldVector(),
				Rotation = state.Rotation.ToWorldVector(),
				AngularVelocity = state.AngularVelocity.ToWorldVector(),
			},
			Emissions = new Emissions
			{
				ThermalEmission = state.ThermalEmission,
				EmEmission = state.EmEmission,
				ParticleEmission = state.ParticleEmission,
			},
		};

	public static Vector3 ToVector(this World3dVector vector) => new Vector3
	{
		X = vector.X,
		Y = vector.Z,
		Z = vector.Y,
	};

	public static Vector3I ToVector(this WorldReferencePoint vector) => new Vector3I
	{
		X = vector.X,
		Y = vector.Z,
		Z = vector.Y,
	};

	public static World3dVector ToWorldVector(this Vector3 vector) => new World3dVector
	{
		X = vector.X,
		Y = vector.Z,
		Z = vector.Y,
	};
}
