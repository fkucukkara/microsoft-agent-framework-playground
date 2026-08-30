using Microsoft.Agents.AI.Workflows;

namespace AgentFrameworkCheatsheet.Samples;

/// <summary>
/// Step 5: Workflows.
/// Teaches: chaining independent steps (executors) into a graph instead of one monolithic agent call.
/// Docs: https://learn.microsoft.com/en-us/agent-framework/get-started/workflows
/// </summary>
internal static class Step05_Workflows
{
    #region Step 2 executor: reverse the string and yield output

    private sealed class ReverseTextExecutor() : Executor<string, string>("ReverseTextExecutor")
    {
        public override ValueTask<string> HandleAsync(
            string message, IWorkflowContext context, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(string.Concat(message.Reverse()));
    }

    #endregion

    public static async Task RunAsync()
    {
        #region Step 1 executor: convert text to uppercase

        Func<string, string> uppercaseFunc = s => s.ToUpperInvariant();
        var uppercase = uppercaseFunc.BindAsExecutor("UppercaseExecutor");

        #endregion

        ReverseTextExecutor reverse = new();

        #region Build and run the workflow

        WorkflowBuilder builder = new(uppercase);
        builder.AddEdge(uppercase, reverse).WithOutputFrom(reverse);
        var workflow = builder.Build();

        await using Run run = await InProcessExecution.RunAsync(workflow, "Hello, World!");

        foreach (WorkflowEvent evt in run.NewEvents)
        {
            if (evt is ExecutorCompletedEvent executorComplete)
            {
                Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
            }
        }

        #endregion
    }
}
