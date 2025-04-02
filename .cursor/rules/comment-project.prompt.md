# 🧠 MetaMeta Auto-Commenting with Stepwise Logic

## 🎯 Objective:
Decorate all C# classes, methods, and agent logic with clear, useful comments.

This includes:
- XML-style doc comments (`///`) for all public methods, classes, records
- Inline `// Step 1:`, `// Step 2:` for critical method flows (e.g., ExecuteAsync, Objective, Plugin.Apply)
- Logic breakdowns inside orchestration services and agents
- Prompt loaders, plugins, and directive execution paths

---

## 💬 Comment Format Guide

- Use **XML comments** for public interfaces and classes
- Use **inline numbered comments** for methods with execution flow
- Add one-sentence **"why" rationale** per method or handler

---

## 🧪 Stepwise Inline Logic Example

```csharp
/// <summary>
/// Generates a directive from a user's long-term goal using a chat completion agent.
/// </summary>
public class NorthStarDirective
{
    public required string Assistant { get; set; }
    public required string Goal { get; set; }

    /// <summary>
    /// Uses an SK agent to break the goal into steps.
    /// </summary>
    public async Task<string?> Objective(string goal)
    {
        // Step 1: Submit the goal as a prompt to the chat completion agent
        var response = await _agent.Kernel.InvokePromptAsync($"steps to achieve goal: {goal}");

        // Step 2: Extract the formatted plan from the agent's response
        return response.RenderedPrompt;
    }
}
```

---

## 📁 Target Paths

- Scan all `.cs` files inside:
  - `src/MetaMeta.Core/`
  - `src/MetaMeta.Orchestration/`
  - `src/MetaMeta.Plugins/`
  - `src/MetaMeta.GrpcService/`
  - `src/MetaMeta.ApiService/Controllers/`

---

## 📜 Output Requirements

- For each file, insert XML-style summary comments on all:
  - Public classes
  - Public methods
  - DTOs and plugin models

- Add `// Step X:` annotations inside:
  - Agents (e.g., `AgentBase<TRequest, TResponse>`)
  - Service methods (`NorthStar()`, `ExecuteAsync()`)
  - Planner flows and SK invocations

---

## 🧠 Purpose

Make MetaMeta:
- ✅ Easier to onboard for new developers
- ✅ Self-documenting for AI agents (like Cursor, Copilot)
- ✅ Ready for open-source or enterprise SDK consumption

---

### ✅ What to Do in Cursor

1. Save this file as `.cursor/comment-project.prompt.md`
2. Open your **full workspace**
3. Right-click → **Run Prompt on Workspace**
4. Cursor will:
   - Read all your classes
   - Explain flow in comments
   - Add `///` + `// Step 1:` inline

---

### ⚙️ Bonus: You Want to Log Steps Too?

Add runtime logging alongside step comments like:

```csharp
_logger.LogInformation("Step 2: Prompt executed, result: {result}", result);
``` 