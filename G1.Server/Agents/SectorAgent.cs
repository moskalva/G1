

namespace G1.Server.Agents;


public class SectorAgent : Grain, ISectorAgent
{
    private readonly HashSet<Guid> agentsIds = new HashSet<Guid>();

    public async Task Leave(Guid id)
    {
        agentsIds.Remove(id);
        var tasks = from agentId in agentsIds
                    let agent = this.GrainFactory.GetGrain<IClientAgent>(agentId)
                    select agent.Leave(id);
        await Task.WhenAll(tasks);
    }

    public async Task Notify(ClientAgentState state)
    {
        agentsIds.Add(state.Id);
        var tasks = from agentId in agentsIds
                    where agentId != state.Id
                    let agent = this.GrainFactory.GetGrain<IClientAgent>(agentId)
                    select agent.Notify(state);
        await Task.WhenAll(tasks);
    }
}
