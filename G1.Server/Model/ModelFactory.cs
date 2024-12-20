
using G1.Server.Agents;

namespace G1.Server.Models;
public interface IPlayerShip
{
    bool CanSee(ClientAgentState target);
}

public static class ModelFactory
{
    public static IPlayerShip GetShipModel(ClientAgentState agentState, PalyerShipType type) => type.ShipType switch
    {
        ShipType.Mark1 => new Mark1Ship(agentState),
        _ => throw new NotSupportedException($"Ship type '{type.ShipType}' is not supported")
    };
}