using G1.Model;

namespace G1.Server;



public class ClientAgentProvider : IClientAgentProvider
{
    public IClientAgent GetAgent(WorldEntityId cientId)
    {
        return new ClientAgent(cientId);
    }
}