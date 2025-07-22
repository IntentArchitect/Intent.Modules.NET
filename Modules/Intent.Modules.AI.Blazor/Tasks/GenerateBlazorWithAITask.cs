using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Intent.Modules.AI.Blazor.Tasks.Helpers;

namespace Intent.Modules.AI.Prompts.Tasks;

#nullable enable

public class GenerateBlazorWithAITask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;
    private readonly IGeneratedFilesProvider _fileProvider;

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
        _fileProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
    }

    public string TaskTypeId => "Intent.Modules.AI.Blazor.Generate";
    public string TaskTypeName => "Auto-Implement Blazor with AI";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";
        var exampleComponentIds = args.Length > 3 && !string.IsNullOrWhiteSpace(args[3]) ? JsonConvert.DeserializeObject<string[]>(args[3]) : [];

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();
        var componentModel = _metadataManager.UserInterface(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
        if (componentModel == null)
        {
            throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
        }
        var inputFiles = GetInputFiles(componentModel);
        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

        var exampleFiles = exampleComponentIds?.SelectMany(componentId =>
        {
            var component = _metadataManager.UserInterface(applicationId).Elements.FirstOrDefault(x => x.Id == componentId);
            if (component == null)
            {
                return [];
            }
            return _fileProvider.GetFilesForMetadata(component);
        }).ToArray() ?? [];

        var exampleJson = JsonConvert.SerializeObject(exampleFiles, Formatting.Indented);

        var requestFunction = CreatePromptFunction(kernel);
        var fileChangesResult = requestFunction.InvokeFileChangesPrompt(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext,
            ["targetFileName"] = componentModel.Name + "Handler",
            ["examples"] = exampleJson
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
            You are a senior C# Blazor developer specializing MudBlazor in WASM mode. You are an expert in UI layout and always implement exceptional modern user interfaces that follow best practices.

            ## Primary Objective
            Completely implement the MudBlazor component by reading and updating the `.razor` file, and `.razor.cs` file if necessary.

            ## Code File Modification Rules
            1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
            2. You may only create or update the test file
            3. Add using clauses for code files that you use
            4. (CRITICAL) Read and understand the code in all provided Input Code Files. Understand how these code files interact with one another.
            5. If services to provide data are available, use them.
            6. If you bind to a field or method from the `.razor` file, you must make sure that the `.razor.cs` file has that code declared. If it doesn't add it appropriately.
            7. (CRITICAL) CHECK AND ENSURE AND CORRECT all bindings between the `.razor` and `.razor.cs`. The code must compile!

            ## Important Rules
            * The `.razor.cs` file is the C# backing file for the `.razor` file.
            * (IMPORTANT) Only add razor markup to the `.razor` file. If you want to add C# code, add it to the `.razor.cs` file. Therefore, do NOT add a @code directive to the `.razor` file.
            * PRESERVE existing code in the `.razor.cs` file. You may add code, but you are not allowed to change the existing code (IMPORTANT) in the .`razor.cs` file!
            * ONLY IF YOU add any code directives in the `.razor.cs` file, MUST you add an `[IntentIgnore]` attribute to that directive.
            * NEVER ADD COMMENTS

            ## Input Code Files
            {{$inputFilesJson}}

            ## User Context
            {{$userProvidedContext}}

            ## Previous Error Message:
            {{$previousError}}

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
            
            ## Example Components:
            {{$examples}}
            """;
        
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate);
        return requestFunction;
    }

    private List<ICodebaseFile> GetInputFiles(IElement element)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        var inputFiles = filesProvider.GetFilesForMetadata(element).ToList();
        foreach (var dto in element.ChildElements)
        {
            if (dto.TypeReference != null && dto.TypeReference.Element != null)
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(dto.TypeReference.Element));
                foreach (var genericTypeParameter in dto.TypeReference.GenericTypeParameters.Where(x => x.Element != null))
                {
                    inputFiles.AddRange(filesProvider.GetFilesForMetadata(genericTypeParameter.Element));
                }
            }

            foreach (var associationEnd in dto.AssociatedElements)
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(associationEnd.TypeReference.Element));
            }
        }

        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
    }
}