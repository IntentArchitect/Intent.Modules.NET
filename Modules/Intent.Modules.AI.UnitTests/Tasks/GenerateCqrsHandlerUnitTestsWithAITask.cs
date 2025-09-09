using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Intent.Modules.AI.UnitTests.Tasks.Helpers;

namespace Intent.Modules.AI.Prompts.Tasks;

#nullable enable

public class GenerateCqrsHandlerUnitTestsWithAITask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    public GenerateCqrsHandlerUnitTestsWithAITask(
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

    public string TaskTypeId => "Intent.Modules.AI.UnitTests.Cqrs.Generate";
    public string TaskTypeName => "Auto-Implement Unit Tests with AI Task (CQRS Handler)";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        var applicationId = args[0];
        var elementId = args[1];
        var userProvidedContext = args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel();
        
        var queryModel = _metadataManager.Services(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
        if (queryModel == null)
        {
            throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
        }
        var inputFiles = GetInputFiles(queryModel);
        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);
        
        var requestFunction = CreatePromptFunction(kernel);
        var fileChangesResult = requestFunction.InvokeFileChangesPrompt(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext,
            ["targetFileName"] = queryModel.Name + "Handler",
            ["mockFramework"] = GetMockFramework(),
            ["slnRelativePath"] = "/" + string.Join('/', queryModel.GetParentPath().Select(x => x.Name))
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
            You are a senior C# developer specializing in clean architecture with MediatR, FluentValidation, and Entity Framework Core. You're implementing testing logic in a system that strictly follows the repository pattern.

            ## Primary Objective
            Create or update the existing test class file with a set of unit tests that comprehensively test the `Handle` method using xUnit and {{$mockFramework}} in the {{$targetFileName}} class.

            ## Code File Modification Rules
            1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
            2. You may only create or update the test file
            3. Add using clauses for ALL classes that you use in your test (IMPORTANT)
            4. Read and understand the code in all provided Input Code Files. Understand how these code files interact with one another.

            ## Input Code Files:
            {{$inputFilesJson}}

            ## Required Output Format
            Your response MUST include:
            1. Respond ONLY with deserializable JSON that matches the following schema:
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
            
            EXAMPLE RESPONSE BEGIN
            {
                "FileChanges": [
                    {
                        "FilePath": <some-file-path_1>,
                        "Content": <some-file-content_1>
                    },
                    {
                        "FilePath": <some-file-path_2>,
                        "Content": <some-file-content_2>
                    }
                ]
            }
            EXAMPLE RESPONSE END
            
            2. The Content must contain:
            2.1. Your test file as pure code (no markdown).
            2.2. The file must have an appropriate path in the appropriate Tests project. Look for a project in the .sln file that would be appropriate and use the following relative path: '{{$slnRelativePath}}'.

            ## Important Reminders
            - Repositories will assign an Id to entities when `SaveChangesAsync` is called.
            - Collections on entities cannot be treated like arrays.
            - If an existing file exists, you must read this file and update it according to the 'Code Preservation Requirements' below.
            - If you want to construct a DTO, there is a static constructor called 'Create' that you must use.
            - No FluentValidations happen inside of the handlers so don't test for that.
            - Never add your own [IntentManaged] attributes to the test.
            - Don't create test cases with input that would be rejected by the Validator.

            ## Code Preservation Rules (CRITICAL)
            1. **NEVER remove or modify existing class members, methods, or properties, including their attributes or annotations**
            2. **NEVER change existing method signatures or implementations**
            3. **ONLY add new members when necessary (repository methods)**
            4. **DO NOT REMOVE OR ALTER any existing Class Attributes or Method Attributes in the existing code (CRITICAL)**
            5. **NEVER add comments to existing code**
            6. **NEVER remove any existing using clauses (CRITICAL)**
            7. **Ensure that `using Intent.RoslynWeaver.Attributes;` using clause is always present.**

            ## Additional User Context (Optional)
            {{$userProvidedContext}}

            ## Examples
            1. Here's an example of a creation handler:
            ```
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

            ## Previous Error Message
            {{$previousError}}
            """;
        
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate);
        return requestFunction;
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

        inputFiles.AddRange(GetRelatedElements(element).SelectMany(x => filesProvider.GetFilesForMetadata(x)));

        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResult"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResultMappingExtensions"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.PagedListInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
    }

    private static List<ICanBeReferencedType> GetRelatedElements(IElement element)
    {
        var relatedElements = element.AssociatedElements.Where(x => x.TypeReference.Element != null)
            .Select(x => x.TypeReference.Element)
            .ToList();
        if (relatedElements.Count == 0)
        {
            return [];
        }
        var relatedClasses = relatedElements
            .Concat(relatedElements.Where(x => x.TypeReference?.Element?.IsDTOModel() == true).Select(x => x.TypeReference.Element))
            .Concat(relatedElements.Where(Intent.Modelers.Services.Api.OperationModelExtensions.IsOperationModel).Select(x => Intent.Modelers.Services.Api.OperationModelExtensions.AsOperationModel(x).ParentService.InternalElement))
            .Concat(relatedElements.Where(Intent.Modules.Common.Types.Api.OperationModelExtensions.IsOperationModel).Select(x => Intent.Modules.Common.Types.Api.OperationModelExtensions.AsOperationModel(x).InternalElement.ParentElement))
            .Concat(relatedElements.Where(x => x.IsClassModel()).Select(x => x.AsClassModel()).SelectMany(x => x.AssociatedClasses.Select(y => y.TypeReference.Element)))
            .ToList();
        return relatedClasses;
    }

    private string GetMockFramework()
    {
        const string defaultMock = "Moq";

        var unitTestGroup = _applicationConfigurationProvider.GetSettings().GetGroup("d62269ea-8e64-44a0-8392-e1a69da7c960");

        if (unitTestGroup is null)
        {
            return defaultMock;
        }

        return unitTestGroup.GetSetting("115c28bc-a4c8-4b30-bd00-2e320fee77dc")?.Value ?? defaultMock;
    }
}