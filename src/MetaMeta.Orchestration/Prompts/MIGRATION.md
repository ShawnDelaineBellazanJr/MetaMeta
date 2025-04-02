# Prompt Migration Guide

This document outlines the migration from flat `.prompt` files to the structured `.prompty` library format as specified in the Cursor Prompty Standards (Rule 006).

## Migration Overview

1. **Original Structure**:
   - Flat files in `src/MetaMeta.Orchestration/Prompts/` directory
   - Mix of `.prompt` and `.prompty` files
   - No categorization

2. **New Structure**:
   - Categorized structure in `src/MetaMeta.Orchestration/Prompts/Library/`
   - All files in `.prompty` format with YAML headers
   - Organized into domain-specific categories

## File Conversion Process

1. **Convert File Format**:
   - Add YAML header to all files
   ```yaml
   ---
   name: PromptName
   description: Brief description
   model:
     api: chat
   ---
   ```
   - Keep the existing prompt content below the header

2. **Organize By Category**:
   - `AgentDesign`: Agent selection, orchestration, debugging prompts
   - `ContentCreation`: Content generation, summarization prompts
   - `ProductMarketing`: Marketing, sales, copy prompts
   - `TechnicalDocs`: Documentation generation prompts

3. **Rename Files**:
   - Use PascalCase for all files
   - Add "Agent" suffix to agent-specific prompts
   - Ensure descriptive names (e.g., `BlogPostGenerator.prompty`)

## Updated PromptLoader

The `PromptLoader.cs` has been updated to support:
1. Loading from both legacy locations and new Library structure
2. Category-based loading with an optional category parameter
3. Automatic searching across categories if needed

Example usage:
```csharp
// Load from specific category
var prompt = await _promptLoader.LoadPromptAsync("AgentName", "PromptName", "Category");

// Legacy loading (still supported)
var legacyPrompt = await _promptLoader.LoadPromptAsync("LegacyPromptName");
```

## Migration Checklist

- [x] Update PromptLoader.cs to support Library structure
- [x] Create Library directory structure
- [x] Move existing .prompty files to appropriate categories
- [x] Convert .prompt files to .prompty format
- [x] Update sample code to demonstrate Library usage
- [x] Create documentation for new structure
- [x] Verify build and functionality

## Future Improvements

1. Add full Handlebars templating to all prompts
2. Implement better error handling for missing prompts
3. Add validation for prompt YAML header format
4. Enhance PromptLoader with caching capabilities
5. Create a catalog/registry of available prompts 