using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AI.AutoImplementation.Tasks.Helpers;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Intent.Modules.AI.AutoImplementation.Tasks;

#nullable enable

public class AutoImplementServiceOperationTask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    public AutoImplementServiceOperationTask(
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

    public string TaskTypeId => "Intent.Modules.AI.AutoImplement.ServiceOperation";
    public string TaskTypeName => "Auto-Implementation with AI Task (Service Operation)";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();

        var element = _metadataManager.Services(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
        if (element == null)
        {
            throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
        }
        var inputFiles = GetInputFiles(element);
        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

        var requestFunction = CreatePromptFunction(kernel);
        var fileChangesResult = requestFunction.InvokeFileChangesPrompt(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext,
            ["targetFileName"] = element.Name + "Handler",
            ["modelName"] = element.Name
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

    private static KernelFunction CreatePromptFunction(Kernel kernel)
    {
	    const string promptTemplate =
            """
            ## Role and Context
            You are a senior C# developer specializing in clean architecture with Entity Framework Core. You're implementing business logic in a system that strictly follows the repository pattern.

            ## Primary Objective
            Implement the `{{$modelName}}` service method in the {{$targetFileName}} service class following the implementation process below exactly.

            ## Repository Pattern Rules (CRITICAL)
            1. **NEVER use Queryable() or access DbContext directly** from service methods - this violates the architecture
            2. **ONLY use existing repository methods** defined in the interfaces when possible
            3. Don't add a new repository method for filtering purposes where the standard available methods could be used instead.
            4. If you need custom repository functionality:
               - Define the method in the appropriate repository interface
               - Implement the method in the corresponding concrete repository class
               - Apply `[IntentIgnore]` attribute to both declaration and implementation
               - Then call this method from your service method
            5. Repository methods cannot return DTOs and must define their own data contracts alongside the interface if needed.

            ## Implementation Process
            1. First, analyze all code files provided and understand how the fit together.
            2. Next, analyze which repository interface to use and which methods could be appropriate to complete the implementation.
            3. If an existing repository method can accomplish the use case efficiently, skip step 4 and go straight to step 5 implementation (IMPORTANT).
            4. If the response contains aggregated data (e.g. Count, Sum, Average, etc.):
               - Identify which repository interface needs extension
               - Add a properly named method to that interface with appropriate parameters and return type
               - Implement the method in the concrete repository class with proper EF Core code
               - Mark both with `[IntentIgnore]`
               - Use this method in implementing the {{$modelName}} method (IMPORTANT).
            5. Implement the service's {{$modelName}} method using the appropriate repository methods.
            6. Update the `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` attribute to `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`

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
               - The Service class implementation (primarily the {{$modelName}} method)
               - Repository interfaces (adding new methods only)
               - Repository concrete classes (implementing new methods only)
            2. Preserve all existing code, attributes, and file paths exactly

            ## Additional User Context (Optional)
            {{$userProvidedContext}}

            ## Input Code Files
            {{$inputFilesJson}}

            ## Previous Error Message
            {{$previousError}}

            ## Required Output Format
            Your response MUST include:
            1. Respond ONLY with JSON that matches the following schema:
            ```json
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
            ```
            2. The Content must contain:
            2.1. The fully implemented handler class with the Handle method
            2.2. Any modified repository interfaces (if you added methods)
            2.3. Any modified repository concrete classes (if you added implementations)
            2.4. All files must maintain their exact original paths
            2.5. All existing code and attributes must be preserved unless explicitly modified

            ## Important Reminders
            - NEVER remove or modify existing class members
            - NEVER access DbContext or use Queryable() directly in services
            - NEVER invoke repository methods that don't exist
            - IF you add a new repository method, you MUST provide BOTH the interface declaration AND concrete implementation
            - ALL new repository methods must be marked with `[IntentIgnore]`
            - Performance and clean architecture are key priorities
            - You can't access owned entities from the dbContext in EntityFrameworkCore
            """;
	    
	    var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate);
	    return requestFunction;
    }
    
    private List<ICodebaseFile> GetInputFiles(IElement element)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        var inputFiles = filesProvider.GetFilesForMetadata(element.ParentElement).ToList();
        if (element.TypeReference.ElementId != null)
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(element.TypeReference.Element));
        }
        foreach (var dto in element.ChildElements.Where(x => x.TypeReference.Element.IsDTOModel()).Select(x => x.TypeReference.Element))
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(dto));
        }
        inputFiles.AddRange(GetRelatedDomainEntities(element).SelectMany(x => filesProvider.GetFilesForMetadata(x)));

        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResult"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResultMappingExtensions"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.PagedListInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));

        return inputFiles;
    }

    private static List<ICanBeReferencedType> GetRelatedDomainEntities(IElement element)
    {
        var queriedEntity = element.AssociatedElements.Where(x => x.TypeReference.Element != null)
            .Select(x => x.TypeReference.Element.AsClassModel())
            .ToList();
        if (queriedEntity.Count == 0)
        {
            return [];
        }
        var relatedClasses = queriedEntity.Select(x => x.InternalElement)
            .Concat(queriedEntity.SelectMany(x => x.AssociatedClasses.Select(y => y.TypeReference.Element)))
            .Distinct()
            .ToList();
        return relatedClasses;
    }
}