namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 7: Host Your Agent (REFERENCE ONLY — not executed from the menu).
/// ASP.NET Core hosting needs an <c>IHostApplicationBuilder</c>/<c>WebApplication</c>, which can't
/// live inside this console app's Main() alongside the other samples. See README.md, section
/// "Step 7: Host Your Agent" for the full explanation and a pointer to a standalone Web App to try this.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/hosting
/// </summary>
internal static class Step07_HostYourAgent
{
    public static void PrintReferenceNotice()
    {
        Console.WriteLine();
        Console.WriteLine("Step 7 (Host Your Agent) requires an ASP.NET Core host and can't run inside this");
        Console.WriteLine("console app. Read the annotated code below and see README.md > 'Step 7' to try it");
        Console.WriteLine("in a minimal Web API project of your own.");
        Console.WriteLine();
    }

    /*
     * The snippets below mirror the official docs 1:1 and are meant to be read, not compiled here.
     * They'd live in a Program.cs of a separate `dotnet new web` project that also references
     * Microsoft.Agents.AI.Hosting (+ Microsoft.Agents.AI.Hosting.A2A.AspNetCore for A2A).

        var builder = WebApplication.CreateBuilder(args);

        #region Register a chat client for agents to share

        // endpoint is your Microsoft Foundry project endpoint; deploymentName e.g. "gpt-5-mini".
        IChatClient chatClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential())
            .GetProjectOpenAIClient()
            .GetProjectResponsesClient()
            .AsIChatClient(deploymentName);
        builder.Services.AddSingleton(chatClient);

        #endregion

        #region Register an agent with DI (AddAIAgent)

        var pirateAgent = builder.AddAIAgent(
            "pirate",
            instructions: "You are a pirate. Speak like a pirate",
            description: "An agent that speaks like a pirate.",
            chatClientServiceKey: "chat-model")
            .WithAITool(new MyTool())          // attach a custom AITool
            .WithInMemorySessionStore();       // configure conversation-data storage

        #endregion

        #region Register a multi-agent workflow (AddWorkflow)

        builder.AddAIAgent("agent-1", instructions: "you are agent 1!");
        builder.AddAIAgent("agent-2", instructions: "you are agent 2!");

        var workflow = builder.AddWorkflow("my-workflow", (sp, key) =>
        {
            var agent1 = sp.GetRequiredKeyedService<AIAgent>("agent-1");
            var agent2 = sp.GetRequiredKeyedService<AIAgent>("agent-2");
            return AgentWorkflowBuilder.BuildSequential(key, [agent1, agent2]);
        });

        // Convert the workflow into a standalone agent so it can use the same protocol integrations.
        var workflowAsAgent = workflow.AddAsAIAgent();

        #endregion

        #region Expose via the A2A protocol

        builder.Services.AddA2AServer();
        var app = builder.Build();
        app.MapA2AServer();
        app.Run();

        #endregion

     */
}
