using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AI.UnitTests.Tasks.Helpers;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Intent.Modules.AI.Prompts.Tasks;

#nullable enable

public class GenerateServiceUnitTestsWithAITask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    public GenerateServiceUnitTestsWithAITask(
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

    public string TaskTypeId => "Intent.Modules.AI.UnitTests.Service.Generate";
    public string TaskTypeName => "Auto-Implement Unit Tests with AI Task (Service)";
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
        Logging.Log.Info($@"Input Files: {Environment.NewLine}{string.Join(Environment.NewLine, inputFiles.Select(x => x))}" );
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
            You are a senior C# developer specializing in clean architecture that uses Entity Framework Core. You're implementing testing logic in a system that strictly follows the repository pattern.

            ## Primary Objective
            Create or update the existing test class file with a set of unit tests that comprehensively test the public methods in the service using xUnit and {{$mockFramework}} in the {{$targetFileName}} class.

            ## Code File Modification Rules
            1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
            2. You may only create or update the test file
            3. Add using clauses for ALL classes that you use in your test (IMPORTANT)
            4. Read and understand the code in all provided Input Code Files. Understand how these code files interact with one another.

            ## Input Code Files:
            {{$inputFilesJson}}

            ## Required Output Format
            Your response MUST include:
            1. Respond ONLY with JSON that matches the following schema:
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
            2. The Content must contain:
            2.1. Your test file as pure code (no markdown).
            2.2. The file must have an appropriate path in the appropriate Tests project. Look for a project in the .sln file that would be appropriate and use the following relative path: '{{$slnRelativePath}}'.

            ## Important things to understand
            - Repositories will assign an Id to entities when `SaveChangesAsync` is called.
            - Collections on entities cannot be treated like arrays.
            - If an existing file exists, you must read this file and update it according to the 'Code Preservation Requirements' below.
            - If you want to construct a DTO, there is a static constructor called 'Create' that you must use.
            - No FluentValidations happen inside of the handlers so don't test for that.
            - Never add your own [IntentManaged] attributes to the test.

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
            1. Here's an example of unit tests implemented for a ProductService:
            ```
            public class ProductsServiceTests
            {
                private readonly Mock<IProductRepository> _productRepositoryMock;
                private readonly Mock<IMapper> _mapperMock;
                private readonly ProductsService _sut;
                private readonly Mock<IUnitOfWork> _unitOfWorkMock;

                public ProductsServiceTests()
                {
                    _productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
                    _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
                    _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
                    _productRepositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
                    _sut = new ProductsService(_productRepositoryMock.Object, _mapperMock.Object);
                }

                [Fact]
                public async Task CreateProduct_ShouldAddAndSave_ThenReturnId()
                {
                    // Arrange
                    var dto = ProductCreateDto.Create("ProductX", "Some Description", 99, "http://example.com/image.jpg");
                    var addedProduct = (Product)null;
                    _productRepositoryMock
                        .Setup(r => r.Add(It.IsAny<Product>()))
                        .Callback<Product>(prod => addedProduct = prod);
                    _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback(() => addedProduct.Id = Guid.NewGuid())
                        .ReturnsAsync(1);

                    // Act
                    var id = await _sut.CreateProduct(dto);

                    // Assert
                    Assert.NotEqual(Guid.Empty, addedProduct.Id); // New product, Id still empty unless set
                    Assert.Equal(dto.Name, addedProduct.Name);
                    Assert.Equal(dto.Description, addedProduct.Description);
                    Assert.Equal(dto.Price, addedProduct.Price);
                    Assert.Equal(dto.ImageUrl, addedProduct.ImageUrl);
                    _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
                    _productRepositoryMock.Verify(x => x.UnitOfWork, Times.AtLeastOnce);
                    _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
                    Assert.Equal(addedProduct.Id, id);
                }

                [Fact]
                public async Task UpdateProduct_WhenProductExists_UpdatesIt()
                {
                    // Arrange
                    var product = new Product {
                        Id = Guid.NewGuid(),
                        Name = "old", Description = "desc", Price = 1M, ImageUrl = "oldUrl"
                    };
                    var dto = ProductUpdateDto.Create(
                        product.Id,
                        "newName",
                        "newDesc",
                        111,
                        "newUrl"
                    );
                    _productRepositoryMock.Setup(r => r.FindByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(product);

                    // Act
                    await _sut.UpdateProduct(product.Id, dto);
                    // Assert
                    Assert.Equal(dto.Name, product.Name);
                    Assert.Equal(dto.Description, product.Description);
                    Assert.Equal(dto.Price, product.Price);
                    Assert.Equal(dto.ImageUrl, product.ImageUrl);
                    _productRepositoryMock.Verify(r => r.FindByIdAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
                }

                [Fact]
                public async Task UpdateProduct_WhenProductNotFound_ThrowsNotFound()
                {
                    // Arrange
                    var id = Guid.NewGuid();
                    var dto = ProductUpdateDto.Create(id, "n", "d", 5, null);
                    _productRepositoryMock.Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Product?)null);
                    // Act/Assert
                    await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateProduct(id, dto));
                }

                [Fact]
                public async Task FindProductById_WhenProductExists_ReturnsDto()
                {
                    // Arrange
                    var id = Guid.NewGuid();
                    var product = new Product { Id = id, Name = "A", Description = "Desc", Price = 2, ImageUrl = "url" };
                    var dto = ProductDto.Create("A", "Desc", 2, "url", id);
                    _productRepositoryMock.Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(product);
                    _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(dto);
                    // Act
                    var result = await _sut.FindProductById(id);
                    // Assert
                    Assert.Equal(dto.Id, result.Id);
                    Assert.Equal(dto.Name, result.Name);
                    Assert.Equal(dto.Description, result.Description);
                    Assert.Equal(dto.Price, result.Price);
                    Assert.Equal(dto.ImageUrl, result.ImageUrl);
                    _productRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
                    _mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
                }

                [Fact]
                public async Task FindProductById_WhenNotFound_ThrowsNotFound()
                {
                    // Arrange
                    var id = Guid.NewGuid();
                    _productRepositoryMock.Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Product?)null);
                    // Act/Assert
                    await Assert.ThrowsAsync<NotFoundException>(() => _sut.FindProductById(id));
                }

                [Fact]
                public async Task FindProducts_ReturnsPagedDtoResult()
                {
                    // Arrange
                    var list = new List<Product>
                    {
                        new Product { Id = Guid.NewGuid(), Name = "A", Description = "a", Price = 100, ImageUrl = "img1" },
                        new Product { Id = Guid.NewGuid(), Name = "B", Description = "b", Price = 200, ImageUrl = "img2" },
                    };
                    var paged = new PagedList<Product>(2, 1, 2, list);
                    _productRepositoryMock.Setup(repo => repo.FindAllAsync(1, 2, It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(paged);
                    var dtos = list.Select(x => ProductDto.Create(x.Name, x.Description, x.Price, x.ImageUrl ?? string.Empty, x.Id)).ToList();
                    _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns<Product>(p =>
                        dtos.First(d => d.Id == p.Id));
                    // Act
                    var result = await _sut.FindProducts(1, 2, "Name");
                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(paged.Count, result.Data.Count());
                    Assert.Equal(result.Data.Count(), dtos.Count);
                    foreach(var dto in result.Data) {
                        Assert.Contains(dtos, x => x.Id == dto.Id);
                    }
                    _productRepositoryMock.Verify(x => x.FindAllAsync(1, 2, It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>(), It.IsAny<CancellationToken>()), Times.Once);
                }

                [Fact]
                public async Task DeleteProduct_WhenFound_Deletes()
                {
                    // Arrange
                    var id = Guid.NewGuid();
                    var product = new Product { Id = id, Name = "P", Description = "desc", Price = 1, ImageUrl = "i" };
                    _productRepositoryMock.Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
                    _productRepositoryMock.Setup(r => r.Remove(product));
                    // Act
                    await _sut.DeleteProduct(id);
                    // Assert
                    _productRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
                    _productRepositoryMock.Verify(r => r.Remove(product), Times.Once);
                }

                [Fact]
                public async Task DeleteProduct_WhenNotFound_ThrowsNotFound()
                {
                    // Arrange
                    var id = Guid.NewGuid();
                    _productRepositoryMock.Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
                    // Act/Assert
                    await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteProduct(id));
                }
            }
            ```

            ## Previous Error Message
            {{$previousError}}
            """;
        
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate);
        return requestFunction;
    }

    private List<ICodebaseFile> GetInputFiles(IElement service)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        var inputFiles = filesProvider.GetFilesForMetadata(service).ToList();
        foreach (var operation in service.ChildElements)
        {
            
            if (operation.TypeReference.ElementId != null)
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(operation.TypeReference.Element));
            }
            foreach (var paramType in operation.ChildElements.Where(x => x.TypeReference.Element.IsDTOModel()).Select(x => x.TypeReference.Element))
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(paramType));
            }

            inputFiles.AddRange(GetRelatedDomainEntities(operation).SelectMany(x => filesProvider.GetFilesForMetadata(x)));
        }

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

    private static List<ICanBeReferencedType> GetRelatedDomainEntities(IElement element)
    {
        var queriedEntity = element.AssociatedElements.Where(x => x.TypeReference.Element != null)
            .Select(x => x.TypeReference.Element.AsClassModel())
            .ToList();
        if (queriedEntity.Count == 0)
        {
            return [];
        }
        var relatedClasses = queriedEntity.Select(x => x.InternalElement)
            .Concat(queriedEntity.SelectMany(x => x.AssociatedClasses.Select(y => y.TypeReference.Element)))
            .ToList();
        return relatedClasses;
    }

    private string GetMockFramework()
    {
        var defaultMock = "Moq";

        var unitTestGroup = _applicationConfigurationProvider.GetSettings().GetGroup("d62269ea-8e64-44a0-8392-e1a69da7c960");

        if (unitTestGroup is null)
        {
            return defaultMock;
        }

        return unitTestGroup.GetSetting("115c28bc-a4c8-4b30-bd00-2e320fee77dc")?.Value ?? defaultMock;
    }
}