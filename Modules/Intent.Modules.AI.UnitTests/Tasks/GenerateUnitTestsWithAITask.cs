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
using Intent.Modules.Common.AI;
using Intent.Modules.VisualStudio.Projects.Api;
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

public class GenerateUnitTestsWithAITask : IModuleTask
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

    public GenerateUnitTestsWithAITask(
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

    public string TaskTypeId => "Intent.Modules.AI.UnitTests.Generate";
    public string TaskTypeName => "Create Prompt for Handler";
    public int Order => 0;

    [Experimental("SKEXP0010")]
    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        _applicationConfig = _solution.GetApplicationConfig(applicationId);

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();
        var chatResponseFormat = CreateJsonSchemaFormat();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = chatResponseFormat
        };

        var queryModel = _metadataManager.Services(applicationId).Elements.Single(x => x.Id == elementId);
        var promptTemplate = GetTestPromptTemplate(queryModel);
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


    private string GetTestPromptTemplate(IElement model)
    {
        var targetFileName = model.Name + "Handler";
        var prompt = @$"
## Role and Context
You are a senior C# developer specializing in clean architecture with Entity Framework Core. You're implementing business logic in a system that strictly follows the repository pattern.

## Primary Objective
Write me a set of unit tests that comprehensively test the `Handle` method using xUnit and Moq in the {targetFileName} class.

## Code Preservation Requirements (CRITICAL)
1. **NEVER remove or modify existing class members, methods, or properties**
2. **NEVER change existing method signatures or implementations**
3. **ONLY add new members when necessary (repository methods)**
4. **Preserve all existing attributes and code exactly as provided**

## Code File Modifications
1. You may only create the test file
2. Preserve all existing code, attributes, and file paths exactly
3. Add using clauses for code files that you use

## Input Code Files:
```json
{{{{$inputFilesJson}}}}
```


## Required Output Format
Your response MUST include:
1. Your test file as pure code (no markdown).
2. The file must have an appropriate path in the appropriate Tests project. Look for a project in the .sln file that would be appropriate and mirror the relative path of the class that is being tested.

## Important things to understand
- Repositories will assign an Id to entities when `SaveChangesAsync` is called.
- Collections on entities cannot be treated like arrays.

## Examples
1. Here's an example of a creation handler:
```
public async Task Handle_Should_Create_Buyer_When_Command_Is_Valid()
{{
    // Arrange
    var command = new CreateBuyerCommand(""John"", ""Doe"", ""john.doe@example.com"", true);
    Buyer buyer = null;
    var buyerId = Guid.NewGuid();

    _buyerRepositoryMock
        .Setup(repo => repo.Add(It.IsAny<Buyer>()))
        .Callback<Buyer>(b => buyer = b);
    _buyerRepositoryMock
        .Setup(repo => repo.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .Callback(() => buyer!.Id = buyerId)
        .ReturnsAsync(1);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    _buyerRepositoryMock.Verify(repo => repo.Add(It.IsAny<Buyer>()), Times.Once);
    _buyerRepositoryMock.Verify(repo => repo.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    Assert.NotEqual(Guid.Empty, result);
    Assert.NotNull(buyer);
    Assert.Equal(command.Name, buyer.Name);
    Assert.Equal(command.Surname, buyer.Surname);
    Assert.Equal(command.Email, buyer.Email);
}}
```
";
        return prompt;
    }

    private List<ICodebaseFile> GetInputFiles(IElement element)
    {
        var codebaseAccessor = _applicationConfigurationProvider.GetTrackedCodebaseFiles();
        var inputFiles = codebaseAccessor.GetFilesForMetadata(element).ToList();
        if (element.TypeReference.ElementId != null)
        {
            inputFiles.AddRange(codebaseAccessor.GetFilesForMetadata(element.TypeReference.Element));
        }

        inputFiles.AddRange(codebaseAccessor.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        inputFiles.AddRange(codebaseAccessor.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        inputFiles.AddRange(codebaseAccessor.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        inputFiles.AddRange(codebaseAccessor.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
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