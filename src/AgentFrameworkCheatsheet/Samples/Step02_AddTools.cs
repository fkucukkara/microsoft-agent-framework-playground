using AgentFrameworkCheatsheet.Configuration;
using Azure.AI.Projects;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 2: Add Tools.
/// Teaches: giving an agent a function it can call to answer questions it can't reason about alone.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/add-tools
/// </summary>
internal static class Step02_AddTools
{
    #region Tool definition

    // The [Description] attributes are what the model reads to decide when/how to call this tool.
    [Description("Get the weather for a given location.")]
    private static string GetWeather([Description("The location to get the weather for.")] string location)
        => $"The weather in {location} is cloudy with a high of 15°C.";

    #endregion

    public static async Task RunAsync()
    {
        #region Create agent with tool

        AIAgent agent = AgentConfig.CreateProjectClient()
            .AsAIAgent(
                model: AgentConfig.DeploymentName,
                instructions: "You are a helpful assistant.",
                tools: [AIFunctionFactory.Create(GetWeather)]);

        #endregion

        #region Invoke

        // The model decides on its own whether GetWeather needs to be called to answer this prompt.
        Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));

        #endregion
    }
}
