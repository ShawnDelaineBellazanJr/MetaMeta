# MetaMeta Prompts

This directory serves as a central location for high-level, project-wide prompts that don't belong to specific agent implementations.

## Important Note

The primary prompt storage is now in `src/MetaMeta.Orchestration/Prompts/` and organized into categories.

Please refer to [Prompts Migration Documentation](../src/MetaMeta.Orchestration/Prompts/MIGRATION.md) for details on the new organization structure.

## Usage Guidelines

1. Agent-specific prompts should be placed in their appropriate category folder in the Orchestration project
2. Only global, project-wide prompts should be placed here
3. All prompts should use the `.prompty` extension and Handlebars format

## Relevant Files

- `src/MetaMeta.Orchestration/Prompts/PromptLoader.cs` - Handles loading prompts at runtime
- `src/MetaMeta.Core/Abstractions/IPromptTemplate.cs` - Interface for prompt templates
- `src/MetaMeta.Core/Abstractions/IPromptTemplateFactory.cs` - Factory for creating prompt templates

## Prompt Categories

- **AgentDesign**: Architecture and agent selection prompts
- **ReasoningAgents**: Reasoning, chat, and thinking prompts
- **ExecutionAgents**: Planning, execution, and tool usage prompts
- **MemoryAgents**: Memory, recall, and summarization prompts
- **ContentCreation**: Content generation prompts

See the [full documentation](../src/MetaMeta.Orchestration/Prompts/MIGRATION.md) for more details. 