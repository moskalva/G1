

namespace G1.Server.Agents;


public interface ISectorAgent : IGrainWithGuidKey
{
    Task EntityUpdated(ClientAgentState state);
    Task EntityLeft(Guid clientId);
    Task<Guid[]> GetClients();
}