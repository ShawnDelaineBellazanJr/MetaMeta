# MetaMeta Prompt Library

This directory contains categorized prompt templates that follow the Cursor Prompty Standards (Rule 006). These templates are designed to be reusable, well-structured, and version-controlled.

## Directory Structure

The prompt library is organized into specific categories, each focusing on a particular domain:

- **AgentDesign**: Prompts for designing, debugging, and orchestrating AI agents
- **ContentCreation**: Prompts for generating various types of content (tutorials, guides, FAQs, etc.)
- **FeedbackAnalytics**: Prompts for analyzing user feedback, A/B test results, and sentiment
- **Marketing**: Prompts for social media campaigns, product launches, and SEO content
- **Monetization**: Prompts for pricing strategies, feature tiering, and freemium models
- **ProductDevelopment**: Prompts for project scaffolding, API specifications, and CRUD operations
- **Sales**: Prompts for outreach emails, value propositions, and product comparisons
- **TechnicalDocs**: Prompts for technical documentation (API docs, architecture diagrams, etc.)

## Usage

You can use the `PromptLoader` class to load prompts from this library structure:

```csharp
// Load a prompt from a specific category
var prompt = await _promptLoader.LoadPromptAsync(
    "PricingStrategyGenerator",  // Agent name
    "PricingStrategyGenerator",  // Prompt name (can be different from agent)
    "Monetization"               // Category
);

// Create a function from the prompt
var function = _kernel.CreateFunctionFromPrompt(
    prompt.Template,
    new PromptExecutionSettings()
);

// Execute with arguments
var result = await _kernel.InvokeAsync(function, new KernelArguments
{
    ["product_name"] = "SaaS Platform X",
    ["target_audience"] = "Small Business Owners",
    ["competitor_pricing"] = "Competitor A: $29/mo, Competitor B: $49/mo"
});
```

## Prompt File Format

All prompts follow the `.prompty` YAML format:

```yaml
---
name: PromptName
description: Clear description of what this prompt does
model:
  api: chat
---

You are an assistant that...

Parameter 1: {{$parameter1}}
Parameter 2: {{$parameter2}}

Your task is to...
```

## Creating New Prompts

When creating new prompts:

1. Place the prompt in the appropriate category directory
2. Use descriptive PascalCase naming (e.g., `PricingStrategyGenerator.prompty`)
3. Include mandatory YAML header with name, description, and model
4. Use Handlebars syntax for dynamic inputs (`{{$variable_name}}`)
5. Include conditional sections with `{{#if variable}}...{{/if}}` or `{{#each items}}...{{/each}}`
6. Provide clear step-by-step instructions in the prompt
7. Add example input/output in JSON format when applicable

## JSON Response Format

Many of the prompts are designed to return structured JSON responses. This enables:

1. Consistent outputs that can be parsed programmatically
2. Easy integration with downstream processes
3. Clear organization of complex information
4. Ability to extract specific parts of responses

When creating prompts that return JSON, include a template of the expected format in the prompt itself to guide the model's response.

## Migrating Existing Prompts

To convert existing `.prompt` files to the `.prompty` format:

1. Add the YAML header section
2. Update variable references to use Handlebars syntax
3. Add to the appropriate category directory
4. Update code to load from the Library structure 