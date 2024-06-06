using G1.Model;

namespace G1.Server;



public class ClientAgentProvider : IClientAgentProvider
{
    private readonly IGrainFactory grains;

    public ClientAgentProvider(IGrainFactory grains)
    {
        this.grains = grains;
    }

    public IClientAgent GetAgent(WorldEntityId cientId)
    {   
        return grains.GetGrain<IClientAgent>(cientId.Id);
    }
}