

namespace G1.Server.Agents;


public class SectorAgent : Grain, ISectorAgent
{
    private readonly HashSet<Guid> agentsIds = new HashSet<Guid>();

    public Task<Guid[]> GetClients() => Task.FromResult(agentsIds.ToArray());

    public async Task EntityLeft(Guid id)
    {
        agentsIds.Remove(id);
        var tasks = from agentId in agentsIds
                    let agent = this.GrainFactory.GetGrain<IClientAgent>(agentId)
                    select agent.NeighborLeft(id);
        await Task.WhenAll(tasks);
    }

    public async Task EntityUpdated(ClientAgentState state)
    {
        var agents = (from agentId in agentsIds
                      where agentId != state.Id
                      select this.GrainFactory.GetGrain<IClientAgent>(agentId)).ToList();

        // Notify existing agents of new state                      
        await Task.WhenAll(agents.Select(agent => agent.NeighborStateChanged(state)));
        agentsIds.Add(state.Id);
    }
}
