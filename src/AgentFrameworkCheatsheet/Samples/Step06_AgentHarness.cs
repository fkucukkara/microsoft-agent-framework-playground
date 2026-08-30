using AgentFrameworkCheatsheet.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 6: Agent Harness.
/// Teaches: wrapping a raw IChatClient in a harness that adds planning, todo tracking, and
/// history persistence for long, multi-step tasks - out of the box, no manual scaffolding.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/harness
/// </summary>
internal static class Step06_AgentHarness
{
    public static async Task RunAsync()
    {
        #region Create the harness agent

        IChatClient chatClient = AgentConfig.CreateChatClient();
        AIAgent agent = chatClient.AsHarnessAgent();

        #endregion

        #region Conversation loop

        // A session carries the harness state (plan, todos, history) across turns.
        AgentSession session = await agent.CreateSessionAsync();

        Console.WriteLine("Harness agent ready. Type 'exit' to quit.");
        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // Stream this turn's output as the harness plans and works through the request.
            await foreach (var update in agent.RunStreamingAsync(input, session))
            {
                Console.Write(update);
            }

            Console.WriteLine();
        }

        #endregion
    }
}
