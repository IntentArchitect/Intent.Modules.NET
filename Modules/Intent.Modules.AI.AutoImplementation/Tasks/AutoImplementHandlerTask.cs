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
using Newtonsoft.Json;

namespace Intent.Modules.AI.Prompts.Tasks;

#nullable enable

public class AutoImplementHandlerTask : IModuleTask
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

    public AutoImplementHandlerTask(
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

    public string TaskTypeId => "Intent.Modules.AI.Prompts.CreateMediatRHandlerPrompt";
    public string TaskTypeName => "Auto-Implementation with AI Task";
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

        var element = _metadataManager.Services(applicationId).Elements.Single(x => x.Id == elementId);
        var promptTemplate = GetPromptTemplate(element);

        var inputFiles = GetInputFiles(element);

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

    private static string GetPromptTemplate(IElement model)
    {
        var targetFileName = model.Name + "Handler";
        var prompt =
            $$$"""
               ## Role and Context
               You are a senior C# developer specializing in clean architecture with Entity Framework Core, implementing business logic that strictly follows the repository pattern.

               ## Primary Objective
               Implement the `Handle` method in the {{{targetFileName}}} class following the step-by-step process below.

               ## Implementation Process
               1. **Analysis Phase**: Study all provided code files to understand their relationships and identify the appropriate repository interface and methods for this use case.

               2. **Method Selection Decision**: 
                  - If existing repository methods can accomplish the task efficiently → proceed directly to Step 4 (Implementation)
                  - If you need aggregated data (Count, Sum, Average, etc.) that existing methods cannot provide → proceed to Step 3

               3. **Repository Extension** (only if Step 2 requires it):
                  - Add a properly named method to the appropriate repository interface
                  - Implement the method in the concrete repository class using proper EF Core code
                  - Mark both interface declaration and implementation with `[IntentIgnore]` attribute

               4. **Handler Implementation**: Implement the Handle method using the selected repository methods.

               ## Required Output Format
               Respond ONLY with JSON that matches the following schema:

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

               ## Additional Context (Optional)
               {{$userProvidedContext}}

               ## Input Code Files:
               {{$inputFilesJson}}
               
               ## Previous Error Message:
               {{$previousError}}

               ## CRITICAL CONSTRAINTS - NEVER VIOLATE:
               1. **Architecture Violations:** NEVER use Queryable() or access DbContext directly from handlers.
               2. **Code Preservation:** NEVER remove, modify, or comment existing class members, methods, properties, or attributes.
               3. **Method Preference:** NEVER create new repository methods when existing ones can handle the use case.
               4. **Repository Contract:** Repository methods cannot return DTOs - define separate data contracts if needed.
               5. **Implementation Completeness:** IF you add repository methods, you MUST provide BOTH interface declaration AND concrete implementation with [IntentIgnore] attributes.
               6. **Response Format:** Provide ONLY the JSON response with FileChanges array containing modified files with exact original file paths.
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
        inputFiles.AddRange(GetRelatedDomainEntities(element).SelectMany(x => filesProvider.GetFilesForMetadata(x.InternalElement)));

        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));

        return inputFiles;
    }

    private IEnumerable<ClassModel> GetRelatedDomainEntities(IElement element)
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