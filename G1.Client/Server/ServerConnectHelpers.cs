using Godot;
using G1.Model;

public static class ServerConnectHelpers
{
	public static ShipState ExtractState(this Exterier player)
	{
		return new ShipState
		{
			Id = player.Id,
			Type = WorldEntityType.Ship,
			Position = player.Position,
			Velocity = player.Velocity,
		};
	}

	public static ShipState ToShipState(this WorldEntityState state)
	{
		var result = new ShipState
		{
			Id = state.Id,
			Type = state.Type,
		};
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
			Velocity= state.Velocity.ToWorldVector(),
		};
	}

	public static Vector3 ToVector(this World3dVector vector)
	{
		return new Vector3
		{
			X = vector.X,
			Y = vector.Y,
			Z = vector.Z,
		};
	}

	public static World3dVector ToWorldVector(this Vector3 vector)
	{
		return new World3dVector
		{
			X = vector.X,
			Y = vector.Y,
			Z = vector.Z,
		};
	}
}
