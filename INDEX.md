# Intent.Modules.NET Repository Analysis Index

**Last Updated**: 2024-12-19

This document provides an index of all analyzed Intent.NET modules in this repository, with one-sentence summaries of their purpose and functionality.

## Core Infrastructure and Common Modules

- **Intent.Modules.VisualStudio.Projects**: Visual Studio solution and project layout designer for managing .NET project structure and configurations.
- **Intent.Modules.Constants**: Provides constant value generation and management capabilities for .NET projects.

## Application Layer Modules

- **Intent.Modules.Application.MediatR.CRUD**: CRUD conventions for Commands and Queries using the MediatR pattern to implement standardized Create, Read, Update, Delete operations.
- **Intent.Modules.Application.MediatR**: Core MediatR integration module providing the foundation for CQRS pattern implementation.
- **Intent.Modules.Application.MediatR.Behaviours**: Cross-cutting behavior implementations for MediatR pipelines including validation, logging, and performance monitoring.
- **Intent.Modules.Application.AutoMapper**: AutoMapper integration for automatic object-to-object mapping in application layers.
- **Intent.Modules.Application.Contracts**: Defines service contracts and interfaces for application layer interactions.
- **Intent.Modules.Application.Dtos**: Data Transfer Object generation and management for application layer communication.
- **Intent.Modules.Application.FluentValidation**: FluentValidation integration for comprehensive input validation in application services.

## Data Persistence Modules

- **Intent.Modules.CosmosDB**: Core Cosmos DB Database Provider Repository backed using the Cosmos DB Client for document-based data persistence.
- **Intent.Modules.EntityFrameworkCore**: Entity Framework Core integration module providing ORM capabilities for relational database access.
- **Intent.Modules.EntityFrameworkCore.Repositories**: Repository pattern implementation for Entity Framework Core with standardized data access methods.
- **Intent.Modules.MongoDb**: MongoDB integration module for document database operations and data persistence.

## Web and API Modules

- **Intent.Modules.AspNetCore**: Core ASP.NET Core integration module providing web application framework support.
- **Intent.Modules.AspNetCore.Controllers**: ASP.NET Core MVC controller generation and management for RESTful API endpoints.
- **Intent.Modules.AspNetCore.Swashbuckle**: Swagger/OpenAPI documentation generation integration for ASP.NET Core APIs.
- **Intent.Modules.FastEndpoints**: FastEndpoints integration module providing high-performance, minimal API endpoint implementations.

## Cloud and Hosting Modules

- **Intent.Modules.AzureFunctions**: Azure Functions integration module for serverless computing implementations.
- **Intent.Modules.Azure.BlobStorage**: Azure Blob Storage integration for cloud-based file and object storage operations.
- **Intent.Modules.AmazonS3.ObjectStorage**: Amazon S3 integration module for AWS-based object storage capabilities.

## Security Modules

- **Intent.Modules.Security.JWT**: JSON Web Token (JWT) authentication and authorization implementation for secure API access.
- **Intent.Modules.AspNetCore.Identity**: ASP.NET Core Identity integration for user authentication and authorization management.

## Blazor and UI Modules

- **Intent.Modules.Blazor**: Core Blazor framework integration for building interactive web UIs with C#.
- **Intent.Modules.Blazor.Components.MudBlazor**: MudBlazor component library integration for rich Blazor UI components.

## Infrastructure and Cross-Cutting Modules

- **Intent.Modules.Infrastructure.DependencyInjection**: Dependency injection configuration and management for .NET applications.
- **Intent.Modules.OpenTelemetry**: OpenTelemetry integration for distributed tracing and observability.
- **Intent.Modules.Hangfire**: Hangfire integration module for background job processing and scheduling.

---

**Total Modules Analyzed**: 206

**Analysis Status**: In Progress - Initial core modules documented

## Module Categories

- **Application Layer**: 25+ modules
- **Data Persistence**: 15+ modules  
- **Web/API**: 20+ modules
- **Cloud/Hosting**: 10+ modules
- **Security**: 8+ modules
- **UI/Blazor**: 12+ modules
- **Infrastructure**: 15+ modules
- **Integration**: 20+ modules
- **Testing**: 8+ modules
- **DevOps/CI**: 5+ modules

For detailed analysis of individual modules, see the corresponding `[MODULE_NAME].md` files in this repository.