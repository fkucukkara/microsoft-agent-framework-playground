# Microsoft Agent Framework — .NET Cheatsheet

An educational, runnable .NET console app that walks through the official
[Agent Framework "Get started"](https://learn.microsoft.com/en-us/agent-framework/get-started/)
tutorial, one step per file. Pick a step from the menu, read the code alongside the
output, and use it as a quick-reference when building your own agents.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A [Microsoft Foundry](https://learn.microsoft.com/en-us/azure/ai-foundry/) project with a
  chat-capable model deployed (e.g. `gpt-5-mini`)
- Signed in locally so `AzureCliCredential` can authenticate, e.g. `az login`

Steps 1–4 and 6 make real calls to your deployed model — no mocking. Step 5 (Workflows)
runs entirely offline. Step 7 is documentation only (see below).

## Configuration

Set these environment variables before running:

| Variable | Required | Description |
|---|---|---|
| `AZURE_OPENAI_ENDPOINT` | Yes | Your Foundry project endpoint, e.g. `https://your-project.services.ai.azure.com` |
| `AZURE_OPENAI_DEPLOYMENT_NAME` | No | Model deployment name. Defaults to `gpt-5-mini` |

If `AZURE_OPENAI_ENDPOINT` isn't set, each sample fails fast with a clear message instead
of an unhandled exception.

## Run it

```powershell
$env:AZURE_OPENAI_ENDPOINT = "https://your-project.services.ai.azure.com"
dotnet run --project src/AgentFrameworkCheatsheet
```

You'll get an interactive menu — pick a step (1-7), see it run, then pick another.

## Project layout

```
src/AgentFrameworkCheatsheet/
├── Program.cs                                 # Interactive menu / dispatcher
├── Configuration/AgentConfig.cs                # Shared endpoint/credential/client wiring
└── Samples/
    ├── Step01_YourFirstAgent.cs
    ├── Step02_AddTools.cs
    ├── Step03_MultiTurnConversations.cs
    ├── Step04_MemoryAndPersistence.cs
    ├── Step05_Workflows.cs
    ├── Step06_AgentHarness.cs
    └── Step07_HostYourAgent.cs                 # Reference only, see below
```

Each sample file uses `#region` blocks to separate the concepts being taught (tool
definition vs. agent creation vs. invocation, etc.), with single-line comments only
where the code itself doesn't make the "why" obvious.

## The steps

### Step 1 — Your First Agent
Creates an `AIAgent` from an `AIProjectClient` and invokes it both non-streaming
(`RunAsync`) and streaming (`RunStreamingAsync`).
**Best practices:** prefer streaming for interactive UIs; swap `AzureCliCredential`
for a specific credential (e.g. `ManagedIdentityCredential`) in production to avoid
relying on the Azure CLI being installed/signed in on the host.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/your-first-agent)

### Step 2 — Add Tools
Exposes a C# method as a callable tool via `[Description]` + `AIFunctionFactory.Create`.
**Best practices:** write specific, unambiguous `[Description]` text — it's the only
thing the model uses to decide when/how to call your tool; keep tools narrow and
single-purpose.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/add-tools)

### Step 3 — Multi-Turn Conversations
Uses `AgentSession` so the agent remembers earlier turns.
**Best practices:** create one session per logical conversation, not per call; don't
share a session across unrelated users/requests.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/multi-turn)

### Step 4 — Memory & Persistence
Shows the default `InMemoryChatHistoryProvider` fallback and a minimal custom
`ChatHistoryProvider` (override `ProvideChatHistoryAsync` / `StoreChatHistoryAsync`) for
mirroring history to your own store.
**Best practices:** only one history provider should load messages per run to avoid
replaying duplicate context; keep custom providers side-effect-light since they run on
every turn.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/memory)

### Step 5 — Workflows
Chains two `Executor`s (`BindAsExecutor` for a function, a class-based `Executor<string,string>`)
into a graph with `WorkflowBuilder`, run via `InProcessExecution`.
**Best practices:** keep each executor single-responsibility so steps are independently
testable and reusable across workflows.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/workflows)

### Step 6 — Agent Harness
Wraps a raw `IChatClient` with `AsHarnessAgent()`, which adds planning, todo tracking,
and history persistence for long multi-step tasks, then drives it from a REPL loop.
**Best practices:** always keep the `AgentSession` alive across turns so harness state
(plan/todos/history) persists; stream output since harness turns can take a while.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/harness)

### Step 7 — Host Your Agent (reference only)
ASP.NET Core hosting (`AddAIAgent`, `AddWorkflow`, `AddAsAIAgent`, A2A/`AddA2AServer`)
needs an `IHostApplicationBuilder`/`WebApplication`, which can't coexist with this
console app's `Main()`. [Step07_HostYourAgent.cs](src/AgentFrameworkCheatsheet/Samples/Step07_HostYourAgent.cs)
contains the fully annotated docs code as a reference — copy it into a
`dotnet new web` project (referencing `Microsoft.Agents.AI.Hosting`, and
`Microsoft.Agents.AI.Hosting.A2A.AspNetCore` for the A2A snippet) to try it live.
**Best practices:** register your `IChatClient` once and share it across agents via DI;
convert multi-agent workflows to an `AIAgent` (`AddAsAIAgent`) before exposing them
through a hosting protocol.
📖 [docs](https://learn.microsoft.com/en-us/agent-framework/get-started/hosting)

## NuGet packages used

`Azure.AI.Projects`, `Azure.Identity`, `Microsoft.Agents.AI.Foundry`,
`Microsoft.Agents.AI.Harness`, `Microsoft.Agents.AI.Workflows`, `Microsoft.Extensions.AI`
(all current as of the Agent Framework preview at the time of writing — these APIs may
still change before general availability).
