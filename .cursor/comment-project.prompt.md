
# 📌 MetaMeta Project Auto-Commenting Pass

## 🎯 Goal:
Read through all C# files in the workspace and apply structured, meaningful comments to:

- Public classes, interfaces, records
- Method definitions (especially agents, plugin handlers, planners)
- Important properties (e.g., required fields in DTOs or directives)
- Core orchestration steps (e.g., ExecuteAsync, Objective, Plugin dispatch)
- Prompt loaders and SKFunction/Kernel interaction

## 🧠 Style Guide:
- Use XML comments (`///`) for public members and classes
- Use inline `//` comments for internal logic steps
- Annotate “reasoning” flow with numbered steps
- Use semantic tags where helpful (e.g. `// @meta:Agent`)
- Keep comments brief, useful, and focused on *why*

## 🧪 Scope:
- Scan `src/**/*.cs`, `plugins/**/*.cs`, `core/**/*.cs`
- Exclude auto-generated files, `obj/`, `bin/`, `.g.cs`

## ✅ Output Format:
- Updated C# files with XML + inline comments
- Comment headers before class declarations
- Stepwise flow comments inside key methods

## Example:
```csharp
/// <summary>
/// Represents the directive object created by the ExecutiveAgent.
/// </summary>
public class NorthStarDirective
{
    // Required goal provided by the user
    public required string Goal { get; set; }

    /// <summary>
    /// Generates steps based on the high-level goal.
    /// </summary>
    public async Task<string?> Objective(string goal)
    {
        // Step 1: Submit prompt to agent
        var response = await _agent.Kernel.InvokePromptAsync($"steps to achieve goal: {goal}");

        // Step 2: Return rendered plan
        return response.RenderedPrompt;
    }
}
```

---

## 2. 🚀 Run It in Cursor

- Open your full `MetaMeta/` project in Cursor
- Right-click on `comment-project.prompt.md`
- Select **“Run Prompt on Workspace”**

💡 Tip: You can also scope the prompt to only `src/MetaMeta.Core/` or `src/MetaMeta.Orchestration/` if you want to test it first.

---

## 🛠️ Optional Enhancement (MDC Style)

Drop this line into your `.cursor/rules/001_core.mdc` to make comments *enforceable*:

```mdc
All public classes, agents, and service methods must have XML summary comments and inline step documentation.
```
