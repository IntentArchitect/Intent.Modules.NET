# AWS SQS Shared Project Implementation Summary

## 🎯 Purpose

This document provides a focused summary of the **Shared Project** implementation requirements for the AWS SQS module, extracted from the full implementation plan.

## � Locating Reference Files

All reference implementations are available in the current workspace:

### Azure Service Bus Module (Pattern to Follow)
Use workspace search or file_search tool to locate:
- `Intent.Modules.Integration.IaC.Shared.AzureServiceBus/` - Shared project folder
- `IntegrationManager.cs` in Azure shared project - Metadata aggregation pattern
- `AzureServiceBusMessage.cs` in Azure shared project - Message implementation pattern
- `AzureServiceBusItemBase.cs` in Azure shared project - Base class pattern

### This Module (Target Implementation)
- Shared project folder: `../Intent.Modules.Integration.IaC.Shared.AwsSqs/`
- Main module folder: `./` (current directory)

## �🚨 Why Shared Project First?

The shared project `Intent.Modules.Integration.IaC.Shared.AwsSqs` is **mandatory** before implementing any templates in the main module because:

1. **IntegrationManager** is used by all templates to access metadata
2. **SqsItemBase** and **SqsMessage** are used in template generation
3. Templates won't compile without these types in scope
4. Factory extension needs to call `IntegrationManager.Initialize()`

## 📁 Files to Create

### File 1: `SqsMethodType.cs`
**Location:** `Intent.Modules.Integration.IaC.Shared.AwsSqs/SqsMethodType.cs`  
**Lines:** ~7  
**Complexity:** Simple

```csharp
#nullable enable
namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal enum SqsMethodType
{
    Publish = 1,
    Subscribe = 2
}
```

---

### File 2: `SqsItemBase.cs`
**Location:** `Intent.Modules.Integration.IaC.Shared.AwsSqs/SqsItemBase.cs`  
**Lines:** ~15  
**Complexity:** Simple

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
- No `ChannelType` property (SQS only has queues)
- No `QueueOrTopicSubscriptionConfigurationName` (no subscriptions in SQS)

---

### File 3: `SqsMessage.cs`
**Location:** `Intent.Modules.Integration.IaC.Shared.AwsSqs/SqsMessage.cs`  
**Lines:** ~65  
**Complexity:** Medium

```csharp
using System;
using Intent.Aws.Sqs.Api;  // Will be created in main module
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
- Extracts queue name from AWS SQS stereotype or uses convention
- Generates configuration keys like `"AwsSqs:ClientCreated"`
- Provides type names for code generation

**Dependencies:**
- `Intent.Aws.Sqs.Api.MessageModelStereotypeExtensions` (created in Phase 2)

---

### File 4: `IntegrationManager.cs`
**Location:** `Intent.Modules.Integration.IaC.Shared.AwsSqs/IntegrationManager.cs`  
**Lines:** ~120  
**Complexity:** Complex (but pattern-based)

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
1. Scans all applications in solution for AWS SQS module
2. Collects published/subscribed messages from metadata
3. Provides aggregation methods for templates
4. Ensures deduplication

**Usage in Templates:**
```csharp
// In SqsConfiguration template
var publishers = IntegrationManager.Instance.GetAggregatedPublishedSqsItems(ExecutionContext.GetApplicationConfig().Id);
var subscribers = IntegrationManager.Instance.GetAggregatedSubscribedSqsItems(ExecutionContext.GetApplicationConfig().Id);
```

---

### File 5: `Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems`
**Location:** `Intent.Modules.Integration.IaC.Shared.AwsSqs/Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems`  
**Lines:** ~18  
**Complexity:** Simple

**Update the existing file to:**
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
  </ItemGroup>
</Project>
```

---

## 🔗 Integration with Main Module

Once the shared project is complete, the main module must:

### 1. Import Shared Project
**File:** `Intent.Modules.Aws.Sqs.csproj`

Add this line:
```xml
<Import Project="..\Intent.Modules.Integration.IaC.Shared.AwsSqs\Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems" Label="Shared" />
```

### 2. Initialize IntegrationManager
**File:** `Intent.Modules.Aws.Sqs/FactoryExtensions/MetadataLoaderExtension.cs`

```csharp
public class MetadataLoaderExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Aws.Sqs.MetadataLoaderExtension";

    public override void Execute()
    {
        IntegrationManager.Initialize(ExecutionContext);
    }
}
```

### 3. Use in Templates
**Example:** `SqsConfiguration` template

```csharp
var publishers = IntegrationManager.Instance.GetAggregatedPublishedSqsItems(ExecutionContext.GetApplicationConfig().Id);
var subscribers = IntegrationManager.Instance.GetAggregatedSubscribedSqsItems(ExecutionContext.GetApplicationConfig().Id);

foreach (var publisher in publishers)
{
    // Generate publisher configuration
    // publisher.QueueName
    // publisher.QueueConfigurationName
    // publisher.GetModelTypeName(this)
}
```

---

## ✅ Verification Checklist

After implementing the shared project:

- [ ] All 4 C# files compile without errors
- [ ] `.projitems` file includes all 4 source files
- [ ] Namespace is `Intent.Modules.Integration.IaC.Shared.AwsSqs`
- [ ] All classes/records are `internal`
- [ ] `IntegrationManager` implements singleton pattern correctly
- [ ] `SqsMessage` correctly uses stereotype extensions (will add in Phase 2)
- [ ] Main module `.csproj` imports shared project
- [ ] Can reference types from shared project in main module

---

## 🎯 Next Steps After Shared Project

1. **Phase 2:** Create API Extensions (`MessageModelStereotypeExtensions.cs`)
2. **Phase 3:** Create Factory Extension (`MetadataLoaderExtension.cs`)
3. **Phase 4:** Create Templates (Options, EventBus, Dispatcher, Configuration)

**Do not proceed to templates until shared project is complete and tested.**

---

## 📚 Reference Implementation

**Pattern to follow:**  
Search workspace for folder: `Intent.Modules.Integration.IaC.Shared.AzureServiceBus`

**How to find Azure files:**
```
Use file_search or grep_search tools:
- file_search: "Intent.Modules.Integration.IaC.Shared.AzureServiceBus/**/*.cs"
- Search for: "IntegrationManager.cs" OR "AzureServiceBusMessage.cs"
```

**Key differences when adapting from Azure:**
- Remove `AzureServiceBusChannelType` (no topics in SQS)
- Simplify `SqsItemBase` (no subscription configuration names)
- Keep `IntegrationManager` pattern exactly the same
- Keep metadata aggregation logic exactly the same

---

## 🚨 Common Mistakes to Avoid

❌ **Don't** add `ChannelType` - SQS only has queues  
❌ **Don't** add subscription names - SQS doesn't have subscriptions  
❌ **Don't** skip `IntegrationManager.Initialize()` - templates will throw exceptions  
❌ **Don't** make classes `public` - use `internal` (shared project)  
❌ **Don't** implement Lambda consumers here - that's a bridge module  

✅ **Do** follow Azure's pattern closely  
✅ **Do** simplify where SQS is simpler than Azure  
✅ **Do** test shared project compilation independently  
✅ **Do** verify IntegrationManager singleton works  

---

**Status:** Ready for Implementation  
**Priority:** HIGH - Blocking all other work  
**Estimated Time:** 2-3 hours
