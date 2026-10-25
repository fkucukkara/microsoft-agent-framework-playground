using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.AI;

namespace AgentFrameworkCheatsheet.Configuration;

/// <summary>
/// Shared configuration and client factory for all cheatsheet samples.
/// Centralizing this avoids repeating the endpoint/credential wiring in every step.
/// </summary>
internal static class AgentConfig
{
    private const string DefaultDeploymentName = "gpt-5-mini";

    /// <summary>Foundry project endpoint, e.g. https://your-project.services.ai.azure.com.</summary>
    //public static string Endpoint =>
    //    Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    //    ?? throw new InvalidOperationException(
    //        "Set AZURE_OPENAI_ENDPOINT to your Microsoft Foundry project endpoint. See README.md for setup steps.");
    public static string Endpoint => "https://fatihkucukkaraaz-4878-resource.services.ai.azure.com";

    public static string DeploymentName =>
        Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? DefaultDeploymentName;

    /// <summary>Name of an existing Prompt Agent created in the Foundry portal (used by Step 8).</summary>
    public static string PredefinedAgentName =>
        Environment.GetEnvironmentVariable("FOUNDRY_AGENT_NAME") ?? "arch-agent-ml03wb9pvh";

    /// <summary>
    /// Project-scoped endpoint (account endpoint + /api/projects/{project}), required for Prompt Agent
    /// administration (used by Step 8) - the account-root <see cref="Endpoint"/> returns 404 for those calls.
    /// </summary>
    public static string ProjectEndpoint =>
        Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT")
        ?? $"{Endpoint}/api/projects/fatihkucukkaraaz-4878";

    /// <summary>
    /// Creates the shared <see cref="AIProjectClient"/>.
    /// NOTE: AzureCliCredential relies on `az login` and avoids DefaultAzureCredential's
    /// credential-probing latency; swap for a specific credential (e.g. ManagedIdentityCredential) in production.
    /// </summary>
    public static AIProjectClient CreateProjectClient(string? endpoint = null) =>
        new(new Uri(endpoint ?? Endpoint), new AzureCliCredential());

    /// <summary>
    /// Builds a raw <see cref="IChatClient"/> for scenarios (like the harness in Step 6) that operate
    /// directly against a chat client instead of an <see cref="Microsoft.Agents.AI.AIAgent"/>.
    /// </summary>
    public static IChatClient CreateChatClient() =>
        CreateProjectClient()
            .GetProjectOpenAIClient()
            .GetProjectResponsesClient()
            .AsIChatClient(DeploymentName);
}
