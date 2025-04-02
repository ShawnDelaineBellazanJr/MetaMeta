# MetaMeta Project Structure

## Overview

MetaMeta is a .NET-based AI IDE built on Semantic Kernel agent architecture. The project is organized as a collection of microservices using .NET Aspire, with a modular design that separates concerns and promotes maintainability.

## High-Level Directory Structure

```
/
├── .cursor/                  # Cursor IDE configuration and rules
├── .devcontainer/            # Development container configuration
├── .github/                  # GitHub workflows and templates
├── Prompts/                  # Global prompt templates (see note below)
├── src/                      # Source code for all MetaMeta projects
├── tests/                    # Test projects
├── .gitignore                # Git ignore rules
├── LICENSE                   # Project license
├── MetaMeta.sln              # Solution file
├── README.md                 # Main project documentation
└── PROJECT_STRUCTURE.md      # Project structure documentation
```

## Source Code Structure (`/src/`)

The source code follows a logical organization with each project having a specific responsibility:

```
/src
├── MetaMeta.AgentHost/          # Agent hosting service
│   ├── Controllers/             # API controllers for agent interactions
│   └── Services/                # Agent hosting services
│
├── MetaMeta.AppHost/            # .NET Aspire application host
│   └── Program.cs               # Orchestrates all services
│
├── MetaMeta.ApiService/         # REST API services
│   ├── Controllers/             # API endpoints
│   └── Models/                  # API request/response models
│
├── MetaMeta.Configs/            # Configuration resources
│   └── Settings/                # App settings and configuration classes
│
├── MetaMeta.Core/               # Core domain models and abstractions
│   ├── Abstractions/            # Interfaces and abstract classes
│   ├── Chat/                    # Chat functionality
│   ├── Models/                  # Domain models
│   └── PromptTemplates/         # Core prompt template definitions
│
├── MetaMeta.GrpcService/        # gRPC service implementations
│   ├── Protos/                  # Protocol buffer definitions
│   └── Services/                # gRPC service implementations
│
├── MetaMeta.Infrastructure/     # Data access and external integrations
│   ├── Data/                    # Database contexts
│   └── Repositories/            # Repository implementations
│
├── MetaMeta.Orchestration/      # Agent orchestration service
│   ├── Agents/                  # AI agent implementations
│   ├── Prompts/                 # Prompt templates (organized structure below)
│   ├── Services/                # Orchestration services
│   ├── Templates/               # Template utilities
│   └── Models/                  # Orchestration models
│
├── MetaMeta.Plugins/            # Extension plugins
│   └── Connectors/              # External service connectors
│
├── MetaMeta.ServiceDefaults/    # Shared service configuration defaults
│   └── Extensions/              # Service extension methods
│
├── MetaMeta.Console/            # Console application interface
│
└── MetaMeta.Web/                # Web UI application
    ├── Components/              # Blazor components
    ├── Pages/                   # Blazor pages
    └── wwwroot/                 # Static web assets
```

## Prompt Organization Structure

Prompts are primarily organized within the Orchestration project and follow a structured approach:

```
/src/MetaMeta.Orchestration/Prompts/
├── AgentDesign/               # Agent selection and architecture prompts
│   └── AgentSelection.prompty # Agent selection logic
│
├── ReasoningAgents/           # Reasoning, chat, and thinking prompts
│   ├── ChatCompletion.prompty        # Basic chat completion
│   ├── ReasoningAgent.prompty        # Advanced reasoning capabilities
│   └── TechnicalChatCompletion.prompty # Technical domain chat
│
├── ExecutionAgents/           # Planning, execution, and tool usage prompts
│   ├── ExecutionPlan.prompty         # Plan generation
│   ├── ExecutiveStrategy.prompty     # Strategic decision making
│   ├── PlannerAgent.prompty          # Task planning
│   ├── ResultIntegration.prompty     # Result processing
│   └── ToolAgent.prompty             # Tool orchestration
│
├── MemoryAgents/              # Memory, recall, and summarization prompts
│   ├── MemoryAgent.prompty           # Memory management
│   ├── MemoryRecall.prompty          # Information retrieval
│   └── SummaryAgent.prompty          # Content summarization
│
├── ContentCreation/           # Content generation prompts
│   ├── ContentAgent.prompty          # General content creation
│   └── GenericSummarization.prompty  # Content summarization
│
├── MIGRATION.md               # Documentation of prompt organization changes
└── PromptLoader.cs            # Utility for loading prompts from files
```

> **Note:** There is also a global `/Prompts/` directory at the root level for project-wide prompts that don't belong to specific agent implementations.

## Test Structure (`/tests/`)

The test projects mirror the structure of the main source code:

```
/tests
└── MetaMeta.Core.Tests/     # Tests for Core functionality
    ├── Abstractions/        # Tests for interfaces and abstract classes
    └── Models/              # Tests for domain models
```

> **Note:** Additional test projects for other modules are planned to improve coverage.

## Cursor Configuration (`.cursor/`)

The `.cursor/` directory contains configuration files for the Cursor IDE:

```
/.cursor
├── rules/                  # Rules enforcing architectural patterns
│   ├── 000_structure.mdc   # Project structure standards
│   ├── 001_core.mdc        # Core module standards
│   ├── 002_runtime.mdc     # Runtime behavior standards
│   ├── 003_assistants.mdc  # Assistant implementation standards
│   ├── 004_plugins.mdc     # Plugin development standards
│   ├── 005_orchestration.mdc # Orchestration standards
│   ├── 006_prompts.mdc     # Prompt template standards
│   └── 007_tests.mdc       # Testing standards
└── tasks/                  # Automated tasks for Cursor
```

## Key Files

- **MetaMeta.sln**: Visual Studio solution file that organizes all projects
- **PROJECT_STRUCTURE.md**: Additional documentation about the project structure
- **README.md**: Main project documentation with setup and usage instructions
- **.gitignore**: Specifies intentionally untracked files to ignore
- **LICENSE**: License information for the project

## Naming Conventions

- All top-level folders follow `MetaMeta.[DomainName]` naming convention
- Agent classes are named according to their purpose (e.g., `ReasoningAgent.cs`)
- Prompts use `.prompty` extension and are organized by function
- Controllers use the standard ASP.NET `[Resource]Controller` naming

## Project Dependencies

The dependency flow generally follows:
- Web UI and API Services depend on Core and Infrastructure
- Core contains interfaces and abstractions
- Infrastructure implements Core interfaces
- AppHost references all projects for orchestration

This modular structure enables scalable AI agent orchestration while maintaining a clean separation of concerns throughout the application. 