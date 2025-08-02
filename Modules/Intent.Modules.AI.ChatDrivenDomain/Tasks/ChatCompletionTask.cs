using System;
using System.Text.Json;
using Intent.Engine;
using Intent.Modules.AI.ChatDrivenDomain.Plugins;
using Intent.Modules.AI.ChatDrivenDomain.Tasks.Helpers;
using Intent.Modules.AI.ChatDrivenDomain.Tasks.Models;
using Intent.Modules.Common.AI;
using Intent.Utils;
using Microsoft.SemanticKernel;

namespace Intent.Modules.AI.ChatDrivenDomain.Tasks;

public class ChatCompletionTask : ModuleTaskSingleInputBase<ChatCompletionModel>
{
    private readonly IUserSettingsProvider _userSettingsProvider;

    public ChatCompletionTask(IUserSettingsProvider userSettingsProvider)
    {
        _userSettingsProvider = userSettingsProvider;
    }

    public override string TaskTypeId => "Intent.Modules.ChatDrivenDomain.Tasks.ChatCompletionTask";
    public override string TaskTypeName => "Consulting the great LLAMA...";

    protected override ValidationResult ValidateInputModel(ChatCompletionModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel?.Prompt))
        {
            return ValidationResult.ErrorResult("Prompt is required");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(ChatCompletionModel inputModel)
    {
        var executeResult = new ExecuteResult();
        
        try
        {
            var modelMutationPlugin = new ModelMutationPlugin(inputModel);
            var kernel = new IntentSemanticKernelFactory(_userSettingsProvider).BuildSemanticKernel((builder) =>
            {
                builder.Plugins.AddFromObject(modelMutationPlugin);
            });

            var requestFunction = CreatePromptFunction(kernel);
            var result = requestFunction.InvokeAsync(kernel, new KernelArguments
            {
                ["prompt"] = inputModel.Prompt
            }).Result;

            Logging.Log.Info($"LLM Interaction Complete");

            // Get the final domain model from the plugin using new ElementModel structure
            var finalModel = modelMutationPlugin.GetCurrentModel();
            var jsonResult = JsonSerializer.Serialize(finalModel, SerializerOptions);
            
            Logging.Log.Debug($"Result: \r\n{jsonResult}");
            
            executeResult.Result = finalModel;
            return executeResult;
        }
        catch (Exception e)
        {
            Logging.Log.Failure(e);
            executeResult.Errors.Add(e.GetBaseException().Message);
            return executeResult;
        }
    }

    private static KernelFunction CreatePromptFunction(Kernel kernel)
    {
        const string promptTemplate =
            """
            # Domain Modeling Expert for Intent Architect

            You are a specialized domain modeling expert for Intent Architect. 
            Your task is analyzing, optimizing, and modifying domain models using Domain-Driven Design principles. 
            You MUST apply proper domain modeling constraints and best practices.

            ## Domain Model Structure

            The domain model consists of Classes, Attributes, and Associations:

            - Classes have a name, optional comment, attributes, and associations.
            - Attributes have a name, type, and can be nullable or collections. Attributes are storage fields of a Class.
            - Associations define relationships between classes.

            ## Primitive Types
            - string, int, long, decimal, datetime, bool, guid, object, float, double

            ## Relationship Rules - CRITICAL

            1. **Composite Relationships (1 -> 1)**: 
               - An entity can have only ONE composite owner
               - INVALID: If ClassA and ClassB both have a 1 -> 1 composite relationship to ClassC.
               - CORRECT: If an entity has a 1 -> 1 relationship, it cannot be owned by multiple entities.

            2. **One-to-Many (1 -> *)**: 
               - Source has one reference to Target.
               - Target has a collection of Source entities.
               - This is considered a Composite relationship by default.
               
            3. **Many-to-Many (* -> *)**:
               - Both sides have collections of each other.
               - This is considered an Aggregate relationship by default.

            4. **Navigability**:
               - "Source End" is where the relationship originates.
               - "Target End" is where the relationship points to.
               - Composite relationship will have the "Source End" on the Class being the "owner" and the "Target End" on the Class "being owned".
               - Aggregate relationship will favor the "Source End" on the Class needing the association on the other class while still retaining a unidirectional relationship (unless explicitly asked to make bidirectional by the user). 

            ## Available Validated Functions

            You MUST use these validated functions that provide proper Intent Architect integration and DDD semantics:

            ### Core Domain Functions
            1. `GetDomainContext()` - Gets current domain model context (call this FIRST to understand current state)
            2. `CreateClass(className, comment, packageId)` - Creates a new class with validation
            3. `CreateOrUpdateAttribute(classId, attributeName, typeName, isNullable, isCollection, comment, existingAttributeId)` - Creates/updates a single attribute
            4. `CreateAttributesForClass(classId, attributesJson)` - Creates multiple attributes in one operation
            5. `CreateAssociation(sourceClassId, targetClassId, sourceEndName, targetEndName, sourceCardinality, targetCardinality, associationType)` - Creates bidirectional associations with DDD validation

            ### Context Management Functions
            6. `GetCurrentContext()` - Gets comprehensive project context and state tracking information
            7. `UpdateContext(operation, details, status, metadata)` - Updates context with operation results for AI session tracking

            ### Function Details

            **CreateClass Parameters:**
            - className: The name of the class
            - comment: Optional description
            - packageId: Optional package ID (leave empty for current package)

            **CreateOrUpdateAttribute Parameters:**
            - classId: The ID of the class
            - attributeName: Name of the attribute
            - typeName: Type (string, int, bool, decimal, DateTime, Guid, or custom class name)
            - isNullable: Whether nullable (true/false)
            - isCollection: Whether collection (true/false)
            - comment: Optional description
            - existingAttributeId: For updates only (leave empty for new attributes)

            **CreateAttributesForClass Parameters:**
            - classId: The ID of the class
            - attributesJson: JSON array like: [{"name":"Email","type":"string","isNullable":true,"comment":"Contact email"}]

            **CreateAssociation Parameters:**
            - sourceClassId, targetClassId: Class IDs
            - sourceEndName, targetEndName: Association names (e.g., "Orders", "Customer")
            - sourceCardinality, targetCardinality: "1", "0..1", or "*"
            - associationType: "Composite" (ownership) or "Aggregate" (collaboration)

            **GetCurrentContext Parameters:**
            - No parameters - returns comprehensive context summary

            **UpdateContext Parameters:**
            - operation: The operation performed (e.g., "CreateClass", "CreateAssociation")
            - details: Detailed description of what was done
            - status: "success", "warning", or "error"
            - metadata: Additional information as JSON string (optional)

            ### DDD Rules Enforced by Tools
            - **Composite**: Owner controls lifecycle, only one composite owner per entity
            - **Aggregate**: Independent lifecycles, collaboration relationships
            - **Validation**: All tools validate before execution and return structured results
            - **ID Management**: Tools handle Intent Architect ID mapping automatically

            ## User Instructions

            ```
            {{$prompt}}
            ```

            ## Workflow

            1. **ALWAYS start by calling `GetDomainContext()`** to understand the current state
            2. **Optionally call `GetCurrentContext()`** for comprehensive project context and state tracking
            3. Use the specific validated functions to implement changes
            4. Each function returns a JSON result with success/failure and detailed information
            5. If a function fails, read the error message and correct your approach
            6. For associations, understand that ONE call creates BOTH ends of the relationship
            7. **Optionally use `UpdateContext()`** to track significant operations for future AI sessions

            ### Context Tracking Benefits
            - Enables seamless handoff between AI sessions
            - Maintains project state and operation history
            - Provides DDD pattern analysis and validation insights
            - Tracks model evolution and complexity metrics

            IMPORTANT: Use ONLY the listed functions. Do not generate JSON directly.
            """;
        
        var requestFunction = kernel.CreateFunctionFromPrompt(
            promptTemplate,
            new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            });
        return requestFunction;
    }
}
