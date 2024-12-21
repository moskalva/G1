
using G1.Server.Agents;

namespace G1.Server.Models;

public class Mark1Ship : IPlayerShip
{
    private ClientAgentState myState;

    public Mark1Ship(ClientAgentState agentState)
    {
        this.myState = agentState;
    }

    public bool CanSee(ClientAgentState target)
    {
        var distance = WorldPositionTools.GetDistance(myState.Position, target.Position);
        if (distance < G1.WorldParameters.Physics.FogOfWarDistance)
            return true;

        var thermalVisibilityDistance = GetThermalVisibilityDistance(target.ThermalEmission);
        if (distance < thermalVisibilityDistance)
            return true;

        return false;
    }

    private static float GetThermalVisibilityDistance(float thermalEmmission)
        => thermalEmmission switch
        {// todo build a chart here and use formula to calculate
            > 10_000 => 100_000,
            > 5_000 => 50_000,
            > 1_000 => 10_000,
            > 300 => 2_000,
            _ => 0
        };

}