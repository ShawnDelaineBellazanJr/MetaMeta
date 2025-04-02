# Prompt Organization Migration Plan

## Overview

This document outlines the standardization of prompt organization in the MetaMeta project. We are moving from a flat structure to a more organized, folder-based approach to better manage the growing number of prompt templates.

## New Folder Structure

All prompts are now organized into the following categories:

```
src/MetaMeta.Orchestration/Prompts/
├── AgentDesign/         # Prompts related to agent selection and architecture
├── ReasoningAgents/     # Prompts for reasoning, chat completion, and thinking
├── ExecutionAgents/     # Prompts for planning, execution, and tool usage
├── MemoryAgents/        # Prompts for memory, recall, and summarization
└── ContentCreation/     # Prompts for content generation and formatting
```

## File Extension Standardization

All prompt templates are being standardized to use the `.prompty` extension, rather than the mixture of `.prompt` and `.prompty` used previously.

## Migration Status

The migration is currently in progress:

- [x] Create folder structure
- [x] Move existing prompts to appropriate folders
- [x] Update PromptLoader.cs to search in the new folder structure
- [ ] Convert all `.prompt` files to `.prompty` format
- [ ] Update agent references to use the new paths

## How To Work With Prompts

When creating new prompts:

1. Determine the appropriate category for your prompt
2. Create the file in the corresponding folder with the `.prompty` extension
3. Use the standard Handlebars templating format

Example:
```
You are a {{$agent_type}} agent that helps with {{$capability}}.

TASK: {{$operation}}

{{#if $context}}
CONTEXT:
{{$context}}
{{/if}}

Your response should include:
1. {{$output_requirement1}}
2. {{$output_requirement2}}
```

## Runtime Behavior

The PromptLoader has been updated to search for prompts in this new folder structure. It follows this search order:

1. Looks in each category folder for the prompt by name
2. Falls back to the root Prompts directory
3. Searches both `.prompty` and `.prompt` extensions for backward compatibility

This approach maintains backward compatibility while encouraging the new organization pattern. 