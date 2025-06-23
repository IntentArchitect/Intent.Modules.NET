using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Intent.Modules.AI.Prompts.Tasks;

#nullable enable

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
    public string TaskTypeName => "Auto-Implement Unit Tests with AI Task";
    public int Order => 0;
    
    private const int MaxAttempts = 2;

    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";

        _applicationConfig = _solution.GetApplicationConfig(applicationId);

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();

        var queryModel = _metadataManager.Services(applicationId).Elements.Single(x => x.Id == elementId);
        var promptTemplate = GetTestPromptTemplate(queryModel);
        var inputFiles = GetInputFiles(queryModel);

        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate);
        
        FileChangesResult? fileChangesResult = null;
        var previousError = string.Empty;
        
        for (var i = 0; i < MaxAttempts; i++) {
            var result = requestFunction.InvokeAsync(kernel, new KernelArguments()
            {
                ["inputFilesJson"] = jsonInput,
                ["userProvidedContext"] = userProvidedContext,
                ["previousError"] = previousError
            }).Result;

            if (TryGetFileChangesResult(result, out fileChangesResult))
            {
                break;
            }

            previousError = "The previous prompt execution failed. You need to return ONLY the JSON response in the defined schema format!";
        }

        if (fileChangesResult is null)
        {
            throw new Exception("AI Prompt failed to return a valid response.");
        }

        // Output the updated file changes.
        var applicationConfig = _solution.GetApplicationConfig(args[0]);
        var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
        foreach (var fileChange in fileChangesResult.FileChanges)
        {
            _outputRegistry.Register(Path.Combine(basePath, fileChange.FilePath), fileChange.Content);
        }

        return "success";
    }

    /// <summary>
    /// AI Models / APIs that doesn't support JSON Schema Formatting (like OpenAI) will not completely give you a "clean result".
    /// This will attempt to cut through the thoughts it writes out to get to the payload.
    /// </summary>
    private static bool TryGetFileChangesResult(FunctionResult aiInvocationResponse, [NotNullWhen(true)] out FileChangesResult? fileChanges)
    {
        try
        {
            var textResponse = aiInvocationResponse.ToString();
            string payload;
            var jsonMarkdownStart = textResponse.IndexOf("```", StringComparison.Ordinal);
            if (jsonMarkdownStart < 0)
            {
                // Assume AI didn't respond with ``` wrappers.
                // JSON Deserializer will pick up if it is not valid.
                payload = textResponse;
            }
            else
            {
                var sanitized = textResponse.Substring(jsonMarkdownStart).Replace("```json", "").Replace("```", "");
                payload = sanitized;
            }

            var fileChangesResult = JsonConvert.DeserializeObject<FileChangesResult>(payload);
            if (fileChangesResult is null)
            {
                fileChanges = null;
                return false;
            }

            fileChanges = fileChangesResult;
            return true;
        }
        catch
        {
            fileChanges = null;
            return false;
        }
    }

    private string GetTestPromptTemplate(IElement model)
    {
        var targetFileName = model.Name + "Handler";
        var mockFramework = GetMockFramework();
        var relativePath = string.Join('/', model.GetParentPath().Select(x => x.Name));

        var prompt =
            $$$"""
               ## Role and Context
               You are a senior C# developer specializing in clean architecture with MediatR, FluentValidation, and Entity Framework Core, implementing testing logic in systems that follow the repository pattern.
               
               ## Task
               Create or update unit tests that comprehensively test the Handle method using xUnit and {{{mockFramework}}} in the {{{targetFileName}}} class file.
               
               ## Input Data
               - Input Code Files: {{$inputFilesJson}}
               - Target File Name: Use the mocking framework and target filename provided in the input
               - User Context: {{$userProvidedContext}}
               
               ## Critical Constraints (MUST FOLLOW)
               1. PRESERVE all [IntentManaged] Attributes on existing test file's constructor, class, or file
               2. NEVER remove or modify existing class members, methods, properties, or their attributes
               3. NEVER change existing method signatures or implementations
               4. ONLY add new test methods - never alter existing ones
               5. Add using clauses for any new dependencies
               6. Provide ONLY the JSON response with FileChanges array containing modified files with exact original file paths
               
               ## File Modification Rules
               - You may only create or update the test file
               - File path: Use appropriate Tests project from .sln with relative path structure
               - Read and understand all provided code files and their interactions
               - If existing file exists, update according to preservation constraints above
               
               ## Domain Knowledge
               - Repositories assign entity IDs when SaveChangesAsync is called
               - Collections on entities cannot be treated as arrays
               - Use static 'Create' constructor for DTOs
               - FluentValidations don't occur in handlers (don't test for validation)
               
               ## Test Pattern Example
               ```csharp
               public async Task Handle_Should_Create_Buyer_When_Command_Is_Valid()
               {
                   // Arrange
                   var command = new CreateBuyerCommand("John", "Doe", "john.doe@example.com", true);
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
               }
               ```
               
               ## Output Format (CRITICAL - FOLLOW EXACTLY)
               The file must have an appropriate path in the appropriate Tests project. 
               Look for a project in the .sln file that would be appropriate and use the following relative path: '{{{relativePath}}}'.
               
               **Response Format:** Provide ONLY the JSON response schema with FileChanges array containing modified files with exact original file paths:
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
               
               ## Previous Error Message:
               {{$previousError}}
               """;
        return prompt;
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

        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
    }

    private string GetMockFramework() 
    {
        var defaultMock = "Moq";

        var unitTestGroup = _applicationConfigurationProvider.GetSettings().GetGroup("d62269ea-8e64-44a0-8392-e1a69da7c960");

        if(unitTestGroup is null)
        {
            return defaultMock;
        }

        return unitTestGroup.GetSetting("115c28bc-a4c8-4b30-bd00-2e320fee77dc")?.Value ?? defaultMock;
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