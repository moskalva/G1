using G1.Model;

namespace G1.Server;

public interface IClientAgentProvider
{
    IClientAgent GetAgent(WorldEntityId cientId);
}