# MetaMeta AI IDE

A modular AI-powered IDE built with .NET Aspire, Semantic Kernel, and autonomous agents.

## Features

- AI-powered development assistance with Semantic Kernel
- Microservices architecture using .NET Aspire
- Autonomous agents for code analysis and generation
- DevContainer support for consistent development environments
- Modern web interface for seamless development experience

## Architecture

The MetaMeta AI IDE is built on a microservices architecture with the following components:

- **ApiService**: REST API for interacting with agents and services
- **GrpcService**: gRPC-based service for efficient inter-service communication
- **AgentHost**: Host for autonomous AI agents
- **Web**: Frontend interface for the IDE
- **Core**: Core domain models and abstractions
- **Infrastructure**: Data persistence and external integrations
- **Orchestration**: Coordination of agent workflows
- **Plugins**: Extensibility modules for additional capabilities

## Setup

### Prerequisites

- Docker and Docker Compose
- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extensions

### Development Environment

1. Clone the repository
2. Open in VS Code with the "Remote Containers" extension
3. VS Code will automatically build and start the DevContainer
4. Run the application:
   ```bash
   dotnet run --project src/MetaMeta.AppHost
   ```

## API Usage

### Executive Agent API

```http
POST /api/executive/analyze
Content-Type: application/json

{
  "requestId": "test-123",
  "sessionId": "session-456",
  "problem": "Generate a RESTful API controller for user management",
  "style": "Clean Architecture",
  "maxSteps": 5
}
```

For more detailed API documentation, run the application and navigate to the Swagger UI at `/swagger`.
