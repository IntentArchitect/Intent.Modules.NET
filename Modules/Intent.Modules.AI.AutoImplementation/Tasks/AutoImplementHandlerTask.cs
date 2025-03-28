using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Output;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AI.ChatDrivenDomain.Settings;
using Intent.Modules.AI.Prompts.Utils;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using OllamaSharp;
using ChatResponseFormat = OpenAI.Chat.ChatResponseFormat;
using Formatting = Newtonsoft.Json.Formatting;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Intent.Modules.AI.Prompts.Tasks;

public class AutoImplementHandlerTask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private IApplicationConfig _applicationConfig;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AutoImplementHandlerTask(
        IApplicationConfigurationProvider applicationConfigurationProvider,
        IMetadataManager metadataManager,
        ISolutionConfig solution,
        IOutputRegistry outputRegistry)
    {
        _applicationConfigurationProvider = applicationConfigurationProvider;
        _metadataManager = metadataManager;
        _solution = solution;
        _outputRegistry = outputRegistry;
    }

    public string TaskTypeId => "Intent.Modules.AI.Prompts.CreateMediatRHandlerPrompt";
    public string TaskTypeName => "Create Prompt for Handler";
    public int Order => 0;

    [Experimental("SKEXP0010")]
    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        _applicationConfig = _solution.GetApplicationConfig(applicationId);

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = BuildSemanticKernel();
        var chatResponseFormat = CreateJsonSchemaFormat();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = chatResponseFormat
        };

        var queryModel = _metadataManager.Services(applicationId).Elements.Single(x => x.Id == elementId);
        var promptTemplate = GetPromptTemplate(queryModel);
        var inputFiles = GetInputFiles(queryModel);

        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate, executionSettings);
        var result = requestFunction.InvokeAsync(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput
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
        var targetFileName = model.Name + "Handler";
        var prompt = @$"
## Role and Context
You are a senior C# developer specializing in clean architecture with Entity Framework Core. You're implementing business logic in a system that strictly follows the repository pattern.

## Primary Objective
Implement the `Handle` method in the {targetFileName} class following the implementation process below exactly.

## Repository Pattern Rules (CRITICAL)
1. **NEVER use Queryable() or access DbContext directly** from handlers - this violates the architecture
2. **ONLY use existing repository methods** defined in the interfaces when possible
3. Don't add a new repository method for filtering purposes where the standard available methods could be used instead.
4. If you need custom repository functionality:
   - Define the method in the appropriate repository interface
   - Implement the method in the corresponding concrete repository class
   - Apply `[IntentIgnore]` attribute to both declaration and implementation
   - Then call this method from your handler
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
   - Use this method in implementing the handler's Handle method (IMPORTANT).
5. Implement the handler's Handle method using the appropriate repository methods.

## Code Preservation Requirements (CRITICAL)
1. **NEVER remove or modify existing class members, methods, or properties**
2. **NEVER change existing method signatures or implementations**
3. **ONLY add new members when necessary (repository methods)**
4. **Preserve all existing attributes and code exactly as provided**

## Code File Modifications
1. You may modify ONLY:
   - The Handler class implementation (primarily the Handle method)
   - Repository interfaces (adding new methods only)
   - Repository concrete classes (implementing new methods only)
2. Preserve all existing code, attributes, and file paths exactly

## Input Code Files:
```json
{{{{$inputFilesJson}}}}
```

## Required Output Format
Your response MUST include:
1. The fully implemented handler class with the Handle method
2. Any modified repository interfaces (if you added methods)
3. Any modified repository concrete classes (if you added implementations)
4. All files must maintain their exact original paths
5. All existing code and attributes must be preserved unless explicitly modified

## Important Reminders
- NEVER remove or modify existing class members
- NEVER access DbContext or use Queryable() directly in handlers
- NEVER invoke repository methods that don't exist
- IF you add a new repository method, you MUST provide BOTH the interface declaration AND concrete implementation
- ALL new repository methods must be marked with `[IntentIgnore]`
- Performance and clean architecture are key priorities
";
        return prompt;
    }

    private FileChange[] GetInputFiles(IElement element)
    {
        var basePath = Path.GetFullPath(Path.Combine(_applicationConfig.DirectoryPath, _applicationConfig.OutputLocation));
        var correlations = MetadataOutputFileCorrelationsPersistable.TryLoad(_applicationConfig.FilePath);
        var outputLog = OutputLogPersistable.TryLoad(Path.Combine(_applicationConfig.DirectoryPath, $"{Path.GetFileNameWithoutExtension(_applicationConfig.FilePath)}.{OutputLogPersistable.FileExtension}"));

        var fileMap = correlations!.Files
            .SelectMany(x => x.Models, (file, model) => (File: file, Model: model))
            .GroupBy(x => x.Model.Id)
            .ToDictionary(x => x.Key, x => x.Select(i => Path.Combine(basePath, i.File.RelativePath)).ToList());

        var correlatedFiles = CorrelatedFiles(fileMap, element.Id);


        var files = new List<FileChange>();
        var queryHandlerFile = correlatedFiles.First(x => x.FileName.EndsWith("Handler"));
        files.Add(new FileChange(Path.GetRelativePath(basePath, queryHandlerFile.Path), "The Handler Class", queryHandlerFile.FileText));
        var otherCorrelatedFiles = correlatedFiles.Where(x => !x.FileName.EndsWith("Handler") && Path.GetExtension(x.Path) == ".cs");
        files.AddRange(otherCorrelatedFiles.Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "File supporting the Handler class", x.FileText)));
        
        if (element.TypeReference.ElementId != null)
        {
            var returnTypeFile = CorrelatedFiles(fileMap, element.TypeReference.ElementId);
            files.AddRange(returnTypeFile.Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "The return type for the Handler class", x.FileText)));
        }
        
        files.AddRange(GetRelatedEntities(element)
            .SelectMany(model => CorrelatedFiles(fileMap, model.Id)
                .Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "Related entity persistence support file", x.FileText))));

        if (outputLog != null)
        {
            outputLog.Load();
            var outputLogFileMap = outputLog!.FileLogs
                .GroupBy(x => x.CorrelationId)
                .ToDictionary(x => x.Key, x => x.Select(i => i.FilePath).ToList());

                ;
            files.AddRange(CorrelatedFiles(outputLogFileMap, "Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface")
                .Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "EF Repository Interface", x.FileText)));
            files.AddRange(CorrelatedFiles(outputLogFileMap, "Intent.EntityFrameworkCore.Repositories.RepositoryBase")
                .Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "EF Repository Base Implementation", x.FileText)));
        }

        return files.ToArray();
    }
    /**
## Adding code to the repository:
* Ignore this unless you plant to use the repository method in the handler.
* If there is no repository method that would be able to fetch out the required results in a performant way, then you may update the entity repository concrete and its interface with an appropriate method. You may need to introduce a Domain Object as the return type of these methods.
* In this case, if you added any methods, then you must add the [IntentIgnore] attribute to that method.
* Only add repository methods if one doesn't already exist that is appropriate and follows best practices.
* The repository interfaces are in the Domain / Core layer and you so cannot reference types in the other layers. This is important for if you created Domain Objects as a return type for the new repository.

     */

    private Kernel BuildSemanticKernel()
    {
        var settings = _applicationConfigurationProvider.GetSettings().GetChatDrivenDomainSettings();
        var model = string.IsNullOrWhiteSpace(settings.Model()) ? "gpt-4o" : settings.Model();
        var apiKey = settings.APIKey();
        // Create the Semantic Kernel instance with your LLM service.
        // Replace <your-openai-key> with your actual OpenAI API key and adjust the model name as needed.
        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging(b => b.AddProvider(new SoftwareFactoryLoggingProvider()).SetMinimumLevel(LogLevel.Trace));

        switch (settings.Provider().AsEnum())
        {
            case ChatDrivenDomainSettings.ProviderOptionsEnum.OpenAi:
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                }

                builder.Services.AddOpenAIChatCompletion(
                    modelId: model,
                    apiKey: apiKey ?? throw new Exception("No API Key defined. Locate the ChatDrivenDomainSettings App Settings or set the OPENAI_API_KEY environment variable."));
                break;
            case ChatDrivenDomainSettings.ProviderOptionsEnum.AzureOpenAi:
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
                }

                builder.Services.AddAzureOpenAIChatCompletion(
                    deploymentName: settings.DeploymentName(),
                    endpoint: settings.APIUrl(),
                    apiKey: apiKey ?? throw new Exception("No API Key defined. Locate the ChatDrivenDomainSettings App Settings or set the AZURE_OPENAI_API_KEY environment variable."),
                    modelId: model);
                break;
            case ChatDrivenDomainSettings.ProviderOptionsEnum.Ollama:
#pragma warning disable SKEXP0070
                builder.Services.AddOllamaChatCompletion(
                    new OllamaApiClient(
                        new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(10),
                            BaseAddress = new Uri(settings.APIUrl())
                        },
                        model)
                );
#pragma warning restore SKEXP0070
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Create a JSON schema format based on the FileChange array.
        // This helps ensure that the LLM returns a valid JSON array conforming to our expected structure.
        var kernel = builder.Build();
        return kernel;
    }

    private static ChatResponseFormat CreateJsonSchemaFormat()
    {
        return ChatResponseFormat.CreateJsonSchemaFormat(jsonSchemaFormatName: "movie_result",
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

    private static string Fail(string reason)
    {
        Logging.Log.Failure(reason);
        var errorObject = new { errorMessage = reason };
        var json = JsonSerializer.Serialize(errorObject, SerializerOptions);
        return json;
    }

    private static List<(string Path, string FileName, string FileText)> CorrelatedFiles(Dictionary<string, List<string>> fileMap, string modelId)
    {
        return fileMap[modelId].Select(x => (Path: x, FileName: Path.GetFileNameWithoutExtension(x), FileText: File.ReadAllText(x))).ToList();
    }

    private IEnumerable<ClassModel> GetRelatedEntities(IElement element)
    {
        var queriedEntity = element.AssociatedElements.FirstOrDefault(x => x.TypeReference.Element.IsClassModel())
            ?.TypeReference.Element.AsClassModel();
        if (queriedEntity == null)
        {
            return [];
        }
        var relatedClasses = new[] { queriedEntity }.Concat(queriedEntity.AssociatedClasses.Where(x => x.Class != null).Select(x => x.Class));
        return relatedClasses;
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