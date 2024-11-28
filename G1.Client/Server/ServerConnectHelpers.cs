using Godot;
using G1.Model;

public static class ServerConnectHelpers
{

	public static ShipState ToShipState(this WorldEntityState state)
	{
		var result = new ShipState
		{
			Id = state.Id,
			Type = state.Type,
		};
		if (state.SystemId.HasValue)
		{
			result.SystemId = state.SystemId.Value;
		}
		if (state.ReferencePoint.HasValue)
		{
			result.ReferencePoint = ToVector(state.ReferencePoint.Value);
		}
		if (state.Position.HasValue)
		{
			result.Position = ToVector(state.Position.Value);
		}
		if (state.Velocity.HasValue)
		{
			result.Velocity = ToVector(state.Velocity.Value);
		}
		return result;
	}

	public static WorldEntityState ToWorldState(this ShipState state)
	{
		return new WorldEntityState
		{
			Id = state.Id,
			Type = state.Type,
			Position = state.Position.ToWorldVector(),
			Velocity = state.Velocity.ToWorldVector(),
		};
	}

	public static Vector3 ToVector(this World3dVector vector)
	{
		return new Vector3
		{
			X = vector.X,
			Y = vector.Z,
			Z = vector.Y,
		};
	}

	public static Vector3I ToVector(this WorldReferencePoint vector)
	{
		return new Vector3I
		{
			X = vector.X,
			Y = vector.Z,
			Z = vector.Y,
		};
	}

	public static World3dVector ToWorldVector(this Vector3 vector)
	{
		return new World3dVector
		{
			X = vector.X,
			Y = vector.Z,
			Z = vector.Y,
		};
	}
}
