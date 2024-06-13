

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
        var agents = (from agentId in agentsIds
                      where agentId != state.Id
                      select this.GrainFactory.GetGrain<IClientAgent>(agentId)).ToList();

        // Notify existing agents of new state                      
        await Task.WhenAll(agents.Select(agent => agent.Notify(state)));
        
        bool isNew = agentsIds.Add(state.Id);

        if (isNew)
        {
            // notify sender of all existing agents
            var newAgent = this.GrainFactory.GetGrain<IClientAgent>(state.Id);
            await Task.WhenAll(agents.Select(async agent =>
            {
                var agentState = await agent.GetState();
                await newAgent.Notify(agentState);
            }));
        }
    }
}
