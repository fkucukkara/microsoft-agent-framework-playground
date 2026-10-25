using AgentFrameworkCheatsheet.Samples;

Console.WriteLine("=================================================");
Console.WriteLine(" Microsoft Agent Framework - .NET Cheatsheet");
Console.WriteLine("=================================================");

while (true)
{
    PrintMenu();
    string? choice = Console.ReadLine();

    if (choice is null or "0")
    {
        break;
    }

    try
    {
        await RunStepAsync(choice);
    }
    catch (InvalidOperationException ex)
    {
        // Missing configuration (e.g. AZURE_OPENAI_ENDPOINT) throws this - surface it, don't crash.
        Console.WriteLine($"\n[Configuration error] {ex.Message}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[Error] {ex.GetType().Name}: {ex.Message}\n");
    }
}

static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("1. Your First Agent");
    Console.WriteLine("2. Add Tools");
    Console.WriteLine("3. Multi-Turn Conversations");
    Console.WriteLine("4. Memory & Persistence");
    Console.WriteLine("5. Workflows");
    Console.WriteLine("6. Agent Harness");
    Console.WriteLine("7. Host Your Agent (reference only)");
    Console.WriteLine("8. Predefined Agent (Foundry Prompt Agent)");
    Console.WriteLine("0. Exit");
    Console.Write("Select a step: ");
}

static Task RunStepAsync(string choice) => choice switch
{
    "1" => Step01_YourFirstAgent.RunAsync(),
    "2" => Step02_AddTools.RunAsync(),
    "3" => Step03_MultiTurnConversations.RunAsync(),
    "4" => Step04_MemoryAndPersistence.RunAsync(),
    "5" => Step05_Workflows.RunAsync(),
    "6" => Step06_AgentHarness.RunAsync(),
    "7" => PrintStep07Notice(),
    "8" => Step08_PredefinedAgent.RunAsync(),
    _ => PrintUnknownChoice(),
};

static Task PrintStep07Notice()
{
    Step07_HostYourAgent.PrintReferenceNotice();
    return Task.CompletedTask;
}

static Task PrintUnknownChoice()
{
    Console.WriteLine("Unrecognized choice, try again.");
    return Task.CompletedTask;
}

