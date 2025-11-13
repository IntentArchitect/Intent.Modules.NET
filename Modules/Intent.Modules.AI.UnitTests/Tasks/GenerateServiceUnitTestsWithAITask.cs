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
using Intent.Modules.Common.AI.Settings;
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
        var userProvidedContext = !string.IsNullOrWhiteSpace(args[2]) ? args[2] : "None";
        var provider = new AISettings.ProviderOptions(args[3]).AsEnum();
        var modelId = args[4];
        var thinkingType = args[5];

        Logging.Log.Info($"Args: {string.Join(",", args)}");
        var kernel = _intentSemanticKernelFactory.BuildSemanticKernel(modelId, provider, null);
        
        var queryModel = _metadataManager.Services(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
        if (queryModel == null)
        {
            throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
        }
        var inputFiles = GetInputFiles(queryModel);
        Logging.Log.Info($"Context files included: {inputFiles.Count} files");
        Logging.Log.Debug($"Files: {string.Join(", ", inputFiles.Select(f => Path.GetFileName(f.FilePath)))}");
        
        var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

        var requestFunction = CreatePromptFunction(kernel, thinkingType);
        var fileChangesResult = requestFunction.InvokeFileChangesPrompt(kernel, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext,
            ["targetFileName"] = queryModel.Name,
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


    private static KernelFunction CreatePromptFunction(Kernel kernel, string thinkingType)
    {
        const string promptTemplate =
            """
            ## Role and Context
            You are a senior C# developer specializing in clean architecture that uses Entity Framework Core. You're implementing unit tests that mock all external dependencies using {{$mockFramework}}.

            ## Primary Objective
            Create or update the existing test class file with a set of unit tests that comprehensively test the public methods in the service using xUnit and {{$mockFramework}} in the {{$targetFileName}} class.

            ## Code File Modification Rules
            1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
            2. You may only create or update the test file
            3. Add using clauses for ALL classes that you use in your test (IMPORTANT)
            4. Focus on the service implementation - all infrastructure types (repositories, mappers, etc.) should be mocked.

            ## Test Coverage Requirements
            Generate tests that cover these scenarios (where applicable to the service method):
            1. **Happy Path**: Method executes successfully with valid input
            2. **Entity Not Found**: Test NotFoundException when FindByIdAsync returns null (for Update/Delete/GetById operations)
            3. **Empty Results**: Test behavior when queries return empty collections
            4. **Edge Cases** (CRITICAL):
               * For filtered queries with predicates: Test when data exists but doesn't match ALL filter criteria (e.g., right price range but wrong category)
               * For domain-specific repository methods: Add comment explaining the method filters internally, mock returns empty when criteria not met
               * For date range queries: Test boundary dates (startDate = entityDate, endDate = entityDate)
               * For single entity operations: Test not found scenario
               * Avoid trivial variations: Don't test 1 item, 2 items, 4 items separately - one happy path with multiple items is sufficient
            5. **State Changes**: Assert that entity properties are updated correctly (for Update operations)
            6. **Return Values**: Verify correct return values (Guid for Create, DTO for Query, void for Delete)
            
            ## Tests to AVOID (Anti-Patterns)
            - **Don't create separate tests ONLY for parameter verification** - Parameters are already verified in functional tests
            - **Don't test cancellationToken propagation** - Unless service has specific cancellation logic, this is implicitly tested
            - **Don't create multiple tests for trivial variations** - Combine related scenarios or use Theory/InlineData if needed

            ## Input Code Files (Organized by Priority):
            The files below include various types of code files. Understand their purpose:
            
            **PRIMARY FILES** (Code under test):
            - Service classes - the implementation you're testing
            - DTOs (Data Transfer Objects) - input/output types used by the service
            
            **DOMAIN FILES** (For understanding, NOT for implementation):
            - Entity classes (e.g., Product, Category) - domain models referenced by the service
            - Entity repository interfaces (e.g., IProductRepository) - contain method signatures including domain-specific query methods
            
            **INFRASTRUCTURE FILES** (For mocking):
            - Generic interfaces (IEFRepository, IMapper, IUnitOfWork) - mock these in tests
            - Utility types (PagedResult, NotFoundException) - reference for test assertions
            
            **SOLUTION FILE**:
            - .sln file - use to determine the correct test project path
            
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

            ## Important Reminders for Unit Testing
            - **You are writing UNIT tests**: Mock ALL external dependencies (repositories, DbContext, external services, IMapper).
            - **Infrastructure files are for reference only**: Use them to understand interface signatures for mocking, NOT for implementation.
            - **Repository behavior**: Repositories assign an Id to entities when `SaveChangesAsync` is called - use Callback to simulate this.
            - **Entity collections**: Collections on entities cannot be treated like arrays in tests.
            - **DTO construction**: DTOs MAY have a static `Create` factory method (if configured in Intent settings) OR use property initialization. Check the DTO file in the input context to determine which pattern to use:
              * If DTO has `public static TDto Create(...)` method: Use `ProductDto.Create(param1, param2, ...)`
              * If DTO has public properties with setters: Use `new ProductDto { Property1 = value1, Property2 = value2 }`
            - **AutoMapper mocking (CRITICAL)**: 
              * Mock the SINGULAR `Map<TDto>(entity)` method, NOT `Map<List<TDto>>(list)`
              * Extension methods like `.MapToProductDtoList()` call `Map<TDto>` for each item individually
              * Setup: `_mapperMock.Setup(x => x.Map<ProductDto>(It.IsAny<Product>())).Returns((Product p) => ...)`
            - **Filtered query testing (CRITICAL - TWO SCENARIOS)**:
              * **Scenario 1 - Generic FindAllAsync with predicate**: Service calls `repository.FindAllAsync(x => x.Status == "Active", ...)` 
                - Setup repository to ACTUALLY APPLY the predicate by compiling it
                - Pattern: `predicate.Compile()` then `testData.Where(compiled)`
                - This tests that the service's filter logic is correct
                - See Example 6A for the complete pattern
              * **Scenario 2 - Domain-specific repository method**: Service calls `repository.FindActiveProductsAsync(...)`
                - Mock the domain-specific method directly with its parameters
                - Don't use predicate compilation
                - Add comment explaining the method filters internally
                - See Example 6B for the complete pattern
              * **How to choose**: Inspect the service code to see which repository method it calls
            - **NotFoundException pattern**: Always test both success path AND NotFoundException for FindByIdAsync operations.
            - **Validation**: No FluentValidations happen inside services - don't test for validation errors.
            - **Test naming**: Use concise pattern `{MethodName}_{ExpectedBehavior}_When{Condition}` (e.g., `FindProductById_ReturnsDto_WhenFound`, `FindProductById_ThrowsNotFoundException_WhenNotFound`). Omit "When" clause if obvious from context. Follow user-specified naming convention if provided in their context.
            - **AAA pattern**: Clearly separate Arrange, Act, Assert sections with comments.
            
            ## Test Data Quality Rules (CRITICAL)
            - **Test Data Minimalism (HIGHEST PRIORITY)**:
              * BAD: Setting ALL properties on test entities when only 1-2 are relevant to the test
              * GOOD: Only set properties that are directly used in assertions or affect test behavior
              * Example BAD: `new Product { Id = guid, Name = "Product A", Description = "A description", Price = 100m, ImageUrl = "img.jpg", CategoryId = guid2, Stock = 50 }` when testing price filter
              * Example GOOD: `new Product { Price = 100m }` when testing price filter
              * Exception: If testing entity creation (Add), set all required properties to ensure valid entity construction
            
            - **Avoid Meaningless Placeholders**:
              * Don't use placeholder strings like "Product Name", "Description Here", "Item 1", "Test Value"
              * Use minimal identifiers or empty strings when property value doesn't matter: "", Guid.Empty, 0
              * Only use meaningful values when the actual value is relevant to the test
            
            - **Avoid Redundant Tests**:
              * Don't create separate tests ONLY to verify mock interactions if they're already verified in the happy path
              * BAD: Test 1 verifies repository.FindAllAsync() + mapper.Map() + result correctness, Test 2 only verifies repository call, Test 3 only verifies mapper call
              * GOOD: Test 1 verifies repository + mapper + result correctness (all in one), Test 2 tests empty results, Test 3 tests edge cases
              * Create separate tests for: NotFoundException, empty results, boundary conditions, different logical scenarios
              * Don't create separate tests for: re-verifying the same mock calls, testing the same code path with trivial variations
              * Avoid multiple tests that only differ in data quantity (e.g., 2 items vs 4 items) - one happy path is sufficient
            
            - **Mapper Mock Simplicity**:
              * BAD: `.Returns((Product p) => expectedDtos.First(dto => dto.Id == p.Id))` - Fragile matching logic
              * GOOD: `.Returns((Product p) => ProductDto.Create(p.Name, p.Price, ...))` - Direct creation
              * Use simple, inline DTO creation in mapper mocks rather than pre-creating DTOs and matching them
            
            - **Code preservation**: If an existing test file exists, update it according to 'Code Preservation Requirements' below.
            - **Attributes**: Never add your own [IntentManaged] attributes to tests.
            - **Test data**: Don't create test cases with input that would be rejected by the Validator.

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

            ## Test Class Structure
            Your test class should follow this structure:
            ```csharp
            public class {ServiceName}Tests
            {
                private readonly Mock<IRepository> _repositoryMock;
                private readonly Mock<IMapper> _mapperMock; // Only if service uses IMapper
                private readonly {ServiceName} _sut;

                public {ServiceName}Tests()
                {
                    _repositoryMock = new Mock<IRepository>();
                    _mapperMock = new Mock<IMapper>(); // Only if needed
                    _sut = new {ServiceName}(_repositoryMock.Object, _mapperMock.Object);
                }

                // Test methods follow...
            }
            ```

            ## Test Examples (Follow These Patterns)
            
            **Note on DTO Construction in Examples:** 
            The examples below use `TDto.Create(...)` factory method pattern. If the DTO in your context uses property initialization instead, adapt the pattern to `new TDto { Property1 = value1, Property2 = value2 }`. Check the DTO file provided in the input context to determine which pattern to use.
            
            ### Example 1: CREATE Method
            ```csharp
            [Fact]
            public async Task CreateProduct_AddsProductAndReturnsId()
            {
                // Arrange
                var dto = ProductCreateDto.Create("ProductX", "", 99m, "");
                Product capturedProduct = null;
                var expectedId = Guid.NewGuid();

                _productRepositoryMock
                    .Setup(x => x.Add(It.IsAny<Product>()))
                    .Callback<Product>(p => capturedProduct = p);
                
                _productRepositoryMock
                    .Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => capturedProduct.Id = expectedId)
                    .ReturnsAsync(1);

                // Act
                var result = await _sut.CreateProduct(dto);

                // Assert
                Assert.Equal(expectedId, result);
                Assert.NotNull(capturedProduct);
                Assert.Equal("ProductX", capturedProduct.Name);
                Assert.Equal(99m, capturedProduct.Price);
                _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
                _productRepositoryMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
            ```

            ### Example 2: UPDATE Method (with NotFoundException)
            ```csharp
            [Fact]
            public async Task UpdateProduct_UpdatesProduct_WhenFound()
            {
                // Arrange
                var productId = Guid.NewGuid();
                var dto = ProductUpdateDto.Create(productId, "NewName", "", 150m, "");
                // Minimal test data - only set ID, we're testing updates
                var existingProduct = new Product { Id = productId };

                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(existingProduct);

                // Act
                await _sut.UpdateProduct(productId, dto);

                // Assert
                Assert.Equal("NewName", existingProduct.Name);
                Assert.Equal(150m, existingProduct.Price);
                _productRepositoryMock.Verify(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task UpdateProduct_ThrowsNotFoundException_WhenProductNotFound()
            {
                // Arrange
                var dto = ProductUpdateDto.Create(Guid.NewGuid(), "", "", 0, "");
                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Product)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateProduct(dto.Id, dto));
            }
            ```

            ### Example 3: DELETE Method
            ```csharp
            [Fact]
            public async Task DeleteProduct_RemovesProduct_WhenFound()
            {
                // Arrange
                var productId = Guid.NewGuid();
                var product = new Product { Id = productId };

                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(product);

                // Act
                await _sut.DeleteProduct(productId);

                // Assert
                _productRepositoryMock.Verify(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
                _productRepositoryMock.Verify(x => x.Remove(product), Times.Once);
            }

            [Fact]
            public async Task DeleteProduct_ThrowsNotFoundException_WhenNotFound()
            {
                // Arrange
                var productId = Guid.NewGuid();
                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Product)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteProduct(productId));
            }
            ```

            ### Example 4: GET BY ID with AutoMapper
            ```csharp
            [Fact]
            public async Task FindProductById_ReturnsDto_WhenFound()
            {
                // Arrange
                var productId = Guid.NewGuid();
                // Only set properties we'll verify
                var product = new Product { Id = productId, Name = "Product A", Price = 100m };
                var expectedDto = ProductDto.Create("Product A", "", 100m, "", productId);

                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(product);
                
                // IMPORTANT: Mock singular Map<TDto>, not Map<List<TDto>>
                _mapperMock
                    .Setup(x => x.Map<ProductDto>(product))
                    .Returns(expectedDto);

                // Act
                var result = await _sut.FindProductById(productId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(productId, result.Id);
                Assert.Equal("Product A", result.Name);
                Assert.Equal(100m, result.Price);
                _productRepositoryMock.Verify(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
                _mapperMock.Verify(x => x.Map<ProductDto>(product), Times.Once);
            }

            [Fact]
            public async Task FindProductById_ThrowsNotFoundException_WhenNotFound()
            {
                // Arrange
                var productId = Guid.NewGuid();
                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Product)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _sut.FindProductById(productId));
            }
            ```

            ### Example 5: GET ALL (Simple List)
            ```csharp
            [Fact]
            public async Task FindProducts_ReturnsAllProducts()
            {
                // Arrange
                // Minimal test data - only need IDs to verify count
                var products = new List<Product>
                {
                    new Product { Id = Guid.NewGuid() },
                    new Product { Id = Guid.NewGuid() }
                };

                _productRepositoryMock
                    .Setup(x => x.FindAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(products);
                
                // IMPORTANT: Mock singular Map<TDto>, NOT Map<List<TDto>>
                // The extension method .MapToProductDtoList() calls Map<TDto> for each item
                _mapperMock
                    .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
                    .Returns((Product p) => ProductDto.Create("", "", 0, "", p.Id));

                // Act
                var result = await _sut.FindProducts();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                _productRepositoryMock.Verify(x => x.FindAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
            ```

            ### Example 6A: FILTERED Query - Scenario 1 (Generic FindAllAsync with Predicate)
            **Use this pattern when the service builds a predicate and passes it to a generic FindAllAsync method**
            ```csharp
            // Service code example:
            // var products = await _repository.FindAllAsync(x => x.CategoryId == categoryId, cancellationToken);
            
            [Fact]
            public async Task FindProductsByCategory_ReturnsOnlyMatchingProducts()
            {
                // Arrange
                var categoryId = Guid.NewGuid();
                
                // Create test data covering various scenarios
                // ONLY set CategoryId property - it's the only one used in the filter
                var allProducts = new List<Product>
                {
                    new Product { CategoryId = categoryId },                // ✓ Matches
                    new Product { CategoryId = categoryId },                // ✓ Matches
                    new Product { CategoryId = Guid.NewGuid() }             // ✗ Different category
                };

                // CRITICAL: Setup repository to ACTUALLY APPLY the predicate
                _productRepositoryMock
                    .Setup(x => x.FindAllAsync(
                        It.IsAny<Expression<Func<Product, bool>>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Expression<Func<Product, bool>> predicate, CancellationToken ct) =>
                    {
                        var compiled = predicate.Compile();
                        return allProducts.Where(compiled).ToList();
                    });

                _mapperMock
                    .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
                    .Returns((Product p) => ProductDto.Create("", "", 0, "", Guid.NewGuid()));

                // Act
                var result = await _sut.FindProductsByCategory(categoryId);

                // Assert - This single test verifies: predicate logic, repository call, mapper call, result correctness
                Assert.NotNull(result);
                Assert.Equal(2, result.Count); // Only 2 match the category
            }

            [Fact]
            public async Task FindProductsByCategory_ReturnsEmpty_WhenNoProductsMatchFilter()
            {
                // Arrange
                var categoryId = Guid.NewGuid();
                // CRITICAL EDGE CASE: Products exist but don't match filter criteria
                // This validates the service's predicate logic correctly filters out non-matching data
                var allProducts = new List<Product>
                {
                    new Product { CategoryId = Guid.NewGuid() }, // Different category
                    new Product { CategoryId = Guid.NewGuid() }  // Different category
                };

                _productRepositoryMock
                    .Setup(x => x.FindAllAsync(
                        It.IsAny<Expression<Func<Product, bool>>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Expression<Func<Product, bool>> predicate, CancellationToken ct) =>
                    {
                        var compiled = predicate.Compile();
                        return allProducts.Where(compiled).ToList();
                    });

                // Act
                var result = await _sut.FindProductsByCategory(categoryId);

                // Assert
                Assert.Empty(result);
            }
            ```

            ### Example 6B: FILTERED Query - Scenario 2 (Domain-Specific Repository Method)
            **Use this pattern when the service calls a domain-specific repository method that encapsulates the filtering logic**
            ```csharp
            // Service code example:
            // var products = await _repository.FindActiveProductsInCategoryAsync(categoryId, cancellationToken);
            
            [Fact]
            public async Task FindActiveProducts_ReturnsProducts_WhenProductsMatchCriteria()
            {
                // Arrange
                var categoryId = Guid.NewGuid();
                
                // Only need matching data since repository method handles filtering internally
                var products = new List<Product>
                {
                    new Product { CategoryId = categoryId, Status = "Active" },
                    new Product { CategoryId = categoryId, Status = "Active" }
                };

                // Mock the domain-specific method directly - no predicate compilation needed
                _productRepositoryMock
                    .Setup(x => x.FindActiveProductsInCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(products);

                _mapperMock
                    .Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
                    .Returns((Product p) => ProductDto.Create("", "", 0, "", p.Id));

                // Act
                var result = await _sut.FindActiveProducts(categoryId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.All(result, dto => Assert.NotEqual(Guid.Empty, dto.Id));
                _productRepositoryMock.Verify(
                    x => x.FindActiveProductsInCategoryAsync(categoryId, It.IsAny<CancellationToken>()), 
                    Times.Once);
            }

            [Fact]
            public async Task FindActiveProducts_ReturnsEmpty_WhenNoProductsMatchCriteria()
            {
                // Arrange
                var categoryId = Guid.NewGuid();
                
                // EDGE CASE: Repository method filters internally, returns empty when criteria not met
                // This could mean: no products exist, OR products exist but don't match filters
                _productRepositoryMock
                    .Setup(x => x.FindActiveProductsInCategoryAsync(categoryId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Product>());

                // Act
                var result = await _sut.FindActiveProducts(categoryId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
            ```



            ## Previous Error Message
            {{$previousError}}
            """;
        
        var requestFunction = kernel.CreateFunctionFromPrompt(promptTemplate, kernel.GetRequiredService<IAiProviderService>().GetPromptExecutionSettings(thinkingType));
        return requestFunction;
    }

    private List<ICodebaseFile> GetInputFiles(IElement service)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();
        
        // PRIMARY: Service implementation and its DTOs (the code we're testing)
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

            inputFiles.AddRange(GetRelatedElements(operation).SelectMany(x => filesProvider.GetFilesForMetadata(x)));
        }

        // INFRASTRUCTURE: Only include interfaces we'll mock (not implementations)
        // For unit tests, we only need to know method signatures to create mocks
        var serviceFiles = filesProvider.GetFilesForMetadata(service);
        var serviceContent = string.Join("\n", serviceFiles.Select(f => f.Content));
        
        // Only include repository interface if service uses it
        if (serviceContent.Contains("Repository", StringComparison.OrdinalIgnoreCase))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        }
        
        // Only include pagination if service returns paged results
        if (serviceContent.Contains("PagedResult") || serviceContent.Contains("PagedList"))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResult"));
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.PagedListInterface"));
        }
        
        // Only include NotFoundException if service throws it
        if (serviceContent.Contains("NotFoundException"))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        }
        
        // Only include UnitOfWork if service saves changes
        if (serviceContent.Contains("SaveChangesAsync") || serviceContent.Contains("IUnitOfWork"))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));
        }

        // Solution file needed for test project path discovery
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
        var defaultMock = "Moq";

        var unitTestGroup = _applicationConfigurationProvider.GetSettings().GetGroup("d62269ea-8e64-44a0-8392-e1a69da7c960");

        if (unitTestGroup is null)
        {
            return defaultMock;
        }

        return unitTestGroup.GetSetting("115c28bc-a4c8-4b30-bd00-2e320fee77dc")?.Value ?? defaultMock;
    }
}