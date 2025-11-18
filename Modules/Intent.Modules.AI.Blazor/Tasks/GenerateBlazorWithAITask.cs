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
using Intent.Modelers.Services.Api;
using System.Diagnostics;
using Intent.Modules.AI.Blazor.Tasks.Config;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.AI.CodeGeneration;
using Intent.Modules.Common.AI.Extensions;
using Intent.Modules.Common.AI.Settings;
using Intent.Modules.Common.Types.Api;

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
        var userProvidedContext = !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";
        var exampleComponentIds = !string.IsNullOrWhiteSpace(args[3]) ? JsonConvert.DeserializeObject<string[]>(args[3]) : [];
        var provider = new AISettings.ProviderOptions(args[4]).AsEnum();
        var modelId = args[5];
        var thinkingLevel = args[6];
        var templateId = !string.IsNullOrWhiteSpace(args[7]) ? args[7] : null;

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel(modelId, provider, null);
        var componentModel = _metadataManager.UserInterface(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
        if (componentModel == null)
        {
            throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
        }

        //Append the Component Model Comment for Persistent Instance Based Context - i.e.
        if (!string.IsNullOrEmpty(componentModel.Comment))
        {
            userProvidedContext = componentModel.Comment + Environment.NewLine + userProvidedContext;
        }

        var promptTemplateConfig = PromptConfig.TryLoad(_solution.SolutionRootLocation, _applicationConfigurationProvider.GetApplicationConfig().Name);

        var disposables = new List<IAsyncDisposable>();
        if (promptTemplateConfig?.McpServers.Any() == true)
        {
            var mcpDisposables = McpHelper.WireUpMcpAsync(kernel, promptTemplateConfig.McpServers)
                .GetAwaiter()
                .GetResult();
            disposables.AddRange(mcpDisposables);
        }

        try
        {
            if (promptTemplateConfig is null)
            {
                Logging.Log.Warning("No Prompt Templates Found.");
                promptTemplateConfig = new PromptConfig();
            }
            if (templateId is not null && promptTemplateConfig.Templates.FirstOrDefault(t => t.Id == templateId) == null)
            {
                throw new Exception($"Could not find Template {templateId}.");
            }

            var inputFiles = GetInputFiles(componentModel, out var toModify)
                .Concat(LoadAdditionalFiles(promptTemplateConfig.GetInputFile(templateId)));
            var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);
            var fileToModifyJson = JsonConvert.SerializeObject(toModify.Select(m => m.FilePath).ToArray());

            var exampleFiles = exampleComponentIds?.SelectMany(componentId =>
            {
                var component = _metadataManager.UserInterface(applicationId).Elements.FirstOrDefault(x => x.Id == componentId);
                if (component == null)
                {
                    return [];
                }
                return _fileProvider.GetFilesForMetadata(component);
            }).ToList() ?? [];

            var applicationConfig = _solution.GetApplicationConfig(args[0]);

            var templateFiles = promptTemplateConfig.LoadTemplateFiles(templateId);
            if (templateFiles.Any())
            {
                Logging.Log.Debug($"Adding Template Files ({templateFiles.Count()})");
                exampleFiles.AddRange(templateFiles);
            }

            var environmentMetadata = GetEnvironmentMetadata(applicationConfig, promptTemplateConfig.GetMetadata(templateId));
            var exampleJson = JsonConvert.SerializeObject(exampleFiles, Formatting.Indented);
            var additionalRules = promptTemplateConfig.GetAdditionalRules(templateId);
            var promptTemplate = GetPromptTemplate();

            if (string.IsNullOrEmpty(userProvidedContext))
            {
                userProvidedContext = "None";
            }
            if (string.IsNullOrEmpty(additionalRules))
            {
                additionalRules = "None";
            }

            var fileChangesResult = kernel.InvokeFileChangesPrompt(
                promptTemplate: promptTemplate,
                thinkingLevel: thinkingLevel,
                arguments: new KernelArguments()
                {
                    ["environmentMetadata"] = environmentMetadata,
                    ["inputFilesJson"] = jsonInput,
                    ["userProvidedContext"] = userProvidedContext,
                    ["targetFileName"] = componentModel.Name + "Handler",
                    ["examples"] = exampleJson,
                    ["filesToModifyJson"] = fileToModifyJson,
                    ["additionalRules"] = additionalRules,
                    ["fileChangesSchema"] = FileChangesSchema.GetPromptInstructions()
                });

            // Output the updated file changes.
            var basePath = Path.GetFullPath(Path.Combine(applicationConfig.DirectoryPath, applicationConfig.OutputLocation));
            foreach (var fileChange in fileChangesResult.FileChanges)
            {
                _outputRegistry.Register(Path.Combine(basePath, fileChange.FilePath), fileChange.Content);
            }

            return "success";
        }
        finally
        {
            foreach (var d in disposables)
            {
                try
                {
                    d.DisposeAsync().AsTask().GetAwaiter().GetResult();
                }
                catch { /* log if needed */ }
            }
        }
    }

    private IEnumerable<ICodebaseFile> LoadAdditionalFiles(IEnumerable<string> filenames)
    {
        foreach (var filename in filenames)
        {
            if (!File.Exists(filename))
            {
                Logging.Log.Warning($"Unable to find Prompt Template file : {filename}");
                continue;
            }
            yield return new PromptFile(filename, File.ReadAllText(filename));
        }
    }

    private string GetEnvironmentMetadata(IApplicationConfig applicationConfig, Dictionary<string, object> promptTempalteMetadata)
    {
        var blazorSettings = applicationConfig.ModuleSetting.FirstOrDefault(s => s.Id == "489a67db-31b2-4d51-96d7-52637c3795be");// Blazor Settings
        var prerendering = blazorSettings.GetSetting("d851b4d1-b230-461f-9873-80d6857fa175");// Prerendering
        var renderMode = blazorSettings.GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa");// RenderMode

        // Build a dictionary so values can be easily swapped or extended
        var metadata = new Dictionary<string, object>
        {
            ["renderMode"] = GetRenderMode(renderMode?.Value),
            ["prerenderingMode"] = prerendering?.Value == "false" ? "disabled":"enabled",
        };

        metadata = DictionaryHelper.MergeDictionaries(metadata, promptTempalteMetadata);

        return JsonConvert.SerializeObject(metadata, Formatting.Indented);
    }

    private static string GetPromptTemplate()
    {
        const string promptTemplate =
            """
            ## Role and Context
            You are a senior C# Blazor Engineer. You are an expert in UI layout and always implement exceptional modern user interfaces that follow best practices.
            
            ## Environment Metadata
            {{$environmentMetadata}}

            ## Primary Objective
            Completely implement the Blazor component by reading and updating the `.razor` file, and `.razor.cs` file if necessary.

            ## Code File Modification Rules
            1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
            2. Add using clauses for code files that you use
            3. (CRITICAL) Read and understand the code in all provided Input Code Files. Understand how these code files interact with one another.
            4. If services to provide data are available, use them.
            5. If you bind to a field or method from the `.razor` file, you must make sure that the `.razor.cs` file has that code declared. If it doesn't add it appropriately.
            6. (CRITICAL) CHECK AND ENSURE AND CORRECT all bindings between the `.razor` and `.razor.cs`. The code must compile!
            7. **Only modify files listed in "Files Allowed To Modify". All other Input Code Files are read-only.**
            
            ## Important Rules
            * The `.razor.cs` file is the C# backing file for the `.razor` file.
            * (IMPORTANT) Only add razor markup to the `.razor` file. If you want to add C# code, add it to the `.razor.cs` file. Therefore, do NOT add a @code directive to the `.razor` file.
            * PRESERVE existing code in the `.razor.cs` file. You may add code, but you are not allowed to change the existing code (IMPORTANT) in the .`razor.cs` file!
            * (IMPORTANT)NEVER ADD COMMENTS, not even code comments from templates or examples
            * The supplied Example Components are examples to guide your implementation 
            * Don't display technical ids to end users like Guids
            
            ## Additional Rules
            {{$additionalRules}}

            ## Files Allowed To Modify
            {{$filesToModifyJson}}

            ## Input Code Files
            {{$inputFilesJson}}
            
            ## User Context
            {{$userProvidedContext}}

            ## Validation Checklist (perform before output)
            - [ ] Every `FileChanges[i].FilePath` exists in "Files Allowed To Modify".
            - [ ] All `@bind` and event handlers in `.razor` are defined in `.razor.cs`.
            - [ ] No `@code` blocks in `.razor`.
            - [ ] `[IntentManaged]` attributes preserved.
            - [ ] Code compiles with added `using` directives.
            - [ ] No Comments were added to the code.

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
            
            {{$previousError}}
            """;

        return promptTemplate;
    }

    private List<ICodebaseFile> GetInputFiles(IElement element, out List<ICodebaseFile> filesToModify)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        var inputFiles = filesProvider.GetFilesForMetadata(element).ToList();
        filesToModify = [.. inputFiles];
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
                inputFiles.AddRange(GetCodeFilesForElement(filesProvider, associationEnd.TypeReference.Element));

                if ((associationEnd.TypeReference.Element.IsCommandModel() || associationEnd.TypeReference.Element.IsQueryModel())
                    && associationEnd.TypeReference.Element.TypeReference.Element.IsDTOModel())
                {
                    inputFiles.AddRange(GetCodeFilesForElement(filesProvider, associationEnd.TypeReference.Element.TypeReference.Element));
                }
            }
        }
        inputFiles.AddRange(_fileProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResult"));
        inputFiles.AddRange(_fileProvider.GetFilesForTemplate("Intent.Blazor.Templates.Server.ServerImportsRazorTemplate"));        
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        //inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
    }

    private List<ICodebaseFile> GetCodeFilesForElement(IGeneratedFilesProvider filesProvider, ICanBeReferencedType element)
    {
        var inputFiles = new List<ICodebaseFile>();
        inputFiles.AddRange(filesProvider.GetFilesForMetadata(element));

        foreach (var childDto in GetAllChildren(element, e => e.IsDTOModel() || e.IsEnumModel()))
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(childDto));
        }
        return inputFiles;
    }

    private ICanBeReferencedType[] GetAllChildren(ICanBeReferencedType element, Func<ICanBeReferencedType, bool> match)
    {
        var children = new List<ICanBeReferencedType>();
        if (element is IElement e)
        {
            foreach (var x in e.ChildElements)
            {
                var type = x.TypeReference?.Element;
                if (type is not null && match(type))
                {
                    children.Add(type);
                    children.AddRange(GetAllChildren(type, match));
                }
            }
        }

        return children.ToArray();
    }

    private static string GetRenderMode(string? value)
    {
        switch (value)
        {
            case "interactive-server": return "InteractiveServer";
            case "interactive-auto": return "InteractiveAuto";
            case "interactive-web-assembly":
            default:
                return "InteractiveWebAssembly";
        }
    }


}