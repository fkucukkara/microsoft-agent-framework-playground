using AgentFrameworkCheatsheet.Configuration;
using Azure.AI.Projects;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 4: Memory &amp; Persistence.
/// Teaches: how chat history is stored by default, and how to plug in a custom ChatHistoryProvider.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/memory
/// </summary>
internal static class Step04_MemoryAndPersistence
{
    #region Custom ChatHistoryProvider

    // Minimal illustrative provider: keeps its own in-memory log independent of AgentSession,
    // useful when history needs to be mirrored to an external store (DB, cache, audit log, etc.).
    private sealed class CustomChatHistoryProvider : ChatHistoryProvider
    {
        private readonly List<ChatMessage> _history = [];

        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
            InvokingContext context, CancellationToken cancellationToken = default)
            => ValueTask.FromResult<IEnumerable<ChatMessage>>(_history);

        protected override ValueTask StoreChatHistoryAsync(
            InvokedContext context, CancellationToken cancellationToken = default)
        {
            _history.AddRange(context.RequestMessages);
            _history.AddRange(context.ResponseMessages);
            return ValueTask.CompletedTask;
        }
    }

    #endregion

    public static async Task RunAsync()
    {
        #region Default: InMemoryChatHistoryProvider

        // Chat-completion-style services don't store history server-side, so the agent
        // automatically falls back to an InMemoryChatHistoryProvider under the hood.
        AIAgent defaultAgent = AgentConfig.CreateProjectClient()
            .AsAIAgent(
                model: AgentConfig.DeploymentName,
                instructions: "You are a friendly assistant. Keep your answers brief.",
                name: "MemoryAgent");

        #endregion

        #region Custom ChatHistoryProvider wiring

        AIAgent customAgent = AgentConfig.CreateProjectClient()
            .AsAIAgent(new ChatClientAgentOptions()
            {
                ChatOptions = new() { Instructions = "You are a helpful assistant.", ModelId = AgentConfig.DeploymentName },
                ChatHistoryProvider = new CustomChatHistoryProvider(),
            });

        #endregion

        #region Session memory demo

        AgentSession session = await defaultAgent.CreateSessionAsync();

        Console.WriteLine(await defaultAgent.RunAsync("Hello! What's the square root of 9?", session));
        Console.WriteLine(await defaultAgent.RunAsync("My name is Alice", session));
        Console.WriteLine(await defaultAgent.RunAsync("What is my name?", session));

        #endregion

        // customAgent is created above to demonstrate the wiring; exercise it too so the example is complete.
        Console.WriteLine(await customAgent.RunAsync("Remember: my favorite color is teal."));
    }
}
