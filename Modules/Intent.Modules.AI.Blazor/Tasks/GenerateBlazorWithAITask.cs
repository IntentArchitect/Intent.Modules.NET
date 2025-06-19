using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Intent.Modules.AI.Prompts.Tasks;

public class GenerateBlazorWithAITask : IModuleTask
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

    public GenerateBlazorWithAITask(
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

    public string TaskTypeId => "Intent.Modules.AI.Blazor.Generate";
    public string TaskTypeName => "Auto-Implement Blazor with AI";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";

        _applicationConfig = _solution.GetApplicationConfig(applicationId);

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();

        var componentModel = _metadataManager.UserInterface(applicationId).Elements.Single(x => x.Id == elementId);
        var promptTemplate = GetTestPromptTemplate(userProvidedContext);
        var inputFiles = GetInputFiles(componentModel);

        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate);
        var result = requestFunction.InvokeAsync(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext
        }).Result;

        var aiResponse = result.ToString();
        var sanitizedAiResponse = aiResponse.Substring(aiResponse.IndexOf("```", StringComparison.Ordinal)).Replace("```json", "").Replace("```", "");
        FileChangesResult fileChangesResult = JsonConvert.DeserializeObject<FileChangesResult>(sanitizedAiResponse);

        // Output the updated file changes.
        var applicationConfig = _solution.GetApplicationConfig(args[0]);
        var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
        foreach (var fileChange in fileChangesResult.FileChanges)
        {
            _outputRegistry.Register(Path.Combine(basePath, fileChange.FilePath), fileChange.Content);
        }

        return "success";
    }


    private string GetTestPromptTemplate(string userPrompt)
    {
        var prompt =
            $$$"""
               ## Role and Context
               You are a senior C# Blazor developer specializing in MudBlazor WASM applications.

               ## Critical Requirements (MUST FOLLOW)
               - PRESERVE all [IntentManaged] attributes on existing constructors, classes, or files
               - CHECK AND ENSURE all bindings between `.razor` and `.razor.cs` files compile correctly
               - PRESERVE existing code in `.razor.cs` files - you may only ADD code, never modify existing code
               - NEVER ADD COMMENTS to any code
               - Output ONLY valid JSON matching the specified schema

               ## Task Instructions
               {{{(userPrompt == "None"
                   ? "Create an appropriate MudBlazor view in the `.razor` file based on the provided `.razor.cs` file"
                   : $"Update the component files according to this instruction: {userPrompt}")}}}.

               ## File Modification Rules
               **For .razor files:**
               - Add only razor markup
               - Do NOT add @code directives
               - Ensure all bindings reference methods/fields that exist in the `.razor.cs` file

               **For .razor.cs files:**
               - Add missing methods/fields required by `.razor` file bindings
               - Add any new code directives with `[IntentIgnore]` attribute
               - Include necessary using statements
               - Utilize available services for data operations

               ## Development Guidelines
               1. Read and understand all provided input code files and their interactions
               2. Use available services when providing data functionality is needed
               3. Ensure all code compiles by verifying binding consistency between files

               ## Input Code Files
               ```json
               {{$inputFilesJson}}
               ```

               ## Additional User Context
               {{$userProvidedContext}}

               ## Required Output Format
               Respond with JSON matching this exact schema:
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
               """;
        return prompt;
    }

    private List<ICodebaseFile> GetInputFiles(IElement element)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        var inputFiles = filesProvider.GetFilesForMetadata(element).ToList();
        //if (element.TypeReference.ElementId != null)
        //{
        //    inputFiles.AddRange(filesProvider.GetFilesForMetadata(element.TypeReference.Element));
        //}
        foreach (var dto in element.ChildElements.Where(x => x.TypeReference != null && x.TypeReference.Element != null).Select(x => x.TypeReference.Element))
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(dto));
        }

        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
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