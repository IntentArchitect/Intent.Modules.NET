using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Output;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.AI.ChatDrivenDomain.Settings;
using Intent.Modules.AI.Prompts.Utils;
using Intent.Modules.Common.Templates;
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
        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = BuildSemanticKernel();
        var chatResponseFormat = CreateJsonSchemaFormat();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = chatResponseFormat
        };

        var input = CreatePrompt(args[0], args[1]);
        var jsonInput = JsonConvert.SerializeObject(input.FileInputs, Formatting.Indented);
        var requestFunction = kernel.CreateFunctionFromPrompt(input.Prompt, executionSettings);
        var result = requestFunction.InvokeAsync(kernel, new KernelArguments()
        {
            ["prompt"] = jsonInput
        }).Result;


        FileChangesResult fileChangesResult = JsonConvert.DeserializeObject<FileChangesResult>(result.ToString());

        // Output the updated file changes.
        var applicationConfig = _solution.GetApplicationConfig(args[0]);
        var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
        foreach (var fileChange in fileChangesResult.FileChanges)
        {
            Logging.Log.Info($"File: {Path.Combine(basePath, fileChange.FilePath)}");
            Logging.Log.Info($"Updated Code:\n{fileChange.Content}");
            Logging.Log.Info(new string('-', 50));
            _outputRegistry.Register(Path.Combine(basePath, fileChange.FilePath), fileChange.Content);
        }

        return "success";
    }

    private (string Prompt, FileChange[] FileInputs) CreatePrompt(string applicationId, string elementId)
    {
        var applicationConfig = _solution.GetApplicationConfig(applicationId);
        var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
        var correlations = MetadataOutputFileCorrelationsPersistable.TryLoad(applicationConfig.FilePath);
        var outputLog = OutputLogPersistable.TryLoad(Path.Combine(applicationConfig.DirectoryPath, $"{Path.GetFileNameWithoutExtension(applicationConfig.FilePath)}.{OutputLogPersistable.FileExtension}"));

        var queryModel = _metadataManager.Services(applicationId).GetQueryModels().Single(x => x.Id == elementId);

        var fileMap = correlations!.Files
            .SelectMany(x => x.Models, (file, model) => (File: file, Model: model))
            .GroupBy(x => x.Model.Id)
            .ToDictionary(x => x.Key, x => x.Select(i => Path.Combine(basePath, i.File.RelativePath)).ToList());

        var correlatedFiles = CorrelatedFiles(fileMap, queryModel.Id);


        var files = new List<FileChange>();
        var queryHandlerFile = correlatedFiles.First(x => x.FileName.EndsWith("Handler"));
        files.Add(new FileChange(Path.GetRelativePath(basePath, queryHandlerFile.Path), "The Query Handler", queryHandlerFile.FileText));
        var otherCorrelatedFiles = correlatedFiles.Where(x => !x.FileName.EndsWith("Handler") && Path.GetExtension(x.Path) == ".cs");
        files.AddRange(otherCorrelatedFiles.Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "File supporting the Query Handler", x.FileText)));
        var returnTypeFile = CorrelatedFiles(fileMap, queryModel.TypeReference.ElementId);
        files.AddRange(returnTypeFile.Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "The return type for the Query Handler", x.FileText)));
        files.AddRange(GetRelatedEntities(queryModel).SelectMany(model => CorrelatedFiles(fileMap, model.Id).Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "Related entity persistence support file", x.FileText))));

        if (outputLog != null)
        {
            outputLog.Load();
            var outputLogFileMap = outputLog!.FileLogs
                .GroupBy(x => x.CorrelationId)
                .ToDictionary(x => x.Key, x => x.Select(i => i.FilePath).ToList());

                ;
            files.AddRange(CorrelatedFiles(outputLogFileMap, "Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface")
                .Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "EF Repository Interface", x.FileText)));
            //files.AddRange(CorrelatedFiles(outputLogFileMap, "Intent.EntityFrameworkCore.Repositories.RepositoryBase")
            //    .Select(x => new FileChange(Path.GetRelativePath(basePath, x.Path), "EF Repository Base Implementation", x.FileText)));
        }

        var prompt = @$"
## Background Information
* You're a senior C# developer. Your job is to implement business logic in an optimal way, following best practices.
* The architecture is a clean architecture using EF Core and a Repository pattern.

## Instruction:
* I'm going to provide you an array of objects which represent C# files. One of the C# classes is called {queryHandlerFile.FileName} which is a handler for a MediatR request. 
* I want you to implement the Handle method to fetch out the {queryHandlerFile.FileName.RemovePrefix("Get").RemoveSuffix("Query", "Handler").ToSentenceCase()} appropriately using best practices. This is your main objective.
* You can inject in any entity repository that you may require to complete your job.
* If you realize that there isn't a performant way to achieve this with the existing repository methods, 
    you may add new repository methods to the entity repository interface and concrete, but only if necessary. If you do this, you must add an `[IntentIgnore]` attribute over the methods.

## Important information:
* There is a repository interface and concrete for each
* Repository interfaces for each entity inherit from IEFRepository.
* Methods in IEFRepository are already implemented in the RepositoryBase class

## Rules to follow:
* The input files that you receive will have a file path. You must maintain this filepath in the output with exact precision.
* Inspect each code file and understand how they all fit together.
* Read the code comments in the input files to help understand what the intention of the code is.
* You must keep all attributes unchanged on the class and methods.
* Include entities as part of the repository calls if it will be more performant.

## Input Code Files (JSON):

```
{{{{$prompt}}}}
```";
        return (prompt, files.ToArray());
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

    private IEnumerable<ClassModel> GetRelatedEntities(QueryModel queryModel)
    {
        var queriedEntity = queryModel.QueryEntityActions().FirstOrDefault()?.TypeReference.Element.AsClassModel();
        if (queriedEntity == null)
        {
            return [];
        }
        var sb = new StringBuilder();
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