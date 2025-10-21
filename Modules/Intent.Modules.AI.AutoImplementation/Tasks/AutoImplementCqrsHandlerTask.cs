using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using Intent.Modules.AI.AutoImplementation.Tasks.Helpers;
using Intent.Modules.Common.AI.Settings;
using ApiMetadataDesignerExtensions = Intent.Modelers.Domain.Api.ApiMetadataDesignerExtensions;

namespace Intent.Modules.AI.AutoImplementation.Tasks;

#nullable enable

public class AutoImplementCqrsHandlerTask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    public AutoImplementCqrsHandlerTask(
        IApplicationConfigurationProvider applicationConfigurationProvider,
        IMetadataManager metadataManager,
        ISolutionConfig solution,
        IOutputRegistry outputRegistry,
        IUserSettingsProvider userSettingsProvider)
    {
        _applicationConfigurationProvider = applicationConfigurationProvider;
        _metadataManager = metadataManager;
        _solution = solution;
        _outputRegistry = outputRegistry;
        _intentSemanticKernelFactory = new IntentSemanticKernelFactory(userSettingsProvider);
    }

    public string TaskTypeId => "Intent.Modules.AI.AutoImplement.CqrsHandler";
    public string TaskTypeName => "Auto-Implementation with AI Task (CQRS Handler)";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";
        var provider = new AISettings.ProviderOptions(args[3]).AsEnum();
        var modelId = args[4];
        var thinkingType = args[5];

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel(modelId, provider, null);

        var element = _metadataManager.Services(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
        if (element == null)
        {
            throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
        }
        var inputFiles = GetInputFiles(element);
        foreach (var inputFile in inputFiles)
        {
            Logging.Log.Info($"Including file: {inputFile.FilePath}");
        }
        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

        var designContext = GetDesignContext(element);

        var requestFunction = CreatePromptFunction(kernel, thinkingType);
        var fileChangesResult = requestFunction.InvokeFileChangesPrompt(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["designContext"] = designContext,
            ["userProvidedContext"] = userProvidedContext,
            ["targetFileName"] = element.Name + "Handler"
        });

        // Output the updated file changes.
        var applicationConfig = _solution.GetApplicationConfig(args[0]);
        var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
        foreach (var fileChange in fileChangesResult.FileChanges)
        {
            _outputRegistry.Register(Path.Combine(basePath, fileChange.FilePath), fileChange.Content);
        }

        return "success";
    }

    private KernelFunction CreatePromptFunction(Kernel kernel, string thinkingType)
    {
        const string promptTemplate =
            """
		    ## Role and Context
		    You are a senior C# developer specializing in clean architecture with Entity Framework Core. You're implementing business logic in a system that strictly follows the repository pattern.

		    ## Primary Objective
		    Implement the `Handle` method in the {{$targetFileName}} class following the implementation process below exactly.

		    ## Repository Pattern Rules (CRITICAL)
		    1. **NEVER use Queryable() or access DbContext directly** from handlers - this violates the architecture
		    2. **ONLY use existing repository methods** defined in the interfaces when possible
		    3. Don't add a new repository method for filtering purposes where the standard available methods could be used instead.
		    4. If you need custom repository functionality:
		       - Define the method in the appropriate repository interface
		       - Implement the method in the corresponding concrete repository class
		       - Apply `[IntentIgnore]` attribute to both declaration and implementation
		       - Then call this method from your handler
		       - Utilize the Entity Framework Core Linq queries to fetch out data in a way this performant and readable.
		    5. Repository methods cannot return DTOs and must define their own data contracts alongside the interface if needed. If a new data contract is defined, then add the [IntentIgnore] attribute over the class.
		    6. Never process in memory that which would be more efficiently processed in the database via an Entity Framework Core Linq query (e.g. calculating aggregate values).

		    ## Implementation Process
		    1. First, analyze all code files provided and understand how the fit together.
		    2. Next, analyze which repository interface to use and which methods could be appropriate to complete the implementation.
		    3. If an existing repository method can accomplish the use case efficiently, skip step 4 and go straight to step 5 implementation (IMPORTANT).
		    4. If the response contains aggregated data (e.g. Count, Sum, Average, etc.):
		       - Identify which repository interface needs extension
		       - Add a properly named method to that interface with appropriate parameters and return type
		       - Implement the method in the concrete repository class with proper EF Core code
		       - Mark both with `[IntentIgnore]`
		       - Use this method in implementing the handler's Handle method (IMPORTANT).
		    5. Implement the handler's Handle method using the appropriate repository methods.
		    6. Update the `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` or `[IntentManaged(Mode.Fully, Body = Mode.Merge)]` attribute to `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`

		    ## Code Preservation Requirements (CRITICAL)
		    1. **NEVER remove or modify existing class members, methods, or properties**
		    2. **NEVER change existing method signatures or implementations**
		    3. **ONLY add new members when necessary (repository methods)**
		    4. **Preserve all existing attributes and code exactly as provided**
		    5. **Don't add comments to existing code**
		    6. **NEVER remove any existing using clauses (CRITICAL)**
		    7. **Ensure that `using Intent.RoslynWeaver.Attributes;` using clause is always present.**

		    ## Code File Modifications
		    1. You may modify ONLY:
		       - The Handler class implementation (primarily the Handle method)
		       - Repository interfaces (adding new methods only)
		       - Repository concrete classes (implementing new methods only)
		    2. Preserve all existing code, attributes, and file paths exactly

		    ## Important Reminders
		    - NEVER remove or modify existing class members.
		    - NEVER access DbContext or use Queryable() directly in handlers.
		    - NEVER invoke repository methods that don't exist.
		    - NEVER do any character escaping in your response.
		    - IF you add a new repository method, you MUST provide BOTH the interface declaration AND concrete implementation.
		    - ALL new repository methods must be marked with `[IntentIgnore]`.
		    - Performance and clean architecture are key priorities.
		    - (IMPORTANT) For handlers that update entities, DO NOT call the `Update` method on the repository, or call `UnitOfWork.SaveChangesAsync()`. These are called implicitly as part of the MediatR behaviour pipeline
		    - Use the same newline characters as the ones provided in the code files.
		    - (IMPORTANT) Ensure that any Linq queries against the DbContext will be valid during execution for Entity Framework Core
		    - You can't access owned entities from the dbContext in EntityFrameworkCore
		    
		    ## Design and Intent Context
		    {{$designContext}}
		    
		    ## Additional User Context (Optional)
		    {{$userProvidedContext}}

		    ## Input Code Files
		    {{$inputFilesJson}}

		    ## Previous Error Message
		    {{$previousError}}

		    ## Required Output Format
		    Your response MUST include:
		    1. Respond ONLY with deserializable JSON that matches the following schema:
		    {
		        "type": "object",
		        "properties": {
		            "FileChanges": {
		                "type": "array",
		                "items": {
		                    "type": "object",
		                    "properties": {
		                        "FilePath": { "type": "string" },
		                        "Content": { "type": "string" }
		                    },
		                    "required": ["FilePath", "Content"],
		                    "additionalProperties": false
		                }
		            }
		        },
		        "required": ["FileChanges"],
		        "additionalProperties": false
		    }
		    
		    EXAMPLE RESPONSE BEGIN
		    {
		        "FileChanges": [
		            {
		                "FilePath": <some-file-path_1>,
		                "Content": <some-file-content_1>
		            },
		            {
		                "FilePath": <some-file-path_2>,
		                "Content": <some-file-content_2>
		            }
		        ]
		    }
		    EXAMPLE RESPONSE END
		    
		    2. The Content must contain:
		    2.1. The fully implemented handler class with the Handle method
		    2.2. Any modified repository interfaces (if you added methods)
		    2.3. Any modified repository concrete classes (if you added implementations)
		    2.4. All files must maintain their exact original paths (CRITICAL)
		    2.5. All existing code and attributes must be preserved unless explicitly modified
		    2.6. DO NOT do any character escaping to the code.
		    """;

        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate, kernel.GetRequiredService<IAiProviderService>().GetPromptExecutionSettings(thinkingType));
        return requestFunction;
    }

    /// <summary>
    /// Returns important design guidelines for the LLM based on what the user has modeled in the designer.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private string GetDesignContext(IElement element)
    {
        var designContext = new StringBuilder();
        var associatedServices = element.AssociatedElements.Select(x => x.TypeReference.Element).ToList();
        if (associatedServices.Count > 0)
        {
            designContext.AppendLine("You are expected to use EACH of the following services to implement this handler (IMPORTANT):");
            foreach (var associatedService in associatedServices)
            {
                var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
                var codeFiles = filesProvider.GetFilesForMetadata(associatedService).ToList();
                if (((IElement)associatedService).ParentElement != null)
                {
                    codeFiles.AddRange(filesProvider.GetFilesForMetadata(((IElement)associatedService).ParentElement));
                }
                foreach (var interfaceFile in codeFiles.Where(x => Path.GetFileName(x.FilePath).StartsWith("I") && char.IsUpper(Path.GetFileName(x.FilePath)[1])))
                {
                    designContext.AppendLine($"- {Path.GetFileNameWithoutExtension(interfaceFile.FilePath)}");
                }
            }
        }
        else
        {
            designContext.AppendLine("NONE");
        }

        return designContext.ToString();
    }


    private List<ICodebaseFile> GetInputFiles(IElement element)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        var inputFiles = filesProvider.GetFilesForMetadata(element).ToList();
        if (element.TypeReference.ElementId != null)
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(element.TypeReference.Element));
        }
        foreach (var dto in element.ChildElements.Where(x => x.TypeReference.Element.IsDTOModel()).Select(x => x.TypeReference.Element))
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(dto));
        }
        inputFiles.AddRange(GetRelatedElements(element).SelectMany(x => filesProvider.GetFilesForMetadata(x)));

        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResult"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResultMappingExtensions"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.PagedListInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));

        return inputFiles;
    }


    private static List<ICanBeReferencedType> GetRelatedElements(IElement element)
    {
        var relatedElements = element.AssociatedElements.Where(x => x.TypeReference.Element != null)
            .Select(x => x.TypeReference.Element)
            .ToList();
        if (relatedElements.Count == 0)
        {
            return [];
        }
        var relatedClasses = relatedElements
            .Concat(relatedElements.Where(x => x.TypeReference?.Element?.IsDTOModel() == true).Select(x => x.TypeReference.Element))
            .Concat(relatedElements.Where(Intent.Modelers.Services.Api.OperationModelExtensions.IsOperationModel).Select(x => Intent.Modelers.Services.Api.OperationModelExtensions.AsOperationModel(x).ParentService.InternalElement))
            .Concat(relatedElements.Where(Intent.Modules.Common.Types.Api.OperationModelExtensions.IsOperationModel).Select(x => Intent.Modules.Common.Types.Api.OperationModelExtensions.AsOperationModel(x).InternalElement.ParentElement))
            .Concat(relatedElements.Where(x => x.IsClassModel()).Select(x => x.AsClassModel()).SelectMany(x => x.AssociatedClasses.Select(y => y.TypeReference.Element)))
            .ToList();
        return relatedClasses;
    }
}