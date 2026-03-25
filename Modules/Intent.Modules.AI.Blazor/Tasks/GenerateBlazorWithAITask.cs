using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.AI.CodeGeneration;
using Intent.Modules.Common.AI.Configuration;
using Intent.Modules.Common.AI.Tasks;
using Intent.Modules.Common.Types.Api;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Intent.Modules.AI.Prompts.Tasks.GenerateBlazorWithAITask;

namespace Intent.Modules.AI.Prompts.Tasks;

#nullable enable

public class GenerateBlazorWithAITask : AiPromptBaseTask<PromptInputs>, IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly IGeneratedFilesProvider _fileProvider;

    public GenerateBlazorWithAITask(
        IApplicationConfigurationProvider applicationConfigurationProvider,
        IMetadataManager metadataManager,
        ISolutionConfig solution,
        IOutputRegistry outputRegistry,
        IUserSettingsProvider userSettingsProvider) : base(TaskTypeId, applicationConfigurationProvider, metadataManager, solution, outputRegistry, userSettingsProvider)
    {
        _applicationConfigurationProvider = applicationConfigurationProvider;
        _metadataManager = metadataManager;
        _fileProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
    }

    public new const string TaskTypeId = "Intent.Blazor.AI";

    public override string TaskTypeName => "Auto-Implement Blazor with AI";

    protected override IDesigner GetDesigner(string applicationId)
    {
        return _metadataManager.UserInterface(applicationId);
    }

    public record PromptInputs(
            string EnvironmentMetadata,
            string InputFilesJson,
            string UserProvidedContext,
            string ExamplesJson,
            string FilesToModifyJson,
            string AdditionalRules);

    protected override PromptInputs BuildPromptInputs(
        PromptArgs execArgs,
        IElement componentModel,
        PromptConfig promptTemplateConfig,
        PromptContext promptContext)
    {

        var inputFiles = GetInputFiles(componentModel, out var toModify)
            .Concat(promptContext.IncludeCodeFiles);

        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);
        var fileToModifyJson = JsonConvert.SerializeObject(
            toModify.Select(m => m.FilePath).ToArray());

        var exampleFiles = new List<ICodebaseFile>(promptContext.ExampleFiles);

        var templateFiles = promptContext.TemplateFiles;
        if (templateFiles.Any())
        {
            Logging.Log.Debug($"Adding Template Files ({templateFiles.Count()})");
            exampleFiles.AddRange(templateFiles);
        }

        var exampleJson = JsonConvert.SerializeObject(exampleFiles, Formatting.Indented);

        var metadata = promptContext.Metadata;

        var applicationConfig = _applicationConfigurationProvider.GetApplicationConfig();
        var blazorSettings = applicationConfig.ModuleSetting.FirstOrDefault(s => s.Id == "489a67db-31b2-4d51-96d7-52637c3795be");// Blazor Settings
        var prerendering = blazorSettings.GetSetting("d851b4d1-b230-461f-9873-80d6857fa175");// Prerendering
        var renderMode = blazorSettings.GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa");// RenderMode

        metadata["renderMode"] = GetRenderMode(renderMode?.Value);
        metadata["prerenderingMode"] = prerendering?.Value == "false" ? "disabled" : "enabled";

        var environmentMetadata = JsonConvert.SerializeObject(metadata, Formatting.Indented);

        var additionalRules = promptContext.AdditionalRules;
        if (string.IsNullOrWhiteSpace(additionalRules))
        {
            additionalRules = "None";
        }

        return new PromptInputs(
            EnvironmentMetadata: environmentMetadata,
            InputFilesJson: jsonInput,
            UserProvidedContext: promptContext.UserProvidedContext,
            ExamplesJson: exampleJson,
            FilesToModifyJson: fileToModifyJson,
            AdditionalRules: additionalRules);
    }
    protected override KernelArguments CreateKernelArguments(PromptInputs inputs)
    {
        return new KernelArguments
        {
            ["environmentMetadata"] = inputs.EnvironmentMetadata,
            ["inputFilesJson"] = inputs.InputFilesJson,
            ["userProvidedContext"] = inputs.UserProvidedContext,
            ["examples"] = inputs.ExamplesJson,
            ["filesToModifyJson"] = inputs.FilesToModifyJson,
            ["additionalRules"] = inputs.AdditionalRules,
            ["fileChangesSchema"] = FileChangesSchema.GetPromptInstructions()
        };
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