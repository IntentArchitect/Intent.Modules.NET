# Intent.Modules.Aws.Sqs - Implementation Plan

## 📋 Executive Summary

This document outlines the implementation plan for `Intent.Modules.Aws.Sqs`, a core eventing infrastructure module for AWS SQS integration in Intent Architect. This module follows the architectural pattern established by `Intent.Modules.Eventing.AzureServiceBus` and provides the foundation for SQS-based event publishing and message routing.

**Status:** Planning Phase  
**Target Completion:** TBD  
**Dependencies:** Intent.Eventing.Contracts, Intent.Modelers.Eventing, Intent.Modelers.Services.EventInteractions

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

### Working Reference Application

Located at: `e:\TestApps\SqsSample\SqsSample`

This application demonstrates:
- SqsEventBus publishing to SQS
- SqsMessageDispatcher routing messages to handlers
- Lambda function consuming SQS events
- TransactionScope for atomic operations
- LocalStack support for development

**Key Files:**
- `SqsSample.Infrastructure/Eventing/SqsEventBus.cs` - Publishing implementation
- `SqsSample.Infrastructure/Eventing/SqsMessageDispatcher.cs` - Routing implementation
- `SqsSample.Infrastructure/Configuration/SqsConfiguration.cs` - DI registration
- `SqsSample.Api/ClientsQueueConsumer.cs` - Lambda consumer (bridge module pattern)

---

## 📦 Components to Implement

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

**Stereotype Properties:**
- `QueueName` (string) - Name of the SQS queue
- `QueueUrl` (string, optional) - Full queue URL (can be in config)

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

**Document Version:** 1.0  
**Last Updated:** October 14, 2025  
**Status:** Planning Phase
