# MetaMeta Project Structure

## Overview

MetaMeta is organized as a modular .NET solution with several projects, each with a specific responsibility in the architecture.

## Source Code Structure

The source code follows this organization:

```
/src
├── MetaMeta.AgentHost/          # Agent hosting service
├── MetaMeta.AppHost/            # Application host for .NET Aspire orchestration
├── MetaMeta.ApiService/         # REST API services
├── MetaMeta.Configs/            # Configuration resources
├── MetaMeta.Core/               # Core domain models and abstractions
│   ├── Abstractions/            # Interfaces and abstract classes
│   ├── Agents/                  # AI assistant agent implementations
│   ├── Models/                  # Domain models
│   ├── Plugins/                 # Extension plugins
│   ├── Prompts/                 # Prompt templates for AI
│   └── Runtime/                 # Runtime components
├── MetaMeta.GrpcService/        # gRPC service implementations
├── MetaMeta.Infrastructure/     # Data access and external services integration
│   ├── Data/                    # Database contexts
│   └── Repositories/            # Repository implementations
├── MetaMeta.Orchestration/      # Agent orchestration service
├── MetaMeta.ServiceDefaults/    # Shared service configuration defaults
└── MetaMeta.Web/                # Web UI application
/tests
└── MetaMeta.Core.Tests/         # Core project tests
```

## Project Responsibilities

- **MetaMeta.AgentHost**: Agent hosting service.
- **MetaMeta.AppHost**: The .NET Aspire application host that orchestrates all services.
- **MetaMeta.ApiService**: REST API endpoints for external communication.
- **MetaMeta.Configs**: Configuration resources for the application.
- **MetaMeta.Core**: Contains domain models, interfaces, and business logic.
  - **Abstractions**: Interfaces and abstract base classes like `BaseEntity` and `IGenericRepository<T>`.
  - **Models**: Domain model classes like `Assistant`.
  - **Agents**: AI assistant implementations.
  - **Plugins**: Extension components that add functionality.
  - **Prompts**: Templates for AI model prompting.
- **MetaMeta.GrpcService**: gRPC service implementations for internal communication.
- **MetaMeta.Infrastructure**: Implementation of data access and external service integrations.
  - **Repositories**: Data access implementations like `GenericRepository<T>` and `AssistantRepository`.
  - **Data**: Database contexts like `AppDbContext`.
- **MetaMeta.Orchestration**: Service for orchestrating multiple AI agents.
- **MetaMeta.ServiceDefaults**: Default configurations shared across services.
- **MetaMeta.Web**: Web frontend application.

## Naming Conventions

- All top-level folders must follow `MetaMeta.[DomainName]` naming convention
- Agent classes must be named `[Something]Assistant.cs`
- Plugins must be named `[Capability]Plugin.cs`
- Prompts must end in `.prompty` and live in `src/MetaMeta.Core/Prompts`

## Project Dependencies

The dependency flow generally follows:
- Web UI and API Services depend on Core and Infrastructure
- Core contains interfaces and abstractions
- Infrastructure implements Core interfaces
- AppHost references all projects for orchestration

This modular structure enables scalable AI agent orchestration while maintaining a clean separation of concerns throughout the application. 