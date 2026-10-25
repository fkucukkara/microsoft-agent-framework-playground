using AgentFrameworkCheatsheet.Configuration;
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 8: Predefined (Prompt) Agent.
/// Teaches: connecting to an agent that already exists in Microsoft Foundry (created in the portal
/// or via the Agents API) instead of defining one in code. The agent's model, instructions, and
/// hosted tools live server-side in Foundry; this app just talks to it.
/// Docs: https://learn.microsoft.com/agent-framework/integrations/by-component/agent-services/foundry
/// </summary>
internal static class Step08_PredefinedAgent
{
    public static async Task RunAsync()
    {
        #region Connect to the latest version of a Prompt Agent by name

        // Prompt Agent administration is project-scoped, unlike the account-root endpoint the
        // other steps use for direct model inference - hence the dedicated ProjectEndpoint.
        var projectClient = AgentConfig.CreateProjectClient(AgentConfig.ProjectEndpoint);

        ProjectsAgentRecord agentRecord =
            await projectClient.AgentAdministrationClient.GetAgentAsync(AgentConfig.PredefinedAgentName);

        FoundryAgent agent = projectClient.AsAIAgent(agentRecord);

        #endregion

        #region Run it like any other AIAgent

        Console.WriteLine(await agent.RunAsync("What can you help me with?"));

        #endregion

        #region Pin a specific version instead (optional)

        // Skip the lookup above and connect to an exact, known version when the app must stay
        // pinned to a specific Prompt Agent definition regardless of what's published later.
        // FoundryAgent pinnedAgent = projectClient.AsAIAgent(
        //     new AgentReference(AgentConfig.PredefinedAgentName, "1"));

        #endregion
    }
}
