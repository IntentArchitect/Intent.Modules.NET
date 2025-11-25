# Intent.AI.Blazor

The **Intent.AI.Blazor** module enables AI-powered implementation of Blazor UI components using pre-engineered prompt templates. This module leverages Large Language Models (LLMs) to generate production-ready Blazor views based on your application's domain models, service contracts, and existing component patterns.

> [!NOTE]
> To use this feature, ensure that the required [User Settings](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html#user-settings) have been completed — including a valid API key for your selected AI provider.

> [!TIP]
> For a comprehensive tutorial on using this module to build Blazor UIs, see [Blazor UI Modeling with AI](https://docs.intentarchitect.com/articles/application-development/modelling/ui-designer/blazor-modeling/blazor-modeling.html).

## What This Module Provides

The `Intent.AI.Blazor` module provides:

- **AI Implementation Context Menu** - "Implement with AI" option on UI Page and Component elements
- **Pre-Engineered Prompt Templates** - Six specialized templates for common MudBlazor patterns:
  - **Search Entity** - List/grid views with filtering, sorting, and pagination
  - **Add Entity** - Create forms for new records
  - **Edit Entity** - Update forms for existing records
  - **View Entity** - Read-only detail views
  - **Add Entity Dialog** - Modal dialogs for creating records
  - **Edit Entity Dialog** - Modal dialogs for updating records
- **Context-Aware Generation** - AI considers your domain models, DTOs, services, and existing components
- **MudBlazor Integration** - Generated components use [MudBlazor](https://mudblazor.com/) controls with best practices
- **Customizable Prompt Templates** - Extend or replace templates to support different component libraries

## Implement with AI

Once you have modeled your UI components in the UI Designer, you can use AI to generate the View implementation.

> [!NOTE]
> Ensure your **Software Factory** changes have been applied before running AI prompts, as generated code provides context for the AI.

### Basic Workflow

1. **Model your UI Component** in the UI Designer (Page or Dialog)
2. **Run the Software Factory** to generate the ViewModel (`.razor.cs`)
3. **Right-click on the Component** → **Implement with AI**
4. **Review the AI dialog** and optionally adjust settings
5. **Click Execute** - Intent Architect submits a prompt to the LLM
6. **Review proposed changes** as a code diff
7. **Apply changes** to accept the AI-generated View (`.razor`)

![Implement with AI Dialog](images/implement-ai-dialog.png)

### Influencing Factors

The quality and relevance of generated UI code depend on several factors:

#### Intent Modeling

Before using **Implement with AI**, ensure:

- **Generated Code is up-to-date**: Run the Software Factory to apply all outstanding code changes
- **Component naming follows conventions**: Use descriptive names like `CustomerSearch`, `AddCustomer`, `EditOrder`
- **Service interactions are modeled**: Use "Call Backend Service" to connect to Commands/Queries
- **Navigations are defined**: Model navigation operations and dialog invocations


## Related Documentation

- [Blazor UI Modeling with AI - Tutorial](https://docs.intentarchitect.com/articles/application-development/modelling/ui-designer/blazor-modeling/blazor-modeling.html)
- [Intent.Common.AI - User Settings](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html)
- [Intent.Blazor - Core Module](xref:modules-dotnet.intent-blazor)
- [Intent.Blazor.Components.MudBlazor](xref:modules-dotnet.intent-blazor-components-mudblazor)
- [Intent.AI.AutoImplementation - AI Business Logic](xref:modules-dotnet.intent-ai-autoimplementation)
- [Code Management - Intent Architect](xref:application-development.code-management.about-code-management)

## External Resources

- [MudBlazor Official Documentation](https://mudblazor.com/)
- [Blazor Documentation - Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
