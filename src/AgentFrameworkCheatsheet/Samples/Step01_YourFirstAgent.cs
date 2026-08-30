using AgentFrameworkCheatsheet.Configuration;
using Azure.AI.Projects;
using Microsoft.Agents.AI;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 1: Your First Agent.
/// Teaches: creating an agent and invoking it, both non-streaming and streaming.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/your-first-agent
/// </summary>
internal static class Step01_YourFirstAgent
{
    public static async Task RunAsync()
    {
        #region Create the agent

        AIAgent agent = AgentConfig.CreateProjectClient()
            .AsAIAgent(
                model: AgentConfig.DeploymentName,
                instructions: "You are a friendly assistant. Keep your answers brief.",
                name: "HelloAgent");

        #endregion

        #region Run (non-streaming)

        Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));

        #endregion

        #region Run (streaming)

        Console.Write("Agent (streaming): ");
        await foreach (var update in agent.RunStreamingAsync("Tell me a one-sentence fun fact."))
        {
            Console.Write(update);
        }
        Console.WriteLine();

        #endregion
    }
}
