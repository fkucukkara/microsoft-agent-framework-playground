using AgentFrameworkCheatsheet.Configuration;
using Azure.AI.Projects;
using Microsoft.Agents.AI;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 3: Multi-Turn Conversations.
/// Teaches: using an AgentSession so the agent remembers earlier turns in the same conversation.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/multi-turn
/// </summary>
internal static class Step03_MultiTurnConversations
{
    public static async Task RunAsync()
    {
        AIAgent agent = AgentConfig.CreateProjectClient()
            .AsAIAgent(
                model: AgentConfig.DeploymentName,
                instructions: "You are a friendly assistant. Keep your answers brief.",
                name: "ConversationAgent");

        #region Create a session to maintain conversation history

        AgentSession session = await agent.CreateSessionAsync();

        #endregion

        #region First turn

        Console.WriteLine(await agent.RunAsync("My name is Alice and I love hiking.", session));

        #endregion

        #region Second turn - the agent remembers the user's name and hobby

        Console.WriteLine(await agent.RunAsync("What do you remember about me?", session));

        #endregion
    }
}
