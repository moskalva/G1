
using G1.Model;
using G1.Server.Agents;

namespace G1.Server;

public static class Helpers
{
    public static bool IsClientConnection(string requestPath, out string clientId)
    {
        const string prefix = "/ws/";
        const string ending = "/client";
        var indexOfEnding = requestPath.LastIndexOf(ending);
        if (requestPath.StartsWith(prefix) && indexOfEnding > prefix.Length)
        {
            clientId = requestPath.Substring(prefix.Length, indexOfEnding - prefix.Length);
            return true;
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        clientId = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
    }

    public static Vector3D ToVector(this World3dVector vector) => new Vector3D
    {
        X = vector.X,
        Y = vector.Y,
        Z = vector.Z,
    };
    public static World3dVector ToWorldVector(this Vector3D vector) => new World3dVector
    {
        X = vector.X,
        Y = vector.Y,
        Z = vector.Z,
    };

    public static ClientAgentState FromWorldState(ClientAgentState currentState, ClientStateChange stateUpdate)
    {
        if (!currentState.Id.Equals(stateUpdate.Id.Id))
            throw new InvalidOperationException("Mismatched id received");

        return new ClientAgentState
        {
            Id = currentState.Id,
            Position = new AgentPosition
            {
                SectorId = currentState.Position.SectorId,
                X = stateUpdate.PositionAndSpeed.Position.X,
                Y = stateUpdate.PositionAndSpeed.Position.Y,
                Z = stateUpdate.PositionAndSpeed.Position.Z,
            },
            Velocity = stateUpdate.PositionAndSpeed.Velocity.ToVector(),
            Rotation = stateUpdate.PositionAndSpeed.Rotation.ToVector(),
            AngularVelocity = stateUpdate.PositionAndSpeed.AngularVelocity.ToVector(),
        };
    }

    public static ServerStateChange ToWorldState(ClientAgentState agentState)
    {
        var referencePosition = WorldPositionTools.GetReferencePosition(agentState.Position);
        return new ServerStateChange
        {
            Id = new WorldEntityId { Id = agentState.Id },
            Type = WorldEntityType.Ship,
            SystemId = agentState.Position.SectorId.SystemId,
            ReferencePoint = new WorldReferencePoint
            {
                X = (int)referencePosition.X,
                Y = (int)referencePosition.Y,
                Z = (int)referencePosition.Z
            },
            PositionAndSpeed = new WorldEntityLocationAndSpeed
            {
                Position = agentState.Position.GetRelativePosition().ToWorldVector(),
                Velocity = agentState.Velocity.ToWorldVector(),
                Rotation = agentState.Rotation.ToWorldVector(),
                AngularVelocity = agentState.AngularVelocity.ToWorldVector(),
            },
        };
    }
}