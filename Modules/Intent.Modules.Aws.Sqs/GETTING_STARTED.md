# Getting Started - AWS SQS Module Implementation

> **📍 You are here:** `Intent.Modules.Aws.Sqs/` folder in the Intent.Modules.NET workspace

## 🤖 For AI Implementation Agents

This guide helps you navigate the workspace and locate reference files needed for implementing the AWS SQS module.

---

## Step 1: Explore the Workspace Structure

### Check Current Location
```bash
# See what's in the current module folder
list_dir("./")

# Expected files:
# - Intent.Modules.Aws.Sqs.csproj
# - IMPLEMENTATION_PLAN.md (this comprehensive plan)
# - SHARED_PROJECT_SUMMARY.md (focused on shared project)
# - GETTING_STARTED.md (this file)
```

### See All Modules
```bash
# Navigate to parent to see all Intent modules
list_dir("../")

# You should see folders like:
# - Intent.Modules.Eventing.AzureServiceBus/
# - Intent.Modules.Integration.IaC.Shared.AzureServiceBus/
# - Intent.Modules.Integration.IaC.Shared.AwsSqs/
# - Intent.Modules.Aws.Sqs/ (current)
# - ... many others
```

---

## Step 2: Examine Azure Reference Implementation

The Azure Service Bus module is your primary reference. It uses the same architecture pattern.

### Azure Shared Project (Pattern to Copy)
```bash
# List all files in Azure shared project
list_dir("../Intent.Modules.Integration.IaC.Shared.AzureServiceBus")

# Expected files:
# - IntegrationManager.cs (✨ KEY REFERENCE)
# - AzureServiceBusMessage.cs (✨ KEY REFERENCE)
# - AzureServiceBusCommand.cs
# - AzureServiceBusItemBase.cs (✨ KEY REFERENCE)
# - AzureServiceBusChannelType.cs
# - AzureServiceBusMethodType.cs (✨ KEY REFERENCE)
# - AzureHelper.cs
# - Intent.Modules.Integration.IaC.Shared.AzureServiceBus.projitems (✨ KEY REFERENCE)
```

### Read Key Azure Files
```bash
# Read the IntegrationManager pattern
read_file("../Intent.Modules.Integration.IaC.Shared.AzureServiceBus/IntegrationManager.cs", 1, 100)

# Read the Message implementation pattern
read_file("../Intent.Modules.Integration.IaC.Shared.AzureServiceBus/AzureServiceBusMessage.cs", 1, 100)

# Read the base abstraction
read_file("../Intent.Modules.Integration.IaC.Shared.AzureServiceBus/AzureServiceBusItemBase.cs", 1, 50)

# Read the method type enum
read_file("../Intent.Modules.Integration.IaC.Shared.AzureServiceBus/AzureServiceBusMethodType.cs", 1, 20)
```

### Azure Main Module (API Extensions Pattern)
```bash
# See Azure module structure
list_dir("../Intent.Modules.Eventing.AzureServiceBus")

# Find stereotype extensions
file_search("**/MessageModelStereotypeExtensions.cs")
```

---

## Step 3: Check AWS SQS Shared Project Current State

```bash
# See what exists in AWS SQS shared project (likely minimal)
list_dir("../Intent.Modules.Integration.IaC.Shared.AwsSqs")

# Expected files:
# - Intent.Modules.Integration.IaC.Shared.AwsSqs.shproj (should exist)
# - Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems (should exist, likely empty ItemGroup)

# Read the .projitems file to see current state
read_file("../Intent.Modules.Integration.IaC.Shared.AwsSqs/Intent.Modules.Integration.IaC.Shared.AwsSqs.projitems", 1, 50)
```

---

## Step 4: Search for Patterns Across Workspace

### Find All IntegrationManager Implementations
```bash
file_search("**/IntegrationManager.cs")

# Should return:
# - Azure implementation (reference)
# - Possibly others (for context)
```

### Find Stereotype Extension Patterns
```bash
grep_search("MessageModelStereotypeExtensions", isRegexp=false)

# Shows how stereotypes are implemented across modules
```

### Find Factory Extension Patterns
```bash
file_search("**/MetadataLoaderExtension.cs")

# Shows how IntegrationManager is initialized
```

---

## Step 5: Understand File Relationships

### Shared Project → Main Module
```
Intent.Modules.Integration.IaC.Shared.AwsSqs/  (CREATE FIRST)
├── IntegrationManager.cs              → Used by templates
├── SqsMessage.cs                      → Used by templates  
├── SqsItemBase.cs                     → Used by templates
├── SqsMethodType.cs                   → Used by SqsMessage
└── .projitems                         → References all above

                    ↓ imported by

Intent.Modules.Aws.Sqs/  (current)
├── Intent.Modules.Aws.Sqs.csproj      → <Import Project="..." />
├── Api/
│   └── MessageModelStereotypeExtensions.cs  → Used by SqsMessage
├── FactoryExtensions/
│   └── MetadataLoaderExtension.cs     → Calls IntegrationManager.Initialize()
└── Templates/
    └── (all templates use IntegrationManager)
```

---

## Step 6: Read Implementation Plan

Now that you understand the workspace structure:

1. **Start with TL;DR**: Read `IMPLEMENTATION_PLAN.md` section "📖 TL;DR"
2. **Focus on Shared Project**: Read `SHARED_PROJECT_SUMMARY.md` in detail
3. **Follow Phase 1**: Create all 5 shared project files
4. **Follow Checklist**: Use implementation checklist in `IMPLEMENTATION_PLAN.md`

---

## 🎯 Your Implementation Tasks

### Phase 1: Shared Project (Do This First)
Create these 5 files in `../Intent.Modules.Integration.IaC.Shared.AwsSqs/`:

1. ✅ `SqsMethodType.cs` - Enum (7 lines)
2. ✅ `SqsItemBase.cs` - Abstract record (15 lines)
3. ✅ `SqsMessage.cs` - Concrete record (65 lines)
4. ✅ `IntegrationManager.cs` - Singleton (120 lines)
5. ✅ Update `.projitems` - Add files to ItemGroup

**Reference:** See `SHARED_PROJECT_SUMMARY.md` for complete code

### Phase 2: API Extensions
Create in `./Api/`:
- `MessageModelStereotypeExtensions.cs`

### Phase 3: Factory Extensions
Create in `./FactoryExtensions/`:
- `MetadataLoaderExtension.cs`

### Phase 4-6: Templates
Create in `./Templates/`:
- SqsPublisherOptions/
- SqsSubscriptionOptions/
- ISqsMessageDispatcher/
- SqsMessageDispatcher/
- SqsEventBus/
- SqsConfiguration/

**Reference:** See `IMPLEMENTATION_PLAN.md` for detailed specifications

---

## 🔍 Useful Search Patterns

### Find Azure Patterns
```bash
# All Azure shared project files
file_search("**/Intent.Modules.Integration.IaC.Shared.AzureServiceBus/*.cs")

# All Azure main module files
file_search("**/Intent.Modules.Eventing.AzureServiceBus/**/*.cs")

# Specific pattern search
grep_search("IntegrationManager.Instance", isRegexp=false)
grep_search("GetAggregatedPublishedAzureServiceBusItems", isRegexp=false)
```

### Verify Your Work
```bash
# Check shared project has all files
list_dir("../Intent.Modules.Integration.IaC.Shared.AwsSqs")

# Check main module structure
list_dir("./Api")
list_dir("./FactoryExtensions")
list_dir("./Templates")

# Verify .csproj imports shared project
read_file("./Intent.Modules.Aws.Sqs.csproj", 1, 50)
grep_search("<Import Project.*AwsSqs", isRegexp=true, includePattern="*.csproj")
```

---

## 📚 Key Documentation Files

| File | Purpose |
|------|---------|
| `IMPLEMENTATION_PLAN.md` | Comprehensive 2000+ line implementation guide |
| `SHARED_PROJECT_SUMMARY.md` | Focused shared project guide with complete code |
| `GETTING_STARTED.md` | This file - workspace navigation guide |

---

## 🚨 Critical Reminders

1. **Work Locally**: All paths are relative (`../` = parent folder)
2. **Use Tools**: `file_search`, `grep_search`, `list_dir`, `read_file`
3. **Follow Azure**: Azure Service Bus is your primary reference
4. **Simplify**: AWS SQS is simpler than Azure (no topics, no subscriptions)
5. **Shared First**: Cannot implement templates without shared project

---

## ✅ Ready to Begin?

**Next Step:** Open `SHARED_PROJECT_SUMMARY.md` and start creating the 5 shared project files.

**Questions?** Use grep_search to find examples in Azure module:
```bash
grep_search("YOUR_QUESTION_KEYWORD", isRegexp=false)
```

Good luck! 🚀
