
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



    public static ClientAgentState FromWorldState(WorldSectorId sectorId, WorldEntityState state)
    {
        return new ClientAgentState
        {
            Id = state.Id.Id,
            Position = new AgentPosition
            {
                SectorId = sectorId,
                X = state.Position?.X ?? 0,
                Y = state.Position?.Y ?? 0,
                Z = state.Position?.Z ?? 0,
            },
            Velocity = new Vector3D
            {
                X = state.Velocity?.X ?? 0,
                Y = state.Velocity?.Y ?? 0,
                Z = state.Velocity?.Z ?? 0,
            },
            Rotation = new Vector3D
            {
                X = state.Rotation?.X ?? 0,
                Y = state.Rotation?.Y ?? 0,
                Z = state.Rotation?.Z ?? 0,
            },
            AngularVelocity = new Vector3D
            {
                X = state.AngularVelocity?.X ?? 0,
                Y = state.AngularVelocity?.Y ?? 0,
                Z = state.AngularVelocity?.Z ?? 0,
            },
        };
    }

    public static WorldEntityState ToWorldState(ClientAgentState agentState)
    {
        var referencePosition = WorldPositionTools.GetReferencePosition(agentState.Position);
        return new WorldEntityState
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
            Position = new World3dVector
            {
                X = agentState.Position.X,
                Y = agentState.Position.Y,
                Z = agentState.Position.Z
            },
            Velocity = new World3dVector
            {
                X = agentState.Velocity.X,
                Y = agentState.Velocity.Y,
                Z = agentState.Velocity.Z,
            },
            Rotation = new World3dVector
            {
                X = agentState.Rotation.X,
                Y = agentState.Rotation.Y,
                Z = agentState.Rotation.Z,
            },
            AngularVelocity = new World3dVector
            {
                X = agentState.AngularVelocity.X,
                Y = agentState.AngularVelocity.Y,
                Z = agentState.AngularVelocity.Z,
            },
        };
    }
}