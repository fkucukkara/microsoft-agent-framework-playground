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
    public static string Endpoint =>
        Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
        ?? throw new InvalidOperationException(
            "Set AZURE_OPENAI_ENDPOINT to your Microsoft Foundry project endpoint. See README.md for setup steps.");

    public static string DeploymentName =>
        Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? DefaultDeploymentName;

    /// <summary>
    /// Creates the shared <see cref="AIProjectClient"/>.
    /// NOTE: AzureCliCredential relies on `az login` and avoids DefaultAzureCredential's
    /// credential-probing latency; swap for a specific credential (e.g. ManagedIdentityCredential) in production.
    /// </summary>
    public static AIProjectClient CreateProjectClient() =>
        new(new Uri(Endpoint), new AzureCliCredential());

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
