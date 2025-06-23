using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using ChatResponseFormat = OpenAI.Chat.ChatResponseFormat;
using Formatting = Newtonsoft.Json.Formatting;

namespace Intent.Modules.AI.AutoImplementation.Tasks;

public class AutoImplementServiceOperationTask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private IApplicationConfig _applicationConfig;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

    [Experimental("SKEXP0010")]
    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";

        _applicationConfig = _solution.GetApplicationConfig(applicationId);

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();

        var element = _metadataManager.Services(applicationId).Elements.Single(x => x.Id == elementId);
        var promptTemplate = GetPromptTemplate(element);

        var inputFiles = GetInputFiles(element);

        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

        var chatResponseFormat = CreateJsonSchemaFormat();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = chatResponseFormat
        };
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate, executionSettings);
        var result = requestFunction.InvokeAsync(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext
        }).Result;

        FileChangesResult fileChangesResult = JsonConvert.DeserializeObject<FileChangesResult>(result.ToString());

        // Output the updated file changes.
        var applicationConfig = _solution.GetApplicationConfig(args[0]);
        var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
        foreach (var fileChange in fileChangesResult.FileChanges)
        {
            _outputRegistry.Register(Path.Combine(basePath, fileChange.FilePath), fileChange.Content);
        }

        return "success";
    }


    private string GetPromptTemplate(IElement model)
    {
        var targetFileName = model.ParentElement.Name;
        var prompt = @$"
## Role and Context
You are a senior C# developer specializing in clean architecture with Entity Framework Core. You're implementing business logic in a system that strictly follows the repository pattern.

## Primary Objective
Implement the `{model.Name}` service method in the {targetFileName} service class following the implementation process below exactly.

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
   - Use this method in implementing the {model.Name} method (IMPORTANT).
5. Implement the service's {model.Name} method using the appropriate repository methods.
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
   - The Service class implementation (primarily the {model.Name} method)
   - Repository interfaces (adding new methods only)
   - Repository concrete classes (implementing new methods only)
2. Preserve all existing code, attributes, and file paths exactly

## Additional User Context (Optional)
{{{{$userProvidedContext}}}}

## Input Code Files:
```json
{{{{$inputFilesJson}}}}
```

## Required Output Format
Your response MUST include:
1. The fully implemented {model.Name} method. Do not update any of the other existing methods on the service.
2. Any modified repository interfaces (if you added methods)
3. Any modified repository concrete classes (if you added implementations)
4. All files must maintain their exact original paths
5. All existing code and attributes must be preserved unless explicitly modified

## Important Reminders
- NEVER remove or modify existing class members
- NEVER access DbContext or use Queryable() directly in services
- NEVER invoke repository methods that don't exist
- IF you add a new repository method, you MUST provide BOTH the interface declaration AND concrete implementation
- ALL new repository methods must be marked with `[IntentIgnore]`
- Performance and clean architecture are key priorities
- You can't access owned entities from the dbContext in EntityFrameworkCore
";
        return prompt;
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

    private IEnumerable<ICanBeReferencedType> GetRelatedDomainEntities(IElement element)
    {
        var queriedEntity = element.AssociatedElements.Where(x => x.TypeReference.Element != null)
            .Select(x => x.TypeReference.Element.AsClassModel())
            .ToList();
        if (queriedEntity.Count == 0)
        {
            return [];
        }
        var relatedClasses = queriedEntity.Select(x => x.InternalElement)
            .Concat(queriedEntity.SelectMany(x => x.AssociatedClasses.Select(x => x.TypeReference.Element)))
            .Distinct()
            .ToList();
        return relatedClasses;
    }

    private static ChatResponseFormat CreateJsonSchemaFormat()
    {
        return ChatResponseFormat.CreateJsonSchemaFormat(jsonSchemaFormatName: "FileChangesResult",
            jsonSchema: BinaryData.FromString("""
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
                                              """),
            jsonSchemaIsStrict: true);
    }

    public class FileChangesResult
    {
        public FileChange[] FileChanges { get; set; }
    }
    public class FileChange
    {
        public FileChange()
        {
        }

        public FileChange(string filePath, string fileExplanation, string content)
        {
            FilePath = filePath;
            FileExplanation = fileExplanation;
            Content = content;
        }

        public string FileExplanation { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return $"{FilePath} ({FileExplanation})";
        }
    }
}