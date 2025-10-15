# Intent.Modules.Aws.Sqs - Implementation Plan

## � Table of Contents

1. [📋 Executive Summary](#-executive-summary)
2. [🗺️ Quick Reference Guide](#️-quick-reference-guide)
3. [🎯 Objectives](#-objectives)
4. [🏗️ Architecture Context](#️-architecture-context)
5. [🔗 Shared Project Implementation (PRIORITY)](#-shared-project-implementation-priority)
6. [📦 Components to Implement (Main Module)](#-components-to-implement-main-module)
7. [📚 NuGet Package Dependencies](#-nuget-package-dependencies)
8. [📝 Module Metadata (.imodspec)](#-module-metadata-imodspec)
9. [🔧 Detailed Component Specifications](#-detailed-component-specifications)
10. [🏛️ Architectural Decisions](#️-architectural-decisions)
11. [🔗 Integration Points](#-integration-points)
12. [🧪 Testing Strategy](#-testing-strategy)
13. [📋 Implementation Checklist](#-implementation-checklist)
14. [🎯 Success Criteria](#-success-criteria)
15. [🚀 Key Differences: Azure Service Bus vs AWS SQS](#-key-differences-azure-service-bus-vs-aws-sqs)
16. [📚 Reference Files](#-reference-files)
17. [📝 Notes for Implementation Agent](#-notes-for-implementation-agent)
18. [📖 TL;DR - Quick Start Guide](#-tldr---quick-start-guide-for-implementation-agent)

---

## �📋 Executive Summary

This document outlines the implementation plan for `Intent.Modules.Aws.Sqs`, a core eventing infrastructure module for AWS SQS integration in Intent Architect. This module follows the architectural pattern established by `Intent.Modules.Eventing.AzureServiceBus` and provides the foundation for SQS-based event publishing and message routing.

**Status:** Implementation Phase - Shared Project Priority  
**Target Completion:** TBD  
**Dependencies:** Intent.Eventing.Contracts, Intent.Modelers.Eventing, Intent.Modelers.Services.EventInteractions

### 🚨 CRITICAL: Implementation Order

**⚠️ MUST IMPLEMENT SHARED PROJECT FIRST ⚠️**

The shared project `Intent.Modules.Integration.IaC.Shared.AwsSqs` is a **prerequisite** for all main module templates. It provides:
- `IntegrationManager` - Metadata aggregation singleton
- `SqsItemBase` - Base abstraction for messages/commands
- `SqsMessage` - Concrete message implementation
- `SqsMethodType` - Publish/Subscribe enum

**Implementation sequence:**
1. ✅ **Phase 1: Shared Project** (Pages 24-52) - START HERE
2. ✅ **Phase 2: API Extensions** (Stereotypes)
3. ✅ **Phase 3: Factory Extensions** (MetadataLoader)
4. ✅ **Phase 4-6: Templates** (EventBus, Dispatcher, Configuration)

**Do NOT skip the shared project** - templates will fail to compile without it.

### 📍 Workspace Navigation

**Current Location:** `Intent.Modules.Aws.Sqs/` folder (this module)  
**Workspace Root:** Parent folder containing all Intent modules

**How to find reference files:**
```bash
# Use these tools to explore the workspace:
file_search("**/*AzureServiceBus*.cs")           # Find all Azure files
file_search("**/IntegrationManager.cs")          # Find IntegrationManager
grep_search("IntegrationManager", isRegexp=false) # Search for usage patterns
list_dir("../Intent.Modules.Eventing.AzureServiceBus") # List Azure module files
```

**Key Reference Locations (relative paths):**
- Azure Shared Project: `../Intent.Modules.Integration.IaC.Shared.AzureServiceBus/`
- Azure Main Module: `../Intent.Modules.Eventing.AzureServiceBus/`
- AWS SQS Shared Project: `../Intent.Modules.Integration.IaC.Shared.AwsSqs/` (to create)
- AWS SQS Main Module: `./` (current directory)

---

## 🗺️ Quick Reference Guide

### For Implementation Agent

| What to Build | Where to Look | Complexity |
|---------------|---------------|------------|
| **Shared Project Files** | Section "🔗 Shared Project Implementation (PRIORITY)" | Medium |
| **API Stereotype Extensions** | Section "2. API Extensions (1 Total)" | Simple |
| **Factory Extension** | Section "3. Factory Extensions (1 Total)" | Simple |
| **Templates** | Section "📦 Components to Implement" | Medium-Complex |
| **Complete Checklist** | Section "📋 Implementation Checklist" | N/A |

### Reference Implementations

| Pattern | Azure Reference (in workspace) | AWS Target | Notes |
|---------|----------------|------------|-------|
| Shared Project | `../Intent.Modules.Integration.IaC.Shared.AzureServiceBus/` | `../Intent.Modules.Integration.IaC.Shared.AwsSqs/` | Simpler (no topics) |
| IntegrationManager | `../Intent.Modules.Eventing.AzureServiceBus/` (shared project) | Create equivalent for SQS | Core aggregation logic |
| Stereotypes | `../Intent.Modules.Eventing.AzureServiceBus/Api/` | Create in `./Api/` | Simpler (no Queue/Topic enum) |
| EventBus | Azure module templates | Create in `./Templates/SqsEventBus/` | Publishing logic |
| Dispatcher | Azure module templates | Create in `./Templates/SqsMessageDispatcher/` | Routing logic |
| Configuration | Azure module templates | Create in `./Templates/SqsConfiguration/` | DI registration |

### Key Differences: Azure vs SQS

| Feature | Azure Service Bus | AWS SQS |
|---------|------------------|---------|
| Channel Types | Queue + Topic/Subscription | **Queue only** ✅ |
| Stereotype | Complex (Type enum) | **Simple (QueueName)** ✅ |
| Shared Project | 7 files | **5 files** ✅ |
| Configuration | Complex (subscription names) | **Simpler** ✅ |

**⭐ Good News:** AWS SQS module is architecturally **simpler** than Azure Service Bus due to lack of pub/sub topics.

### Implementation Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 1: SHARED PROJECT (START HERE - CRITICAL)                     │
├─────────────────────────────────────────────────────────────────────┤
│ Intent.Modules.Integration.IaC.Shared.AwsSqs/                       │
│   ├── SqsMethodType.cs (enum)                    [Simple]           │
│   ├── SqsItemBase.cs (abstract record)           [Simple]           │
│   ├── SqsMessage.cs (concrete record)            [Medium]           │
│   ├── IntegrationManager.cs (singleton)          [Complex]          │
│   └── .projitems (MSBuild file)                  [Simple]           │
│                                                                      │
│ Dependencies: Intent.Modelers.Eventing.Api, Intent.Engine           │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 2: API EXTENSIONS                                             │
├─────────────────────────────────────────────────────────────────────┤
│ Intent.Modules.Aws.Sqs/Api/                                         │
│   └── MessageModelStereotypeExtensions.cs        [Simple]           │
│       • AwsSqs stereotype class                                     │
│       • QueueName() / QueueUrl() properties                         │
│       • HasAwsSqs() / GetAwsSqs() helpers                           │
│                                                                      │
│ Dependencies: Shared Project (SqsMessage uses stereotypes)          │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 3: FACTORY EXTENSIONS                                         │
├─────────────────────────────────────────────────────────────────────┤
│ Intent.Modules.Aws.Sqs/FactoryExtensions/                           │
│   └── MetadataLoaderExtension.cs                 [Simple]           │
│       • Calls IntegrationManager.Initialize()                       │
│       • Runs before template generation                             │
│                                                                      │
│ Dependencies: Shared Project (IntegrationManager)                   │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 4: SIMPLE TEMPLATES (Options Classes)                         │
├─────────────────────────────────────────────────────────────────────┤
│ Intent.Modules.Aws.Sqs/Templates/                                   │
│   ├── SqsPublisherOptions/                       [Simple]           │
│   │   └── Maps MessageType → QueueUrl                               │
│   └── SqsSubscriptionOptions/                    [Simple]           │
│       └── Maps MessageType → Handler Delegate                       │
│                                                                      │
│ Dependencies: None (standalone classes)                             │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 5: CORE TEMPLATES (EventBus & Dispatcher)                     │
├─────────────────────────────────────────────────────────────────────┤
│ Intent.Modules.Aws.Sqs/Templates/                                   │
│   ├── ISqsMessageDispatcher/                     [Simple]           │
│   │   └── Interface definition                                      │
│   ├── SqsMessageDispatcher/                      [Medium]           │
│   │   └── Routes SQSMessages to typed handlers                      │
│   └── SqsEventBus/                                [Medium]           │
│       └── Publishes events to SQS queues                            │
│                                                                      │
│ Dependencies: Options templates (injected via DI)                   │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 6: CONFIGURATION TEMPLATE (Orchestrator)                      │
├─────────────────────────────────────────────────────────────────────┤
│ Intent.Modules.Aws.Sqs/Templates/                                   │
│   └── SqsConfiguration/                          [Complex]          │
│       • Registers IAmazonSQS client                                 │
│       • Registers IEventBus → SqsEventBus                           │
│       • Registers ISqsMessageDispatcher                             │
│       • Configures SqsPublisherOptions (metadata-driven)            │
│       • Configures SqsSubscriptionOptions (metadata-driven)         │
│       • Registers all event handlers                                │
│                                                                      │
│ Dependencies: ALL other templates + IntegrationManager              │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ PHASE 7: MODULE METADATA & INTEGRATION                              │
├─────────────────────────────────────────────────────────────────────┤
│   • Update .csproj (import shared project)                          │
│   • Update .imodspec (register templates, dependencies)             │
│   • Create NugetPackages.cs (AWS SDK versions)                      │
│   • Test in sample Intent Architect application                     │
└─────────────────────────────────────────────────────────────────────┘
```

**🎯 Key Insight:** Each phase depends on the previous one. You cannot skip ahead.

---

## 🎯 Objectives

### Primary Goals
1. ✅ Enable **event publishing** to AWS SQS queues via `IEventBus` abstraction
2. ✅ Provide **message routing** infrastructure via `ISqsMessageDispatcher`
3. ✅ Support **metadata-driven configuration** from Intent Architect designers
4. ✅ Maintain **hosting-agnostic** design (works with Lambda, console apps, web apps)
5. ✅ Follow **Azure Service Bus module patterns** for consistency

### Non-Goals (Future Work)
❌ Lambda function generation (goes in `Intent.Modules.AwsLambda.Sqs` bridge module)  
❌ HostedService implementation (user explicitly excluded)  
❌ IaC generation (queue creation, event source mappings)  
❌ Serverless.template integration  

---

## 🏗️ Architecture Context

### Two-Module Design Pattern

Intent Architect uses a **core + bridge** architecture for cloud eventing:

```
┌──────────────────────────────────────────────────┐
│  Intent.Modules.Aws.Sqs (CORE - THIS MODULE)     │
│  • SqsEventBus (publishing to SQS)               │
│  • SqsMessageDispatcher (routing logic)          │
│  • Configuration, Options classes                │
│  • Metadata and stereotypes                      │
│  • Hosting-agnostic infrastructure               │
└──────────────────────────────────────────────────┘
                      ↓ referenced by (future)
┌──────────────────────────────────────────────────┐
│  Intent.Modules.AwsLambda.Sqs (BRIDGE - FUTURE)  │
│  • Lambda consumer function generation           │
│  • [LambdaFunction] attribute usage              │
│  • SQSEvent handling                             │
│  • Event source mapping configuration            │
└──────────────────────────────────────────────────┘
```

### Reference Implementation

**Azure Pattern:**
- **Core:** `Intent.Modules.Eventing.AzureServiceBus` provides EventBus + Dispatcher + Configuration
- **Bridge:** `Intent.Modules.AzureFunctions.AzureServiceBus` generates Azure Function consumers

**SQS Pattern (To Implement):**
- **Core:** `Intent.Modules.Aws.Sqs` provides EventBus + Dispatcher + Configuration
- **Bridge:** `Intent.Modules.AwsLambda.Sqs` (future) generates Lambda consumers

### Working Reference Application (Conceptual)

**Note:** This is a reference implementation that demonstrates the expected output. The actual location may vary.

This application demonstrates:
- **SqsEventBus** - Publishing implementation to SQS queues
- **SqsMessageDispatcher** - Routing implementation for incoming messages
- **Lambda function** - Consuming SQS events with `[LambdaFunction]` attribute
- **TransactionScope** - Atomic operations pattern
- **LocalStack support** - Development/testing configuration

**Key Implementation Patterns:**
- `Infrastructure/Eventing/SqsEventBus.cs` - In-memory queue with batch sending
- `Infrastructure/Eventing/SqsMessageDispatcher.cs` - Message type routing via attributes
- `Infrastructure/Configuration/SqsConfiguration.cs` - DI registration and setup
- `Api/ClientsQueueConsumer.cs` - Lambda consumer example (future bridge module)

---

## � Shared Project Implementation (PRIORITY)

### Intent.Modules.Integration.IaC.Shared.AwsSqs

This shared project provides the metadata infrastructure that enables communication between the AWS SQS eventing module and potential future IaC/bridge modules. **This must be implemented FIRST** as it is a dependency for the main module templates.

#### Project Structure

```
Intent.Modules.Integration.IaC.Shared.AwsSqs/
├── Intent.Modules.Integration.IaC.Shared.AwsSqs.shproj
├── Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems
├── IntegrationManager.cs                  // Core orchestrator
├── SqsItemBase.cs                         // Abstract base for messages/commands
├── SqsMessage.cs                          // Message-specific implementation
├── SqsCommand.cs                          // Command-specific implementation (future)
├── SqsMethodType.cs                       // Publish/Subscribe enum
└── AwsHelper.cs                           // AWS naming utilities (optional)
```

#### Component Specifications

##### 1. Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems

**Purpose:** MSBuild project items file that defines the shared source files.

**Implementation:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <MSBuildAllProjects Condition="'$(MSBuildVersion)' == '' Or '$(MSBuildVersion)' &lt; '16.0'">$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
    <HasSharedItems>true</HasSharedItems>
    <SharedGUID>992bb1f9-cdd6-4a3d-bf90-6554358dd319</SharedGUID>
  </PropertyGroup>
  <PropertyGroup Label="Configuration">
    <Import_RootNamespace>Intent.Modules.Integration.IaC.Shared.AwsSqs</Import_RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="$(MSBuildThisFileDirectory)IntegrationManager.cs" />
    <Compile Include="$(MSBuildThisFileDirectory)SqsItemBase.cs" />
    <Compile Include="$(MSBuildThisFileDirectory)SqsMessage.cs" />
    <Compile Include="$(MSBuildThisFileDirectory)SqsMethodType.cs" />
    <!-- Optional, add if AWS naming utilities are needed -->
    <!-- <Compile Include="$(MSBuildThisFileDirectory)AwsHelper.cs" /> -->
  </ItemGroup>
</Project>
```

**Note:** The `.shproj` file already exists and is correct.

---

##### 2. SqsMethodType.cs

**Purpose:** Enum defining whether an item is published or subscribed.

**Namespace:** `Intent.Modules.Integration.IaC.Shared.AwsSqs`

**Implementation:**
```csharp
#nullable enable
namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal enum SqsMethodType
{
    Publish = 1,
    Subscribe = 2
}
```

**Complexity:** Simple

---

##### 3. SqsItemBase.cs

**Purpose:** Abstract base record for SQS messages and commands (future). Provides common properties and abstract methods for template generation.

**Namespace:** `Intent.Modules.Integration.IaC.Shared.AwsSqs`

**Implementation:**
```csharp
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal abstract record SqsItemBase
{
    public string ApplicationId { get; init; }
    public string ApplicationName { get; init; }
    public SqsMethodType MethodType { get; init; }
    public string QueueName { get; init; }
    public string QueueConfigurationName { get; init; }

    public abstract string GetModelTypeName(IntentTemplateBase template);
    public abstract string GetSubscriberTypeName<T>(IntentTemplateBase<T> template);
}
```

**Key Differences from Azure:**
- **No ChannelType**: SQS only has queues (no topics/subscriptions like Azure Service Bus)
- **Simpler naming**: Only `QueueName` and `QueueConfigurationName` (no subscription names)

**Complexity:** Simple

---

##### 4. SqsMessage.cs

**Purpose:** Concrete implementation of SqsItemBase for Integration Events (Messages).

**Namespace:** `Intent.Modules.Integration.IaC.Shared.AwsSqs`

**Dependencies:**
- `Intent.Modelers.Eventing.Api` (MessageModel)
- `Intent.Aws.Sqs.Api` (for stereotype extensions - to be created)

**Implementation:**
```csharp
using System;
using Intent.Aws.Sqs.Api;  // Will be created
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal record SqsMessage : SqsItemBase
{
    public SqsMessage(
        string applicationId, 
        string applicationName, 
        MessageModel messageModel, 
        SqsMethodType methodType)
    {
        ApplicationId = applicationId;
        ApplicationName = applicationName;
        MessageModel = messageModel;
        MethodType = methodType;
        QueueName = GetQueueName(messageModel);
        QueueConfigurationName = GetQueueConfigurationName(messageModel);
    }

    public MessageModel MessageModel { get; init; }

    private static string GetQueueName(MessageModel message)
    {
        // Check if AWS SQS stereotype is applied
        if (message.HasAwsSqs())
        {
            return message.GetAwsSqs().QueueName();
        }
        
        // Default naming convention: kebab-case, remove common suffixes
        var resolvedName = message.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToKebabCase();
        return resolvedName;
    }

    private static string GetQueueConfigurationName(MessageModel message)
    {
        const string prefix = "AwsSqs:";
        
        if (message.HasAwsSqs())
        {
            var name = message.GetAwsSqs().QueueName();
            return prefix + name.ToPascalCase();
        }

        // Default: PascalCase name
        var resolvedName = message.Name;
        resolvedName = resolvedName.RemoveSuffix("IntegrationEvent", "Event", "Message");
        resolvedName = resolvedName.ToPascalCase();
        return prefix + resolvedName;
    }

    public override string GetModelTypeName(IntentTemplateBase template)
    {
        return template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", MessageModel);
    }

    public override string GetSubscriberTypeName<T>(IntentTemplateBase<T> template)
    {
        return $"{template.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandlerInterface")}<{GetModelTypeName(template)}>";
    }
}
```

**Key Features:**
- Extracts queue name from `AWS SQS` stereotype or uses convention-based naming
- Generates configuration key (e.g., `"AwsSqs:ClientCreated"`)
- Provides type names for code generation via `GetTypeName()` helper

**Complexity:** Medium

---

##### 5. IntegrationManager.cs

**Purpose:** Singleton that aggregates all published/subscribed SQS messages across applications in the solution.

**Namespace:** `Intent.Modules.Integration.IaC.Shared.AwsSqs`

**Dependencies:**
- `Intent.Engine` (ISoftwareFactoryExecutionContext)
- `Intent.Modelers.Eventing.Api` (MessageModel)
- `Intent.Modelers.Services.Api` (Services)
- `Intent.Modelers.Services.EventInteractions` (IntegrationEventHandlerModel)

**Implementation:**
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal class IntegrationManager
{
    private static IntegrationManager? _instance;
    
    public static void Initialize(ISoftwareFactoryExecutionContext executionContext)
    {
        _instance = new IntegrationManager(executionContext);
    }

    public static IntegrationManager Instance
    {
        get
        {
            if (_instance is null)
            {
                throw new InvalidOperationException("AWS SQS Integration Manager not initialized.");
            }
            return _instance;
        }
    }
    
    private readonly List<MessageInfo> _publishedMessages;
    private readonly List<MessageInfo> _subscribedMessages;

    private IntegrationManager(ISoftwareFactoryExecutionContext executionContext)
    {
        var applications = executionContext.GetSolutionConfig()
            .GetApplicationReferences()
            .Select(app => executionContext.GetSolutionConfig().GetApplicationConfig(app.Id))
            .ToArray();

        const string awsSqsModule = "Intent.Aws.Sqs";
        
        // Collect published messages
        _publishedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == awsSqsModule))
            .SelectMany(app => executionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(app.Id)
                .Select(message => new MessageInfo(app.Id, app.Name, message, null)))
            .Distinct()
            .ToList();
        
        // Collect subscribed messages
        _subscribedMessages = applications
            .Where(app => app.Modules.Any(x => x.ModuleId == awsSqsModule))
            .SelectMany(app => executionContext.MetadataManager
                .Services(app.Id)
                .GetIntegrationEventHandlerModels()
                .SelectMany(handler => handler.IntegrationEventSubscriptions()
                    .Select(sub => new
                    {
                        Message = sub.Element.AsMessageModel(),
                        Handler = handler
                    }))
                .Select(sub => new MessageInfo(app.Id, app.Name, sub.Message, sub.Handler)))
            .Distinct()
            .ToList();
    }

    public IReadOnlyList<SqsMessage> GetPublishedSqsMessages(string applicationId)
    {
        return _publishedMessages
            .Where(p => p.ApplicationId == applicationId)
            .Select(s => new SqsMessage(
                s.ApplicationId, 
                s.ApplicationName, 
                s.Message, 
                SqsMethodType.Publish))
            .ToList();
    }
    
    public IReadOnlyList<SqsMessage> GetSubscribedSqsMessages(string applicationId)
    {
        return _subscribedMessages
            .Where(p => p.ApplicationId == applicationId)
            .DistinctBy(s => s.Message.Id)
            .Select(s => new SqsMessage(
                s.ApplicationId, 
                s.ApplicationName, 
                s.Message, 
                SqsMethodType.Subscribe))
            .ToList();
    }

    public IReadOnlyList<SqsItemBase> GetAggregatedPublishedSqsItems(string applicationId)
    {
        return GetPublishedSqsMessages(applicationId)
            .Cast<SqsItemBase>()
            .ToList();
    }
    
    public IReadOnlyList<SqsItemBase> GetAggregatedSubscribedSqsItems(string applicationId)
    {
        return GetSubscribedSqsMessages(applicationId)
            .Cast<SqsItemBase>()
            .ToList();
    }

    public IReadOnlyList<SqsItemBase> GetAggregatedSqsItems(string applicationId)
    {
        var duplicateCheckSet = new HashSet<string>();
        return GetAggregatedPublishedSqsItems(applicationId)
            .Concat(GetAggregatedSubscribedSqsItems(applicationId))
            .Where(p => duplicateCheckSet.Add($"{p.QueueName}.{p.MethodType}"))
            .ToList();
    }

    public IReadOnlyList<Subscription<SqsItemBase>> GetAggregatedSqsSubscriptions(string applicationId)
    {
        return _subscribedMessages
            .Where(message => message.ApplicationId == applicationId)
            .Select(message => new Subscription<SqsItemBase>(
                message.EventHandlerModel!,
                new SqsMessage(
                    applicationId, 
                    message.ApplicationName, 
                    message.Message, 
                    SqsMethodType.Subscribe)))
            .ToList();
    }

    public record Subscription<TSubscriptionItem>(
        IntegrationEventHandlerModel EventHandlerModel, 
        TSubscriptionItem SubscriptionItem);
    
    private record MessageInfo(
        string ApplicationId, 
        string ApplicationName, 
        MessageModel Message, 
        IntegrationEventHandlerModel? EventHandlerModel);
}
```

**Key Responsibilities:**
1. **Initialization**: Scans all applications in solution for AWS SQS module
2. **Published Messages**: Collects messages marked for publishing
3. **Subscribed Messages**: Collects messages handled by IntegrationEventHandlers
4. **Aggregation**: Provides unified view of all SQS items for code generation
5. **Deduplication**: Prevents duplicate queue configurations

**Complexity:** Complex

---

##### 6. AwsHelper.cs (Optional)

**Purpose:** Utility methods for AWS resource naming (queue names, etc.).

**When to implement:** Only if you need:
- Queue name length truncation (SQS has 80 char limit)
- Character sanitization (alphanumeric + hyphens only)
- Deterministic hashing for long names

**Namespace:** `Intent.Modules.Integration.IaC.Shared.AwsSqs`

**Note:** Can be deferred to later if naming conventions are simple. Azure uses this for storage account naming constraints.

---

#### Integration with Main Module

Once the shared project is implemented, the main `Intent.Modules.Aws.Sqs` module must:

1. **Import the Shared Project** (in `.csproj`):
```xml
<Import Project="..\Intent.Modules.Integration.IaC.Shared.AwsSqs\Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems" Label="Shared" />
```

2. **Initialize IntegrationManager** (in `MetadataLoaderExtension.cs`):
```csharp
public override void Execute()
{
    IntegrationManager.Initialize(ExecutionContext);
}
```

3. **Use IntegrationManager in Templates** (e.g., `SqsConfiguration.cs`):
```csharp
var publishers = IntegrationManager.Instance.GetAggregatedPublishedSqsItems(ExecutionContext.GetApplicationConfig().Id);
var subscribers = IntegrationManager.Instance.GetAggregatedSubscribedSqsItems(ExecutionContext.GetApplicationConfig().Id);
```

---

## �📦 Components to Implement (Main Module)

### 1. Templates (7 Total)

| # | Template Name | Purpose | Output Location | Complexity |
|---|---------------|---------|-----------------|------------|
| 1 | **SqsPublisherOptions** | Maps message types to queue URLs | `Configuration/` | Simple |
| 2 | **SqsSubscriptionOptions** | Maps message types to handler types | `Configuration/` | Simple |
| 3 | **ISqsMessageDispatcher** | Dispatcher interface | `Eventing/` | Simple |
| 4 | **SqsMessageDispatcher** | Routes SQS messages to typed handlers | `Eventing/` | Medium |
| 5 | **SqsEventBus** | Publishes events to SQS | `Eventing/` | Medium |
| 6 | **SqsConfiguration** | DI registration and setup | `Configuration/` | Complex |
| 7 | **TemplateExtensions** | Helper extension methods | N/A | Simple |

### 2. API Extensions (1 Total)

| # | Component | Purpose |
|---|-----------|---------|
| 1 | **MessageModelStereotypeExtensions** | Provides AWS SQS stereotype for message models |

**File Location:** `Api/MessageModelStereotypeExtensions.cs`

**Stereotype Definition:**
- **ID:** Generate new GUID
- **Name:** "AWS SQS"
- **Properties:**
  - `QueueName` (string) - The name of the SQS queue (required)
  - `QueueUrl` (string, optional) - Full queue URL (can be in config instead)

**Implementation:**
```csharp
using System;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Aws.Sqs.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static AwsSqs GetAwsSqs(this MessageModel model)
        {
            var stereotype = model.GetStereotype(AwsSqs.DefinitionId);
            return stereotype != null ? new AwsSqs(stereotype) : null;
        }

        public static bool HasAwsSqs(this MessageModel model)
        {
            return model.HasStereotype(AwsSqs.DefinitionId);
        }

        public static bool TryGetAwsSqs(this MessageModel model, out AwsSqs stereotype)
        {
            if (!HasAwsSqs(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AwsSqs(model.GetStereotype(AwsSqs.DefinitionId));
            return true;
        }

        public class AwsSqs
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "[GENERATE-NEW-GUID]";

            public AwsSqs(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }

            public string QueueUrl()
            {
                return _stereotype.GetProperty<string>("Queue URL");
            }
        }
    }
}
```

**Key Differences from Azure:**
- **No Type property**: SQS only supports queues (no topics)
- **Simpler structure**: Only queue name and optional URL
- **Single channel type**: No enum for Queue/Topic selection

**Complexity:** Simple  
**Reference:** `Intent.Modules.Eventing.AzureServiceBus\Api\MessageModelStereotypeExtensions.cs`

### 3. Factory Extensions (1 Total)

| # | Extension | Purpose |
|---|-----------|---------|
| 1 | **MetadataLoaderExtension** | Initializes IntegrationManager, loads published/subscribed message metadata |

### 4. Infrastructure Files

| File | Purpose |
|------|---------|
| **NugetPackages.cs** | NuGet package definitions and version management |
| **Intent.Aws.Sqs.imodspec** | Module metadata, dependencies, template registrations |

---

## 🔧 Detailed Component Specifications

### Template 1: SqsPublisherOptions

**Purpose:** Configuration class that maps message types to queue URLs for publishing.

**Template Type:** String interpolation

**Generated Code Structure:**
```csharp
public class SqsPublisherOptions
{
    private readonly List<PublisherEntry> _entries = [];
    
    public IReadOnlyList<PublisherEntry> Entries => _entries;
    
    public void AddQueue<TMessage>(string queueUrl)
    {
        ArgumentNullException.ThrowIfNull(queueUrl);
        _entries.Add(new PublisherEntry(typeof(TMessage), queueUrl));
    }
}

public record PublisherEntry(Type MessageType, string QueueUrl);
```

**Template Role:** `Infrastructure.Eventing.SqsPublisherOptions`  
**Output Location:** `Configuration/`

---

### Template 2: SqsSubscriptionOptions

**Purpose:** Configuration class that maps message types to handler delegates for consumption.

**Template Type:** String interpolation

**Generated Code Structure:**
```csharp
public class SqsSubscriptionOptions
{
    private readonly List<SubscriptionEntry> _entries = [];
    
    public IReadOnlyList<SubscriptionEntry> Entries => _entries;
    
    public void Add<TMessage, THandler>()
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        _entries.Add(new SubscriptionEntry(
            typeof(TMessage), 
            SqsMessageDispatcher.InvokeDispatchHandler<TMessage, THandler>));
    }
}

public delegate Task DispatchHandler(
    IServiceProvider serviceProvider, 
    SQSEvent.SQSMessage sqsMessage, 
    CancellationToken cancellationToken);

public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync);
```

**Template Role:** `Infrastructure.Eventing.SqsSubscriptionOptions`  
**Output Location:** `Configuration/`

---

### Template 3: ISqsMessageDispatcher

**Purpose:** Interface for dispatching SQS messages to typed handlers.

**Template Type:** CSharpFileBuilder

**Generated Code Structure:**
```csharp
public interface ISqsMessageDispatcher
{
    Task DispatchAsync(
        IServiceProvider scopedServiceProvider,
        SQSEvent.SQSMessage sqsMessage,
        CancellationToken cancellationToken);
}
```

**Template Role:** `Infrastructure.Eventing.ISqsMessageDispatcher`  
**Output Location:** `Eventing/`

---

### Template 4: SqsMessageDispatcher

**Purpose:** Routes incoming SQS messages to the appropriate typed handler based on MessageType attribute.

**Template Type:** CSharpFileBuilder (medium complexity)

**Key Features:**
- Dictionary lookup: `MessageType.FullName → DispatchHandler`
- Extracts `MessageType` from `SQSMessage.MessageAttributes["MessageType"]`
- Deserializes JSON body using `System.Text.Json`
- Invokes handler via dependency injection

**Generated Code Structure:**
```csharp
public class SqsMessageDispatcher : ISqsMessageDispatcher
{
    private readonly Dictionary<string, DispatchHandler> _handlers;

    public SqsMessageDispatcher(IOptions<SqsSubscriptionOptions> options)
    {
        _handlers = options.Value.Entries.ToDictionary(
            k => k.MessageType.FullName!, 
            v => v.HandlerAsync);
    }

    public async Task DispatchAsync(
        IServiceProvider scopedServiceProvider,
        SQSEvent.SQSMessage sqsMessage,
        CancellationToken cancellationToken)
    {
        var messageTypeName = sqsMessage.MessageAttributes
            .TryGetValue("MessageType", out var messageTypeAttr)
            ? messageTypeAttr.StringValue
            : throw new Exception("MessageType attribute is missing");

        if (_handlers.TryGetValue(messageTypeName, out var handlerAsync))
        {
            await handlerAsync(scopedServiceProvider, sqsMessage, cancellationToken);
        }
    }

    public static async Task InvokeDispatchHandler<TMessage, THandler>(
        IServiceProvider serviceProvider,
        SQSEvent.SQSMessage sqsMessage,
        CancellationToken cancellationToken)
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        var messageObj = JsonSerializer.Deserialize<TMessage>(sqsMessage.Body)!;
        var handler = serviceProvider.GetRequiredService<THandler>();
        await handler.HandleAsync(messageObj, cancellationToken);
    }
}
```

**Template Role:** `Infrastructure.Eventing.SqsMessageDispatcher`  
**Output Location:** `Eventing/`

---

### Template 5: SqsEventBus

**Purpose:** Implements IEventBus to publish events to AWS SQS queues.

**Template Type:** CSharpFileBuilder (medium complexity)

**Key Features:**
- In-memory queue for batching messages
- `Publish<T>(T message)` queues messages
- `FlushAllAsync()` sends all queued messages to SQS
- Groups messages by queue URL for efficiency
- Sets `MessageAttributes["MessageType"]` for routing
- Uses `IAmazonSQS.SendMessageAsync()`

**Generated Code Structure:**
```csharp
public class SqsEventBus : IEventBus
{
    private readonly IAmazonSQS _sqsClient;
    private readonly List<MessageEntry> _messageQueue = [];
    private readonly Dictionary<string, PublisherEntry> _lookup;

    public SqsEventBus(
        IOptions<SqsPublisherOptions> options,
        IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;
        _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);
    }

    public void Publish<T>(T message) where T : class
    {
        ValidateMessage(message);
        _messageQueue.Add(new MessageEntry(message));
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        if (_messageQueue.Count == 0) return;

        var groupedMessages = _messageQueue.GroupBy(entry =>
        {
            var publisherEntry = _lookup[entry.Message.GetType().FullName!];
            return publisherEntry.QueueUrl;
        });

        foreach (var group in groupedMessages)
        {
            foreach (var entry in group)
            {
                var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                var sqsMessage = CreateSqsMessage(entry, publisherEntry);
                await _sqsClient.SendMessageAsync(sqsMessage, cancellationToken);
            }
        }
        _messageQueue.Clear();
    }

    private void ValidateMessage(object message)
    {
        if (!_lookup.TryGetValue(message.GetType().FullName!, out _))
        {
            throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");
        }
    }

    private static SendMessageRequest CreateSqsMessage(
        MessageEntry messageEntry, 
        PublisherEntry publisherEntry)
    {
        var messageBody = JsonSerializer.Serialize(messageEntry.Message);
        var sqsMessage = new SendMessageRequest
        {
            QueueUrl = publisherEntry.QueueUrl,
            MessageBody = messageBody,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["MessageType"] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = messageEntry.Message.GetType().FullName!
                }
            }
        };
        return sqsMessage;
    }
}

internal record MessageEntry(object Message);
```

**Template Role:** `Infrastructure.Eventing.SqsEventBus`  
**Output Location:** `Eventing/`

---

### Template 6: SqsConfiguration

**Purpose:** Static configuration class for DI registration and setup.

**Template Type:** CSharpFileBuilder (complex - metadata-driven)

**Key Features:**
- Metadata-driven: Reads published/subscribed messages from IntegrationManager
- Registers `IAmazonSQS` client (with LocalStack detection)
- Registers `IEventBus → SqsEventBus`
- Registers `ISqsMessageDispatcher → SqsMessageDispatcher`
- Configures `SqsPublisherOptions` with queue URLs
- Configures `SqsSubscriptionOptions` with handlers
- Registers event handlers

**Generated Code Structure:**
```csharp
public static class SqsConfiguration
{
    public static IServiceCollection ConfigureSqs(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register SQS client (with LocalStack support)
        var serviceUrl = configuration["AWS:ServiceURL"];
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            // LocalStack or custom endpoint
            services.AddSingleton<IAmazonSQS>(sp =>
            {
                var sqsConfig = new AmazonSQSConfig
                {
                    ServiceURL = serviceUrl,
                    AuthenticationRegion = configuration["AWS:Region"] ?? "us-east-1"
                };
                return new AmazonSQSClient(
                    new BasicAWSCredentials("test", "test"),
                    sqsConfig
                );
            });
        }
        else
        {
            // Production AWS
            services.AddAWSService<IAmazonSQS>();
        }

        // Register event bus
        services.AddScoped<IEventBus, SqsEventBus>();
        
        // Register dispatcher
        services.AddSingleton<SqsMessageDispatcher>();
        services.AddSingleton<ISqsMessageDispatcher, SqsMessageDispatcher>(
            sp => sp.GetRequiredService<SqsMessageDispatcher>());

        // Configure publisher options (metadata-driven)
        services.Configure<SqsPublisherOptions>(options =>
        {
            // For each published message from metadata:
            options.AddQueue<ClientCreatedEvent>(
                configuration["Sqs:Queues:ClientCreatedEvent:QueueUrl"]!);
            // ... more publishers
        });

        // Register event handlers (metadata-driven)
        // services.AddScoped<IIntegrationEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();

        // Configure subscription options (metadata-driven)
        services.Configure<SqsSubscriptionOptions>(options =>
        {
            // For each subscribed message from metadata:
            options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>();
            // ... more subscriptions
        });

        return services;
    }
}
```

**Template Role:** `Infrastructure.DependencyInjection.Sqs`  
**Output Location:** `Configuration/`

**Metadata Integration:**
- Uses `IntegrationManager.Instance.GetAggregatedPublishedSqsItems()`
- Uses `IntegrationManager.Instance.GetAggregatedSubscribedSqsItems()`
- Generates configuration based on message models with SQS stereotype

---

### API Extension: MessageModelStereotypeExtensions

**Purpose:** Provides access to AWS SQS stereotype properties on message models.

**File Location:** `Api/MessageModelStereotypeExtensions.cs`

**Stereotype Definition:**
- **ID:** New GUID (e.g., `"AWS-SQS-STEREOTYPE-ID"`)
- **Name:** "AWS SQS"
- **Properties:**
  - `QueueName` (string) - The name of the SQS queue
  - `QueueUrl` (string, optional) - Full queue URL (can be in config instead)

**Generated Code Structure:**
```csharp
public static class MessageModelStereotypeExtensions
{
    public static AwsSqs GetAwsSqs(this MessageModel model)
    {
        var stereotype = model.GetStereotype(AwsSqs.DefinitionId);
        return stereotype != null ? new AwsSqs(stereotype) : null;
    }

    public static bool HasAwsSqs(this MessageModel model)
    {
        return model.HasStereotype(AwsSqs.DefinitionId);
    }

    public static bool TryGetAwsSqs(this MessageModel model, out AwsSqs stereotype)
    {
        if (!HasAwsSqs(model))
        {
            stereotype = null;
            return false;
        }
        stereotype = new AwsSqs(model.GetStereotype(AwsSqs.DefinitionId));
        return true;
    }

    public class AwsSqs
    {
        private IStereotype _stereotype;
        public const string DefinitionId = "AWS-SQS-STEREOTYPE-ID"; // Generate new GUID

        public AwsSqs(IStereotype stereotype)
        {
            _stereotype = stereotype;
        }

        public string Name => _stereotype.Name;

        public string QueueName()
        {
            return _stereotype.GetProperty<string>("Queue Name");
        }

        public string QueueUrl()
        {
            return _stereotype.GetProperty<string>("Queue URL");
        }
    }
}
```

---

### Factory Extension: MetadataLoaderExtension

**Purpose:** Initializes integration metadata for SQS message aggregation.

**File Location:** `FactoryExtensions/MetadataLoaderExtension.cs`

**Responsibilities:**
- Initialize `IntegrationManager` (if using shared infrastructure)
- Scan for message models with AWS SQS stereotype
- Build aggregated view of published messages
- Build aggregated view of subscribed messages
- Make metadata available to templates during code generation

**Generated Code Structure:**
```csharp
public class MetadataLoaderExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Aws.Sqs.MetadataLoaderExtension";

    public override int Order => 0;

    protected override void OnAfterMetadataLoad(IApplication application)
    {
        IntegrationManager.Initialize(application);
        // Additional SQS-specific metadata loading if needed
    }
}
```

**Notes:**
- May need to create `IntegrationManager` for SQS or reuse existing infrastructure
- Aggregates published/subscribed messages for configuration generation
- Similar pattern to Azure's `MetadataLoaderExtension`

---

## 📚 NuGet Package Dependencies

### Required Packages

| Package | Purpose | Version Strategy |
|---------|---------|------------------|
| **AWSSDK.SQS** | Core SQS client SDK | Framework-based versioning |
| **AWSSDK.Extensions.NETCore.Setup** | DI integration (`AddAWSService<>()`) | Framework-based versioning |
| **Amazon.Lambda.SQSEvents** | SQSEvent type definitions | Framework-based versioning |
| **Amazon.Lambda.Core** | Lambda core types (for ILambdaContext) | Framework-based versioning |

### NugetPackages.cs Structure

```csharp
public class NugetPackages : INugetPackages
{
    public const string AwsSdkSqsPackageName = "AWSSDK.SQS";
    public const string AwsSdkExtensionsNetCoreSetupPackageName = "AWSSDK.Extensions.NETCore.Setup";
    public const string AmazonLambdaSqsEventsPackageName = "Amazon.Lambda.SQSEvents";
    public const string AmazonLambdaCorePackageName = "Amazon.Lambda.Core";

    public void RegisterPackages()
    {
        // AWSSDK.SQS
        NugetRegistry.Register(AwsSdkSqsPackageName,
            (framework) => framework.Major switch
            {
                >= 8 => new PackageVersion("3.7.x.x"),
                >= 6 => new PackageVersion("3.7.x.x"),
                _ => throw new Exception($"Unsupported Framework '{framework.Major}'")
            });

        // AWSSDK.Extensions.NETCore.Setup
        NugetRegistry.Register(AwsSdkExtensionsNetCoreSetupPackageName,
            (framework) => framework.Major switch
            {
                >= 6 => new PackageVersion("3.7.x"),
                _ => throw new Exception($"Unsupported Framework '{framework.Major}'")
            });

        // Amazon.Lambda.SQSEvents
        NugetRegistry.Register(AmazonLambdaSqsEventsPackageName,
            (framework) => framework.Major switch
            {
                >= 6 => new PackageVersion("2.2.x"),
                _ => throw new Exception($"Unsupported Framework '{framework.Major}'")
            });

        // Amazon.Lambda.Core
        NugetRegistry.Register(AmazonLambdaCorePackageName,
            (framework) => framework.Major switch
            {
                >= 6 => new PackageVersion("2.2.x"),
                _ => throw new Exception($"Unsupported Framework '{framework.Major}'")
            });
    }

    public static NugetPackageInfo AwsSdkSqs(IOutputTarget outputTarget) 
        => NugetRegistry.GetVersion(AwsSdkSqsPackageName, outputTarget.GetMaxNetAppVersion());

    public static NugetPackageInfo AwsSdkExtensionsNetCoreSetup(IOutputTarget outputTarget) 
        => NugetRegistry.GetVersion(AwsSdkExtensionsNetCoreSetupPackageName, outputTarget.GetMaxNetAppVersion());

    public static NugetPackageInfo AmazonLambdaSqsEvents(IOutputTarget outputTarget) 
        => NugetRegistry.GetVersion(AmazonLambdaSqsEventsPackageName, outputTarget.GetMaxNetAppVersion());

    public static NugetPackageInfo AmazonLambdaCore(IOutputTarget outputTarget) 
        => NugetRegistry.GetVersion(AmazonLambdaCorePackageName, outputTarget.GetMaxNetAppVersion());
}
```

**Note:** Version numbers should be verified against latest stable AWS SDK releases.

---

## 📝 Module Metadata (.imodspec)

### Intent.Aws.Sqs.imodspec

**Key Sections to Update:**

#### 1. Module Information
```xml
<id>Intent.Aws.Sqs</id>
<version>1.0.0-pre.0</version>
<summary>Provides AWS SQS eventing infrastructure for publishing and consuming messages.</summary>
<description>Core module for AWS SQS integration. Provides IEventBus implementation, message dispatcher, and configuration for SQS-based eventing patterns. Lambda function generation is handled by the companion Intent.AwsLambda.Sqs module.</description>
<authors>Intent Architect</authors>
<iconUrl><!-- AWS SQS icon base64 --></iconUrl>
```

#### 2. Template Registrations
```xml
<templates>
    <template id="Intent.Aws.Sqs.SqsEventBus" externalReference="[GUID]">
        <role>Infrastructure.Eventing.SqsEventBus</role>
        <location>Eventing</location>
    </template>
    <template id="Intent.Aws.Sqs.SqsMessageDispatcher" externalReference="[GUID]">
        <role>Infrastructure.Eventing.SqsMessageDispatcher</role>
        <location>Eventing</location>
    </template>
    <template id="Intent.Aws.Sqs.ISqsMessageDispatcher" externalReference="[GUID]">
        <role>Infrastructure.Eventing.ISqsMessageDispatcher</role>
        <location>Eventing</location>
    </template>
    <template id="Intent.Aws.Sqs.SqsConfiguration" externalReference="[GUID]">
        <role>Infrastructure.DependencyInjection.Sqs</role>
        <location>Configuration</location>
    </template>
    <template id="Intent.Aws.Sqs.SqsPublisherOptions" externalReference="[GUID]">
        <role>Infrastructure.Eventing.SqsPublisherOptions</role>
        <location>Configuration</location>
    </template>
    <template id="Intent.Aws.Sqs.SqsSubscriptionOptions" externalReference="[GUID]">
        <role>Infrastructure.Eventing.SqsSubscriptionOptions</role>
        <location>Configuration</location>
    </template>
</templates>
```

#### 3. Factory Extensions
```xml
<factoryExtensions>
    <factoryExtension id="Intent.Aws.Sqs.MetadataLoaderExtension" externalReference="[GUID]" />
</factoryExtensions>
```

#### 4. Dependencies
```xml
<dependencies>
    <dependency id="Intent.Common" version="3.7.2" />
    <dependency id="Intent.Common.CSharp" version="3.8.1" />
    <dependency id="Intent.Common.Types" version="3.4.0" />
    <dependency id="Intent.Eventing.Contracts" version="5.2.0" />
    <dependency id="Intent.Modelers.Eventing" version="6.0.2" />
    <dependency id="Intent.Modelers.Services" version="3.9.3" />
    <dependency id="Intent.Modelers.Services.EventInteractions" version="1.2.1" />
    <dependency id="Intent.OutputManager.RoslynWeaver" version="4.9.10" />
</dependencies>
```

#### 5. Interoperability (Future)
```xml
<interoperability>
    <detect id="Intent.AwsLambda">
        <install>
            <package id="Intent.AwsLambda.Sqs" version="1.0.0" />
        </install>
    </detect>
</interoperability>
```

---

## 🗂️ Project Structure

### Final Folder Layout

```
Intent.Modules.Aws.Sqs/
├── .gitignore
├── .intent/
├── Intent.Aws.Sqs.imodspec
├── Intent.Modules.Aws.Sqs.csproj
├── Intent.Modules.Aws.Sqs.application.config
├── modules.config
├── NugetPackages.cs
├── IMPLEMENTATION_PLAN.md (this file)
├── README.md
├── release-notes.md
├── bin/
├── obj/
├── Intent.Metadata/
│   └── Module Builder/
├── Templates/
│   ├── SqsEventBus/
│   │   ├── SqsEventBusTemplatePartial.cs
│   │   └── SqsEventBusTemplateRegistration.cs
│   ├── SqsMessageDispatcher/
│   │   ├── SqsMessageDispatcherTemplatePartial.cs
│   │   └── SqsMessageDispatcherTemplateRegistration.cs
│   ├── SqsMessageDispatcherInterface/
│   │   ├── SqsMessageDispatcherInterfaceTemplatePartial.cs
│   │   └── SqsMessageDispatcherInterfaceTemplateRegistration.cs
│   ├── SqsConfiguration/
│   │   ├── SqsConfigurationTemplatePartial.cs
│   │   └── SqsConfigurationTemplateRegistration.cs
│   ├── SqsPublisherOptions/
│   │   ├── SqsPublisherOptionsTemplatePartial.cs
│   │   ├── SqsPublisherOptionsTemplate.cs
│   │   └── SqsPublisherOptionsTemplateRegistration.cs
│   ├── SqsSubscriptionOptions/
│   │   ├── SqsSubscriptionOptionsTemplatePartial.cs
│   │   ├── SqsSubscriptionOptionsTemplate.cs
│   │   └── SqsSubscriptionOptionsTemplateRegistration.cs
│   └── TemplateExtensions.cs
├── Api/
│   └── MessageModelStereotypeExtensions.cs
└── FactoryExtensions/
    └── MetadataLoaderExtension.cs
```

---

## ✅ Implementation Checklist

### Phase 1: Foundation & Setup
- [ ] Create `NugetPackages.cs` with AWS SDK package definitions
- [ ] Update `Intent.Aws.Sqs.imodspec` with module metadata
- [ ] Create folder structure (`Templates/`, `Api/`, `FactoryExtensions/`)
- [ ] Create `TemplateExtensions.cs` helper file

### Phase 2: Simple Data Templates
- [ ] Implement `SqsPublisherOptions` template
  - [ ] Create `SqsPublisherOptionsTemplatePartial.cs`
  - [ ] Create `SqsPublisherOptionsTemplate.cs` (string interpolation)
  - [ ] Create `SqsPublisherOptionsTemplateRegistration.cs`
  - [ ] Add to `.imodspec`
- [ ] Implement `SqsSubscriptionOptions` template
  - [ ] Create `SqsSubscriptionOptionsTemplatePartial.cs`
  - [ ] Create `SqsSubscriptionOptionsTemplate.cs` (string interpolation)
  - [ ] Create `SqsSubscriptionOptionsTemplateRegistration.cs`
  - [ ] Add to `.imodspec`

### Phase 3: Core Infrastructure Templates
- [ ] Implement `ISqsMessageDispatcher` interface template
  - [ ] Create `SqsMessageDispatcherInterfaceTemplatePartial.cs` (CSharpFileBuilder)
  - [ ] Create `SqsMessageDispatcherInterfaceTemplateRegistration.cs`
  - [ ] Add to `.imodspec`
- [ ] Implement `SqsMessageDispatcher` template
  - [ ] Create `SqsMessageDispatcherTemplatePartial.cs` (CSharpFileBuilder)
  - [ ] Create `SqsMessageDispatcherTemplateRegistration.cs`
  - [ ] Implement dictionary lookup logic
  - [ ] Implement `InvokeDispatchHandler<TMessage, THandler>()` static method
  - [ ] Add to `.imodspec`
- [ ] Implement `SqsEventBus` template
  - [ ] Create `SqsEventBusTemplatePartial.cs` (CSharpFileBuilder)
  - [ ] Create `SqsEventBusTemplateRegistration.cs`
  - [ ] Implement in-memory queue
  - [ ] Implement `Publish<T>()` method
  - [ ] Implement `FlushAllAsync()` with SQS send logic
  - [ ] Add to `.imodspec`

### Phase 4: Configuration Template
- [ ] Implement `SqsConfiguration` template
  - [ ] Create `SqsConfigurationTemplatePartial.cs` (CSharpFileBuilder)
  - [ ] Create `SqsConfigurationTemplateRegistration.cs`
  - [ ] Implement `ConfigureSqs()` extension method
  - [ ] Add IAmazonSQS registration with LocalStack detection
  - [ ] Add IEventBus → SqsEventBus registration
  - [ ] Add ISqsMessageDispatcher → SqsMessageDispatcher registration
  - [ ] Add metadata-driven publisher options configuration
  - [ ] Add metadata-driven subscription options configuration
  - [ ] Add metadata-driven handler registrations
  - [ ] Add to `.imodspec`

### Phase 5: Metadata & API Extensions
- [ ] Implement `MessageModelStereotypeExtensions`
  - [ ] Generate new stereotype GUID
  - [ ] Define stereotype properties (QueueName, QueueUrl)
  - [ ] Create extension methods (GetAwsSqs, HasAwsSqs, TryGetAwsSqs)
  - [ ] Create AwsSqs class with property accessors
- [ ] Implement `MetadataLoaderExtension`
  - [ ] Create factory extension class
  - [ ] Implement `OnAfterMetadataLoad()` override
  - [ ] Initialize IntegrationManager (or create SQS-specific manager)
  - [ ] Add to `.imodspec`

### Phase 6: Integration & Testing
- [ ] Update `.imodspec` with all template registrations
- [ ] Update `.imodspec` with all factory extension registrations
- [ ] Update `.imodspec` with dependencies
- [ ] Build module project
- [ ] Test code generation in Intent Architect
- [ ] Compare generated output with SqsSample reference
- [ ] Adjust templates as needed
- [ ] Create documentation (README.md)
- [ ] Create release notes

### Phase 7: Documentation
- [ ] Create `README.md` with usage instructions
- [ ] Update `release-notes.md`
- [ ] Document configuration requirements
- [ ] Document stereotype usage
- [ ] Document LocalStack support

---

## 🎨 Key Design Decisions

### 1. Core Module Scope
**Decision:** Core module provides infrastructure only, NOT Lambda function generation.

**Rationale:**
- Follows Azure Service Bus pattern (core + bridge architecture)
- Maintains separation of concerns
- Allows non-Lambda usage scenarios
- Lambda generation goes in `Intent.AwsLambda.Sqs` bridge module

### 2. Dispatcher in Core Module
**Decision:** `ISqsMessageDispatcher` and `SqsMessageDispatcher` live in core module.

**Rationale:**
- Routing logic is shared between Lambda and non-Lambda consumers
- Bridge module can simply call dispatcher
- Reduces code duplication
- Matches Azure pattern

### 3. Include Amazon.Lambda.SQSEvents
**Decision:** Include `Amazon.Lambda.SQSEvents` NuGet package in core module.

**Rationale:**
- Dispatcher interface needs `SQSEvent.SQSMessage` type
- Type definition doesn't create Lambda coupling
- Avoids type duplication
- Simplifies bridge module implementation

### 4. Metadata-Driven Configuration
**Decision:** Use IntegrationManager to aggregate published/subscribed messages.

**Rationale:**
- Follows Azure Service Bus pattern
- Reduces manual configuration
- Automatically generates queue URL mappings
- Integrates with Intent Architect designers

### 5. LocalStack Support
**Decision:** Detect `AWS:ServiceURL` configuration to enable LocalStack.

**Rationale:**
- Enables local development without AWS account
- Matches SqsSample reference implementation
- Common pattern in AWS development
- Transparent to application code

### 6. Simple Stereotype
**Decision:** Single "AWS SQS" stereotype with QueueName and QueueUrl properties.

**Rationale:**
- SQS only has queues (unlike Azure's Queue + Topic/Subscription)
- Simpler model = easier to use
- QueueUrl optional (can be in config instead)

### 7. Transaction Scope Strategy
**Decision:** Core module doesn't enforce transaction strategy.

**Rationale:**
- Transaction handling is consumer-specific
- Lambda consumer (bridge module) uses TransactionScope
- Other consumers may have different needs
- Flexibility for different hosting models

### 8. Template Complexity Strategy
**Decision:** Mixed approach - string interpolation for simple classes, CSharpFileBuilder for complex logic.

**Rationale:**
- String interpolation faster for static structures (Options classes)
- CSharpFileBuilder better for dynamic/metadata-driven code (Configuration, EventBus)
- Balances maintainability and flexibility
- Follows Azure module patterns

---

## 🔗 Integration Points

### Upstream Dependencies
- **Intent.Eventing.Contracts** - Provides `IEventBus`, `IIntegrationEventHandler<T>` interfaces
- **Intent.Modelers.Eventing** - Provides message model metadata
- **Intent.Modelers.Services.EventInteractions** - Provides publish/subscribe modeling
- **Intent.Common.CSharp** - Provides CSharpFileBuilder infrastructure

### Downstream Consumers
- **Intent.AwsLambda.Sqs** (future) - Lambda consumer generation bridge module
- User applications - Direct usage in non-Lambda scenarios

### Designer Integration
- Message models with AWS SQS stereotype
- Service models with event publish/subscribe interactions
- Application settings for queue URLs

---

## 🧪 Testing Strategy

### Unit Testing
- Templates generate valid C# code
- Stereotype extensions parse correctly
- NuGet package versions resolve

### Integration Testing
- Generate code in sample Intent Architect application
- Compare output with SqsSample reference
- Verify DI registration works
- Verify publishing to SQS works
- Verify message routing works

---

## 📋 Implementation Checklist

### Phase 1: Shared Project Infrastructure (CRITICAL - DO FIRST)

- [ ] **1.1** Create `SqsMethodType.cs` enum (Publish/Subscribe)
- [ ] **1.2** Create `SqsItemBase.cs` abstract record
- [ ] **1.3** Create `SqsMessage.cs` concrete implementation
- [ ] **1.4** Create `IntegrationManager.cs` singleton
- [ ] **1.5** Update `Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems` to include all files
- [ ] **1.6** Verify shared project compiles independently

### Phase 2: API Extensions

- [ ] **2.1** Create `Api/` folder in main module
- [ ] **2.2** Create `MessageModelStereotypeExtensions.cs`
- [ ] **2.3** Generate new GUID for `AwsSqs.DefinitionId`
- [ ] **2.4** Test stereotype detection in unit tests

### Phase 3: Factory Extensions

- [ ] **3.1** Create `FactoryExtensions/` folder
- [ ] **3.2** Create `MetadataLoaderExtension.cs`
- [ ] **3.3** Implement `Initialize(IntegrationManager)` call
- [ ] **3.4** Register factory extension in `.imodspec`

### Phase 4: Core Templates

- [ ] **4.1** Create `Templates/SqsPublisherOptions/` folder
  - [ ] Create template registration file
  - [ ] Implement string interpolation template
  - [ ] Add `PublisherEntry` record
  
- [ ] **4.2** Create `Templates/SqsSubscriptionOptions/` folder
  - [ ] Create template registration file
  - [ ] Implement string interpolation template
  - [ ] Add `DispatchHandler` delegate and `SubscriptionEntry` record

- [ ] **4.3** Create `Templates/ISqsMessageDispatcher/` folder
  - [ ] Create interface template
  - [ ] Define `DispatchAsync` signature

- [ ] **4.4** Create `Templates/SqsMessageDispatcher/` folder
  - [ ] Implement CSharpFileBuilder template
  - [ ] Add message type dictionary lookup
  - [ ] Add JSON deserialization logic
  - [ ] Add handler invocation via DI

- [ ] **4.5** Create `Templates/SqsEventBus/` folder
  - [ ] Implement CSharpFileBuilder template
  - [ ] Add in-memory message queue
  - [ ] Add `Publish<T>()` method
  - [ ] Add `FlushAllAsync()` with SQS batch sending
  - [ ] Add `MessageType` attribute setting

- [ ] **4.6** Create `Templates/SqsConfiguration/` folder
  - [ ] Implement metadata-driven CSharpFileBuilder template
  - [ ] Add `IAmazonSQS` registration (with LocalStack detection)
  - [ ] Add `IEventBus` registration
  - [ ] Add `ISqsMessageDispatcher` registration
  - [ ] Add `SqsPublisherOptions` configuration
  - [ ] Add `SqsSubscriptionOptions` configuration
  - [ ] Add handler registrations

- [ ] **4.7** Create `Templates/TemplateExtensions.cs`
  - [ ] Add helper methods for templates

### Phase 5: NuGet Package Management

- [ ] **5.1** Create `NugetPackages.cs`
- [ ] **5.2** Define constants for:
  - [ ] `AWSSDK.SQS`
  - [ ] `AWSSDK.Extensions.NETCore.Setup`
  - [ ] `Amazon.Lambda.SQSEvents`
  - [ ] `Amazon.Lambda.Core`
- [ ] **5.3** Implement framework-based versioning strategy

### Phase 6: Module Metadata

- [ ] **6.1** Update `.imodspec` file:
  - [ ] Set module ID, version, summary, description
  - [ ] Register all 7 templates with GUIDs
  - [ ] Register factory extension
  - [ ] Add dependencies (Intent.Eventing.Contracts, etc.)
  - [ ] Add interoperability tags (if needed)
  - [ ] Add icon/logo

### Phase 7: Project Integration

- [ ] **7.1** Update `Intent.Modules.Aws.Sqs.csproj`:
  - [ ] Import shared project: `<Import Project="..\Intent.Modules.Integration.IaC.Shared.AwsSqs\Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems" Label="Shared" />`
  - [ ] Add SDK package references
  
### Phase 8: Testing & Validation

- [ ] **8.1** Create test Intent Architect application
- [ ] **8.2** Add message models with AWS SQS stereotype
- [ ] **8.3** Add publish/subscribe interactions
- [ ] **8.4** Run software factory
- [ ] **8.5** Compare generated code with `SqsSample` reference
- [ ] **8.6** Test compilation
- [ ] **8.7** Test LocalStack integration
- [ ] **8.8** Test message publishing
- [ ] **8.9** Test message consumption

### Phase 9: Documentation

- [ ] **9.1** Create README.md with usage instructions
- [ ] **9.2** Document stereotype properties
- [ ] **9.3** Document configuration options
- [ ] **9.4** Create migration guide from manual implementation

---

## 🎯 Success Criteria

### Functional Requirements
✅ **Publishing**: Can publish events to SQS queues via `IEventBus`  
✅ **Routing**: Can route SQS messages to typed handlers via `ISqsMessageDispatcher`  
✅ **Configuration**: Metadata-driven configuration from Intent Architect designers  
✅ **Flexibility**: Works in Lambda, console apps, web apps (hosting-agnostic)  

### Code Quality
✅ **Pattern Consistency**: Matches Azure Service Bus module architecture  
✅ **Type Safety**: Strong typing for all generated code  
✅ **Dependency Injection**: Proper DI registration and lifecycle management  
✅ **Testability**: Components are mockable and testable  

### Integration
✅ **Metadata Loading**: IntegrationManager correctly aggregates published/subscribed messages  
✅ **Template Generation**: All templates generate valid, compilable C# code  
✅ **NuGet Resolution**: All package references resolve correctly  
✅ **LocalStack Support**: Configuration detects and adapts for LocalStack  

---

## 🚀 Key Differences: Azure Service Bus vs AWS SQS

| Aspect | Azure Service Bus | AWS SQS |
|--------|------------------|---------|
| **Channel Types** | Queue + Topic/Subscription | Queue only |
| **Stereotype Complexity** | `Type` enum (Queue/Topic) + Names | `QueueName` only (simpler) |
| **Shared Project** | `AzureServiceBusItemBase`, `AzureServiceBusChannelType` | `SqsItemBase` (no channel type) |
| **Configuration Keys** | `AzureServiceBus:{QueueName}` | `AwsSqs:{QueueName}` |
| **Subscription Naming** | Topic → Subscription name | N/A (queues are 1:1) |
| **Client SDK** | `Azure.Messaging.ServiceBus` | `AWSSDK.SQS` |
| **Message Routing** | Via `MessageType` attribute | Via `MessageType` attribute (same) |
| **Transaction Handling** | Consumer-specific | Consumer-specific (same) |

**Key Simplification:** AWS SQS module is architecturally simpler due to the lack of pub/sub topics. This means:
- No `ChannelType` enum needed
- No subscription name configuration
- No Topic vs Queue logic branching
- Cleaner stereotype model

---

## 📚 Reference Files

### Azure Service Bus Module (Pattern to Follow - Available in Workspace)
Use these files as reference by searching the workspace:
- **Shared project structure**: `Intent.Modules.Integration.IaC.Shared.AzureServiceBus/` folder
- **Stereotype pattern**: Search for `MessageModelStereotypeExtensions.cs` in Azure module
- **Factory initialization**: Search for `MetadataLoaderExtension.cs` in Azure module  
- **Configuration template**: Search for `AzureServiceBusConfiguration` template folder

### AWS SQS Reference Implementation (Expected Output Patterns)
The module should generate code similar to these conceptual patterns:
- **Publishing**: `SqsEventBus.cs` - In-memory queue, batch sending to SQS
- **Routing**: `SqsMessageDispatcher.cs` - MessageType attribute-based routing
- **DI Setup**: `SqsConfiguration.cs` - Registers IAmazonSQS, IEventBus, handlers
- **Lambda Consumer**: `*QueueConsumer.cs` - Future bridge module responsibility

---

## 📝 Notes for Implementation Agent

### Critical Path
1. **MUST START WITH SHARED PROJECT** - Templates depend on `IntegrationManager` and `SqsItemBase`
2. **API Extensions before Templates** - Templates need `MessageModelStereotypeExtensions`
3. **Simple Templates First** - Build Options classes before EventBus/Dispatcher
4. **Configuration Template Last** - Needs all other components to exist

### Common Pitfalls to Avoid
⚠️ Don't copy Azure's `ChannelType` - SQS doesn't have topics  
⚠️ Don't implement Lambda generation in core module - that's a bridge module  
⚠️ Don't forget `IntegrationManager.Initialize()` in factory extension  
⚠️ Don't hardcode queue URLs - use configuration pattern  
⚠️ Don't skip `MessageType` attribute - critical for routing  

### Testing Recommendations
1. Compile shared project independently first
2. Test stereotype extensions with unit tests
3. Generate code in test app early (don't wait until end)
4. Compare with SqsSample reference continuously
5. Test LocalStack integration before AWS integration

### When in Doubt
- Refer to Azure Service Bus module for patterns
- Check SqsSample reference for runtime behavior
- SQS is simpler than Azure - remove complexity, don't add it

---

## 🎉 End Goal

Upon completion, developers should be able to:

1. **Model** integration events in Intent Architect designer
2. **Apply** AWS SQS stereotype with queue name
3. **Run** software factory
4. **Get** fully functional SQS publishing and routing infrastructure
5. **Extend** with Lambda consumers via bridge module (future)

The generated code should be **indistinguishable** from hand-written code and follow .NET/AWS best practices.

### LocalStack Testing
- Generate code for LocalStack configuration
- Verify AWS:ServiceURL detection works
- Verify messages publish to LocalStack
- Verify messages consumed from LocalStack

---

## 📊 Success Criteria

### Code Generation
✅ All templates generate valid, compilable C# code  
✅ Generated code matches SqsSample reference patterns  
✅ NuGet packages resolve correctly  
✅ No compilation errors in generated projects  

### Functionality
✅ Messages publish to SQS queues successfully  
✅ Messages route to correct handlers  
✅ Configuration reads from Intent Architect metadata  
✅ LocalStack support works for development  

### Architecture
✅ Core module is hosting-agnostic  
✅ No Lambda-specific code in core module  
✅ Follows Azure Service Bus module patterns  
✅ Integrates cleanly with Intent Architect designers  

### Documentation
✅ README.md explains usage  
✅ Implementation plan documents design  
✅ Stereotype usage is clear  
✅ Configuration requirements documented  

---

## 🚀 Future Work (Out of Scope)

### Intent.AwsLambda.Sqs Bridge Module
- Lambda consumer function generation
- `[LambdaFunction]` attribute support
- SQSEvent handling
- Event source mapping configuration
- Serverless.template integration

### IaC Integration
- SQS queue creation (CloudFormation/Terraform)
- Dead letter queue configuration
- IAM policy generation
- Queue URL outputs

### Advanced Features
- Message batching optimization
- Retry policies
- DLQ handling
- SNS + SQS integration (fan-out)
- FIFO queue support

---

## 📚 Reference Documentation

### AWS SDK
- [AWSSDK.SQS Documentation](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/SQS/NSQS.html)
- [Amazon SQS Developer Guide](https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/)
- [LocalStack SQS](https://docs.localstack.cloud/user-guide/aws/sqs/)

### Intent Architect
- [Module Builder Documentation](https://docs.intentarchitect.com/)

---

---

## 📖 TL;DR - Quick Start Guide for Implementation Agent

### What You're Building
An Intent Architect module that generates AWS SQS infrastructure code for .NET applications, following the Azure Service Bus module pattern but simpler (no topics/subscriptions).

### Critical First Step
**BUILD THE SHARED PROJECT FIRST!** (`Intent.Modules.Integration.IaC.Shared.AwsSqs`)

This shared project contains:
1. **IntegrationManager.cs** - Aggregates published/subscribed messages from metadata
2. **SqsMessage.cs** - Wraps MessageModel with queue name resolution
3. **SqsItemBase.cs** - Abstract base for messages/commands
4. **SqsMethodType.cs** - Publish/Subscribe enum

Why first? Because ALL templates depend on these types.

### Implementation Path
```
Shared Project (5 files) 
  → API Extensions (stereotype) 
  → Factory Extension (initialize manager) 
  → Simple Templates (Options classes) 
  → Core Templates (EventBus, Dispatcher) 
  → Configuration Template (orchestrator) 
  → Module Metadata (.imodspec)
```

### Key Files to Create (Total: ~20 files)

**Shared Project (5 files):**
- `SqsMethodType.cs` - 7 lines
- `SqsItemBase.cs` - 15 lines
- `SqsMessage.cs` - 65 lines (similar to Azure's `AzureServiceBusMessage.cs`)
- `IntegrationManager.cs` - 120 lines (similar to Azure's `IntegrationManager.cs`)
- `.projitems` - Update to include above files

**Main Module (~15 files):**
- API: `MessageModelStereotypeExtensions.cs` (55 lines)
- Factory: `MetadataLoaderExtension.cs` (15 lines)
- Templates: 7 template folders with partials and builders
- Infrastructure: `NugetPackages.cs`, `.imodspec` updates, `.csproj` import

### Pattern to Follow
Search workspace for `Intent.Modules.Eventing.AzureServiceBus` module, then **simplify**:
- Remove `ChannelType` logic (no topics)
- Remove subscription name generation
- Remove Queue/Topic branching
- Keep metadata aggregation pattern
- Keep configuration pattern

### Success Metric
Generated code should follow the patterns specified in this document:
- **SqsEventBus**: In-memory queue + batch publishing
- **SqsMessageDispatcher**: MessageType-based routing
- **SqsConfiguration**: Metadata-driven DI registration
- Compilable, testable, production-ready output

### Time Estimate
- **Shared Project**: 2-3 hours
- **API + Factory**: 1 hour
- **Templates**: 4-6 hours
- **Integration + Testing**: 2-3 hours
- **Total**: ~10-15 hours for full implementation

### Most Complex Parts
1. **IntegrationManager.cs** - Metadata aggregation logic
2. **SqsConfiguration template** - Metadata-driven DI registration
3. **SqsEventBus template** - Message batching and publishing

### Easiest Parts
1. All enum/record types (5-10 lines each)
2. Interface definitions
3. Options classes (string interpolation templates)

### When Stuck
1. Check Azure Service Bus equivalent file
2. Check `SqsSample` reference implementation
3. SQS is simpler - if you're adding complexity, you're going wrong

---

**Document Version:** 2.0  
**Last Updated:** October 15, 2025  
**Status:** Implementation Phase - Ready for Execution  
**Priority:** Shared Project First
