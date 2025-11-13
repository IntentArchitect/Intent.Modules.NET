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
using Intent.Modules.Common.AI.CodeGeneration;
using Intent.Modules.Common.AI.Extensions;
using Intent.Modules.Common.AI.Settings;
using Intent.Plugins;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.AI.UnitTests.Tasks;

#nullable enable

[IntentManaged(Mode.Merge)]
#nullable enable

public class GenerateCqrsHandlerUnitTestsWithAITask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    [IntentManaged(Mode.Merge)]
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

    public string TaskTypeId => "Intent.AI.UnitTests.GenerateCqrsHandlerUnitTestsWithAITask";
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public string TaskTypeName => "Auto-Implement Unit Tests with AI Task (CQRS Handler)";
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public int Order => 0;

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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

        var promptTemplate = GetPromptTemplate();
        var fileChangesResult = kernel.InvokeFileChangesPrompt(promptTemplate, thinkingType, new KernelArguments()
        {
            ["inputFilesJson"] = jsonInput,
            ["userProvidedContext"] = userProvidedContext,
            ["targetFileName"] = queryModel.Name + "Handler",
            ["mockFramework"] = GetMockFramework(),
            ["slnRelativePath"] = "/" + string.Join('/', queryModel.GetParentPath().Select(x => x.Name)),
            ["fileChangesSchema"] = FileChangesSchema.GetPromptInstructions()
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

    private static string GetPromptTemplate()
    {
        const string promptTemplate =
            """
            ## Role and Context
            You are a senior C# developer specializing in clean architecture with MediatR, FluentValidation, and Entity Framework Core. You're implementing unit tests that mock all external dependencies using {{$mockFramework}}.

            ## Primary Objective
            Create or update the existing test class file with a set of unit tests that comprehensively test the `Handle` method using xUnit and {{$mockFramework}} in the {{$targetFileName}} class.

            ## Test Coverage Requirements
            Generate tests that cover these scenarios (where applicable to the handler):
            1. **Happy Path**: Handler executes successfully with valid input
            2. **Entity Not Found**: Test NotFoundException when FindByIdAsync returns null (for Update/Delete/GetById operations)
            3. **Empty Results**: Test behavior when queries return empty collections
            4. **Edge Cases** (CRITICAL):
               * For filtered queries with predicates: Test when data exists but doesn't match ALL filter criteria (e.g., right date range but wrong status)
               * For domain-specific repository methods: Add comment explaining the method filters internally, mock returns empty when criteria not met
               * For date range queries: Test boundary dates (startDate = entityDate, endDate = entityDate)
               * For single entity operations: Test not found scenario
               * Avoid trivial variations: Don't test 1 item, 2 items, 4 items separately - one happy path with multiple items is sufficient
            5. **State Changes**: Assert that entity properties are updated correctly (for Update operations)
            6. **Return Values**: Verify correct return values (Guid for Create, DTO for Query, void for Delete)
            
            ## Tests to AVOID (Anti-Patterns)
            - **Don't create separate tests ONLY for parameter verification** - Parameters are already verified in functional tests
            - **Don't test cancellationToken propagation** - Unless handler has specific cancellation logic, this is implicitly tested
            - **Don't create multiple tests for trivial variations** - Combine related scenarios or use Theory/InlineData if needed

            ## Code File Modification Rules
            1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
            2. You may only create or update the test file
            3. Add using clauses for ALL classes that you use in your test (CRITICAL):
               * Include the entity namespace (e.g., `using CleanArch1.Domain.Entities;`) when using domain entities in test data
               * Include repository namespaces when mocking repositories
               * Include DTO namespaces for DTOs used in assertions
            4. Focus on the handler implementation - all infrastructure types (repositories, mappers, etc.) should be mocked.

            ## Input Code Files (Organized by Priority):
            The files below include various types of code files. Understand their purpose:
            
            **PRIMARY FILES** (Code under test):
            - Handler/Command/Query classes - the implementation you're testing
            - DTOs (Data Transfer Objects) - input/output types used by the handler
            
            **DOMAIN FILES** (CRITICAL - Use for test data):
            - Entity classes (e.g., Transaction, Client) - **USE THESE TYPES** when creating test data (e.g., `new Transaction { ... }`)
            - Entity repository interfaces (e.g., `ITransactionRepository : IEFRepository<Transaction, Transaction>`) - identify the entity type from the interface signature
            - **NEVER use anonymous objects** for entities - always use the actual entity class
            
            **INFRASTRUCTURE FILES** (For mocking):
            - Generic interfaces (IEFRepository, IMapper, IUnitOfWork) - mock these in tests
            - Utility types (PagedResult, NotFoundException) - reference for test assertions
            
            **SOLUTION FILE**:
            - .sln file - use to determine the correct test project path
            
            {{$inputFilesJson}}

            {{$fileChangesSchema}}
            
            2. The Content must contain:
            2.1. Your test file as pure code (no markdown).
            2.2. The file must have an appropriate path in the appropriate Tests project. Look for a project in the .sln file that would be appropriate and use the following relative path: '{{$slnRelativePath}}'.
            
            ## Important Reminders for Unit Testing
            - **You are writing UNIT tests**: Mock ALL external dependencies (repositories, DbContext, external services, IMapper).
            - **Infrastructure files are for reference only**: Use them to understand interface signatures for mocking, NOT for implementation.
            - **Repository behavior**: Repositories assign an Id to entities when `SaveChangesAsync` is called - use Callback to simulate this.
            - **Entity collections**: Collections on entities cannot be treated like arrays in tests.
            - **DTO construction**: DTOs MAY have a static `Create` factory method (if configured in Intent settings) OR use property initialization. Check the DTO file in the input context to determine which pattern to use:
              * If DTO has `public static TDto Create(...)` method: Use `TransactionDto.Create(param1, param2, ...)`
              * If DTO has public properties with setters: Use `new TransactionDto { Property1 = value1, Property2 = value2 }`
            - **AutoMapper mocking (CRITICAL)**: 
              * Mock the SINGULAR `Map<TDto>(entity)` method, NOT `Map<List<TDto>>(list)`
              * Extension methods like `.MapToTransactionDtoList()` call `Map<TDto>` for each item individually
              * Setup: `_mapperMock.Setup(x => x.Map<TransactionDto>(It.IsAny<Transaction>())).Returns((Transaction t) => ...)`
            - **Entity types in repository mocks (CRITICAL)**:
              * **ALWAYS use the actual domain entity type** (e.g., `Transaction`, `Product`) when creating test data and mocking repository methods
              * **NEVER use anonymous objects** (e.g., `new { Id = ..., Status = ... }`) for test entity data
              * **Find the entity type**: Look at the repository interface (e.g., `ITransactionRepository : IEFRepository<Transaction, Transaction>`) to identify the entity type
              * **Include entity namespace**: Always add `using` statement for the entity namespace (e.g., `using CleanArch1.Domain.Entities;`)
              * **Correct generic types in mocks**: When mocking `FindAllProjectToAsync<TDto>`, use `Expression<Func<EntityType, bool>>`, NOT `Expression<Func<object, bool>>`
              * Example BAD: `new { Status = "Active" }` with `It.IsAny<Expression<Func<object, bool>>>()`
              * Example GOOD: `new Transaction { Status = "Active" }` with `It.IsAny<Expression<Func<Transaction, bool>>>()`
            - **Filtered query testing (CRITICAL - TWO SCENARIOS)**:
              * **Scenario 1 - Generic FindAllAsync with predicate**: Handler calls `repository.FindAllAsync(x => x.Status == "Completed", ...)` 
                - Setup repository to ACTUALLY APPLY the predicate by compiling it
                - Pattern: `predicate.Compile()` then `testData.Where(compiled)`
                - This tests that the handler's filter logic is correct
                - See Example 6A for the complete pattern
              * **Scenario 2 - Domain-specific repository method**: Handler calls `repository.FindCompletedTransactionsAsync(fromDate, toDate, ...)`
                - Mock the domain-specific method directly with its parameters
                - Don't use predicate compilation
                - Add comment explaining the method filters internally
                - See Example 6B for the complete pattern
              * **How to choose**: Inspect the handler code to see which repository method it calls
            - **NotFoundException pattern**: Always test both success path AND NotFoundException for FindByIdAsync operations.
            - **Validation**: No FluentValidations happen inside handlers - don't test for validation errors.
            - **Test naming**: Use concise pattern `{MethodName}_{ExpectedBehavior}_When{Condition}` (e.g., `Handle_ReturnsTransaction_WhenFound`, `Handle_ThrowsNotFoundException_WhenNotFound`). Omit "When" clause if obvious from context. Follow user-specified naming convention if provided in their context.
            - **AAA pattern**: Clearly separate Arrange, Act, Assert sections with comments.
            
            ## Test Data Quality Rules (CRITICAL)
            - **Test Data Minimalism (HIGHEST PRIORITY)**:
              * BAD: Setting ALL properties on test entities when only 1-2 are relevant to the test
              * GOOD: Only set properties that are directly used in assertions or affect test behavior
              * Example BAD: `new Transaction { Id = guid, TransactionDate = date, SalePrice = 100m, Status = "Pending", CommissionRate = 0.03m, ClosingAgent = "Agent Name", ClientId = guid1, BuyerId = guid2, SellerId = guid3, AssetId = guid4 }` when testing Status filter
              * Example GOOD: `new Transaction { Status = "Completed", TransactionDate = date }` when testing Status filter
              * Exception: If testing entity creation (Add), set all required properties to ensure valid entity construction
            
            - **Avoid Meaningless Placeholders**:
              * Don't use placeholder strings like "Asset Name", "Buyer Name", "Transaction 1", "Description Here"
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
              * BAD: `.Returns((Transaction t) => expectedDtos.First(dto => dto.Id == t.Id))` - Fragile matching logic
              * GOOD: `.Returns((Transaction t) => TransactionDto.Create(t.Property1, t.Property2, ...))` - Direct creation
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
            public class {HandlerName}Tests
            {
                private readonly Mock<IRepository> _repositoryMock;
                private readonly Mock<IMapper> _mapperMock; // Only if handler uses IMapper
                private readonly {HandlerName} _handler;

                public {HandlerName}Tests()
                {
                    _repositoryMock = new Mock<IRepository>();
                    _mapperMock = new Mock<IMapper>(); // Only if needed
                    _handler = new {HandlerName}(_repositoryMock.Object, _mapperMock.Object);
                }

                // Test methods follow...
            }
            ```

            ## Test Examples (Follow These Patterns)
            
            **Note on DTO Construction in Examples:** 
            The examples below use `TDto.Create(...)` factory method pattern. If the DTO in your context uses property initialization instead, adapt the pattern to `new TDto { Property1 = value1, Property2 = value2 }`. Check the DTO file provided in the input context to determine which pattern to use.
            
            ### Example 1: CREATE Command Handler
            ```csharp
            [Fact]
            public async Task Handle_CreatesTransactionAndReturnsId()
            {
                // Arrange
                var transactionDate = DateTime.UtcNow;
                var command = new CreateTransactionCommand(
                    transactionDate,
                    250000m,
                    "Pending",
                    0.03m,
                    "",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                );
                Transaction capturedTransaction = null;
                var expectedId = Guid.NewGuid();

                _transactionRepositoryMock
                    .Setup(x => x.Add(It.IsAny<Transaction>()))
                    .Callback<Transaction>(t => capturedTransaction = t);
                
                _transactionRepositoryMock
                    .Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => capturedTransaction.Id = expectedId)
                    .ReturnsAsync(1);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.Equal(expectedId, result);
                Assert.NotNull(capturedTransaction);
                Assert.Equal(transactionDate, capturedTransaction.TransactionDate);
                Assert.Equal(250000m, capturedTransaction.SalePrice);
                Assert.Equal("Pending", capturedTransaction.Status);
                _transactionRepositoryMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
                _transactionRepositoryMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
            ```

            ### Example 2: UPDATE Command Handler (with NotFoundException)
            ```csharp
            [Fact]
            public async Task Handle_UpdatesTransaction_WhenFound()
            {
                // Arrange
                var transactionId = Guid.NewGuid();
                var newDate = DateTime.UtcNow;
                var command = new UpdateTransactionCommand(
                    transactionId,
                    newDate,
                    300000m,
                    "Completed",
                    0.025m,
                    "",
                    Guid.Empty,
                    Guid.Empty,
                    Guid.Empty,
                    Guid.Empty
                );
                // Only set properties relevant to the test - we're verifying updates
                var existingTransaction = new Transaction { Id = transactionId };

                _transactionRepositoryMock
                    .Setup(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(existingTransaction);

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.Equal(newDate, existingTransaction.TransactionDate);
                Assert.Equal(300000m, existingTransaction.SalePrice);
                Assert.Equal("Completed", existingTransaction.Status);
                Assert.Equal(0.025m, existingTransaction.CommissionRate);
                _transactionRepositoryMock.Verify(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task Handle_ThrowsNotFoundException_WhenTransactionNotFound()
            {
                // Arrange
                var command = new UpdateTransactionCommand(Guid.NewGuid(), DateTime.UtcNow, 0, "", 0, "", Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty);
                _transactionRepositoryMock
                    .Setup(x => x.FindByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Transaction)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            }
            ```

            ### Example 3: DELETE Command Handler
            ```csharp
            [Fact]
            public async Task Handle_DeletesTransaction_WhenFound()
            {
                // Arrange
                var transactionId = Guid.NewGuid();
                var command = new DeleteTransactionCommand(transactionId);
                // Minimal test data - only ID matters for deletion
                var transaction = new Transaction { Id = transactionId };

                _transactionRepositoryMock
                    .Setup(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(transaction);

                // Act
                await _handler.Handle(command, CancellationToken.None);

                // Assert
                _transactionRepositoryMock.Verify(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()), Times.Once);
                _transactionRepositoryMock.Verify(x => x.Remove(transaction), Times.Once);
            }

            [Fact]
            public async Task Handle_ThrowsNotFoundException_WhenTransactionNotFound()
            {
                // Arrange
                var command = new DeleteTransactionCommand(Guid.NewGuid());
                _transactionRepositoryMock
                    .Setup(x => x.FindByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Transaction)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            }
            ```

            ### Example 4: GET BY ID Query Handler (with AutoMapper)
            ```csharp
            [Fact]
            public async Task Handle_ReturnsTransactionDto_WhenFound()
            {
                // Arrange
                var transactionId = Guid.NewGuid();
                var query = new GetTransactionByIdQuery(transactionId);
                // Only set properties we'll verify in assertions
                var transaction = new Transaction 
                { 
                    Id = transactionId, 
                    Status = "Completed",
                    SalePrice = 250000m 
                };
                var expectedDto = TransactionDto.Create(
                    DateTime.MinValue,
                    250000m,
                    "Completed",
                    0,
                    "",
                    Guid.Empty,
                    Guid.Empty,
                    Guid.Empty,
                    Guid.Empty,
                    "",
                    "",
                    "",
                    ""
                );

                _transactionRepositoryMock
                    .Setup(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(transaction);
                _mapperMock
                    .Setup(x => x.Map<TransactionDto>(transaction))
                    .Returns(expectedDto);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(250000m, result.SalePrice);
                Assert.Equal("Completed", result.Status);
                _transactionRepositoryMock.Verify(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()), Times.Once);
                _mapperMock.Verify(x => x.Map<TransactionDto>(transaction), Times.Once);
            }

            [Fact]
            public async Task Handle_ThrowsNotFoundException_WhenTransactionNotFound()
            {
                // Arrange
                var query = new GetTransactionByIdQuery(Guid.NewGuid());
                _transactionRepositoryMock
                    .Setup(x => x.FindByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Transaction)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            }
            ```

            ### Example 5: GET ALL Query Handler (returning list)
            ```csharp
            [Fact]
            public async Task Handle_ReturnsAllTransactions()
            {
                // Arrange
                var query = new GetTransactionsQuery();
                // Minimal test data - just need IDs to verify count
                var transactions = new List<Transaction>
                {
                    new Transaction { Id = Guid.NewGuid() },
                    new Transaction { Id = Guid.NewGuid() }
                };

                _transactionRepositoryMock
                    .Setup(x => x.FindAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(transactions);
                
                // IMPORTANT: Mock singular Map<TDto>, NOT Map<List<TDto>>
                // The extension method .MapToTransactionDtoList() calls Map<TDto> for each item
                _mapperMock
                    .Setup(x => x.Map<TransactionDto>(It.IsAny<Transaction>()))
                    .Returns((Transaction t) => TransactionDto.Create(
                        DateTime.MinValue, 0, "", 0, "", Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        "", "", "", ""
                    ));

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                _transactionRepositoryMock.Verify(x => x.FindAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
            ```

            ### Example 6A: FILTERED Query - Scenario 1 (Generic FindAllAsync with Predicate)
            **Use this pattern when the handler builds a predicate and passes it to a generic FindAllAsync method**
            ```csharp
            // Handler code example:
            // var transactions = await _repository.FindAllAsync(
            //     x => x.Status == "Completed" && x.TransactionDate >= request.FromDate && x.TransactionDate <= request.ToDate,
            //     query => query.OrderBy(x => x.TransactionDate),
            //     cancellationToken);
            
            [Fact]
            public async Task Handle_ReturnsCompletedTransactionsWithinDateRange()
            {
                // Arrange
                var fromDate = new DateTime(2024, 6, 1);
                var toDate = new DateTime(2024, 6, 30);
                var query = new GetCompletedTransactionsQuery(fromDate, toDate);
                
                // Create test data that covers ALL filtering scenarios
                // ONLY set properties used in filter: Status and TransactionDate
                var allTransactions = new List<Transaction>
                {
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 6, 15) }, // ✓ Should match
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 6, 1) },  // ✓ Boundary test
                    new Transaction { Status = "Pending", TransactionDate = new DateTime(2024, 6, 15) },   // ✗ Wrong status
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 5, 31) }, // ✗ Before range
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 7, 1) }   // ✗ After range
                };

                // CRITICAL: Setup repository to ACTUALLY APPLY the predicate (like EF Core does)
                _transactionRepositoryMock
                    .Setup(x => x.FindAllAsync(
                        It.IsAny<Expression<Func<Transaction, bool>>>(),
                        It.IsAny<Func<IQueryable<Transaction>, IQueryable<Transaction>>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((
                        Expression<Func<Transaction, bool>> predicate,
                        Func<IQueryable<Transaction>, IQueryable<Transaction>> orderBy,
                        CancellationToken ct) =>
                    {
                        // Compile and apply the predicate to test data
                        var compiled = predicate.Compile();
                        var filtered = allTransactions.Where(compiled).AsQueryable();
                        var ordered = orderBy != null ? orderBy(filtered) : filtered;
                        return ordered.ToList();
                    });

                _mapperMock
                    .Setup(x => x.Map<TransactionDto>(It.IsAny<Transaction>()))
                    .Returns((Transaction t) => TransactionDto.Create(
                        t.TransactionDate, 0, t.Status, 0, "", Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        "", "", "", ""
                    ));

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert - This single test verifies: predicate logic, repository call, mapper call, result correctness, ordering
                Assert.NotNull(result);
                Assert.Equal(2, result.Count); // Only 2 match: Completed AND within date range
                Assert.All(result, dto => Assert.Equal("Completed", dto.Status));
                Assert.True(result[0].TransactionDate <= result[1].TransactionDate, "Results should be ordered by date");
            }

            [Fact]
            public async Task Handle_ReturnsEmpty_WhenNoTransactionsMatchFilter()
            {
                // Arrange
                var query = new GetCompletedTransactionsQuery(new DateTime(2024, 6, 1), new DateTime(2024, 6, 30));
                // CRITICAL EDGE CASE: Transactions exist but don't match ALL filter criteria
                // This validates the handler's predicate logic correctly filters out non-matching data
                var allTransactions = new List<Transaction>
                {
                    new Transaction { Status = "Pending", TransactionDate = new DateTime(2024, 6, 15) },    // Wrong status
                    new Transaction { Status = "Failed", TransactionDate = new DateTime(2024, 6, 20) },     // Wrong status
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 5, 31) }   // Outside date range
                };

                _transactionRepositoryMock
                    .Setup(x => x.FindAllAsync(
                        It.IsAny<Expression<Func<Transaction, bool>>>(),
                        It.IsAny<Func<IQueryable<Transaction>, IQueryable<Transaction>>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((
                        Expression<Func<Transaction, bool>> predicate,
                        Func<IQueryable<Transaction>, IQueryable<Transaction>> orderBy,
                        CancellationToken ct) =>
                    {
                        var compiled = predicate.Compile();
                        return allTransactions.Where(compiled).ToList();
                    });

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.Empty(result);
            }
            ```

            ### Example 6B: FILTERED Query - Scenario 2 (Domain-Specific Repository Method)
            **Use this pattern when the handler calls a domain-specific repository method that encapsulates the filtering logic**
            ```csharp
            // Handler code example:
            // var transactions = await _repository.FindCompletedTransactionsAsync(request.FromDate, request.ToDate, cancellationToken);
            
            [Fact]
            public async Task Handle_ReturnsCompletedTransactions_WhenTransactionsMatchCriteria()
            {
                // Arrange
                var fromDate = new DateTime(2024, 6, 1);
                var toDate = new DateTime(2024, 6, 30);
                var query = new GetCompletedTransactionsQuery(fromDate, toDate);
                
                // Only need matching data since repository method handles filtering internally
                var transactions = new List<Transaction>
                {
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 6, 15) },
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2024, 6, 20) }
                };

                // Mock the domain-specific method directly - no predicate compilation needed
                _transactionRepositoryMock
                    .Setup(x => x.FindCompletedTransactionsAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(transactions);

                _mapperMock
                    .Setup(x => x.Map<TransactionDto>(It.IsAny<Transaction>()))
                    .Returns((Transaction t) => TransactionDto.Create(
                        t.TransactionDate, 0, t.Status, 0, "", Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        "", "", "", ""
                    ));

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.All(result, dto => Assert.Equal("Completed", dto.Status));
                _transactionRepositoryMock.Verify(
                    x => x.FindCompletedTransactionsAsync(fromDate, toDate, It.IsAny<CancellationToken>()), 
                    Times.Once);
            }

            [Fact]
            public async Task Handle_ReturnsEmpty_WhenNoTransactionsMatchCriteria()
            {
                // Arrange
                var fromDate = new DateTime(2024, 6, 1);
                var toDate = new DateTime(2024, 6, 30);
                var query = new GetCompletedTransactionsQuery(fromDate, toDate);
                
                // EDGE CASE: Repository method filters internally, returns empty when criteria not met
                // This could mean: no transactions exist, OR transactions exist but don't match filters
                _transactionRepositoryMock
                    .Setup(x => x.FindCompletedTransactionsAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Transaction>());

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }

            [Fact]
            public async Task Handle_ReturnsTransaction_WhenDateMatchesBoundary()
            {
                // Arrange
                // EDGE CASE: Test boundary condition where fromDate = toDate = transaction date
                var boundaryDate = new DateTime(2024, 6, 15);
                var query = new GetCompletedTransactionsQuery(boundaryDate, boundaryDate);
                
                var transactions = new List<Transaction>
                {
                    new Transaction { Status = "Completed", TransactionDate = boundaryDate }
                };

                _transactionRepositoryMock
                    .Setup(x => x.FindCompletedTransactionsAsync(boundaryDate, boundaryDate, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(transactions);

                _mapperMock
                    .Setup(x => x.Map<TransactionDto>(It.IsAny<Transaction>()))
                    .Returns((Transaction t) => TransactionDto.Create(
                        t.TransactionDate, 0, t.Status, 0, "", Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        "", "", "", ""
                    ));

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Equal(boundaryDate, result[0].TransactionDate);
                Assert.Equal("Completed", result[0].Status);
            }
            ```

            ### Example 6C: PROJECTION Query with FindAllProjectToAsync (CRITICAL PATTERN)
            **Use this pattern when the handler calls FindAllProjectToAsync<TDto> with a filter expression**
            ```csharp
            // Handler code example:
            // var transactions = await _transactionRepository.FindAllProjectToAsync<TransactionDto>(
            //     filterExpression: t => t.Status == "Completed" && t.TransactionDate >= request.FromDate && t.TransactionDate <= request.ToDate,
            //     queryOptions: q => q.OrderBy(t => t.TransactionDate),
            //     cancellationToken: cancellationToken);
            
            [Fact]
            public async Task Handle_ReturnsCompletedTransactions_WhenTransactionsMatchCriteria()
            {
                // Arrange
                var fromDate = new DateTime(2023, 1, 1);
                var toDate = new DateTime(2023, 1, 31);
                var query = new GetCompletedTransactionsQuery(fromDate, toDate);
                
                // CRITICAL: Use actual entity type (Transaction), NOT anonymous objects
                // Only set properties used in filter or assertions
                var allTransactions = new List<Transaction>
                {
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2023, 1, 15) },
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2023, 1, 20) },
                    new Transaction { Status = "Pending", TransactionDate = new DateTime(2023, 1, 10) },
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2023, 2, 1) }
                };

                // CRITICAL: Use correct generic type - Expression<Func<Transaction, bool>>, NOT Expression<Func<object, bool>>
                _transactionRepositoryMock
                    .Setup(x => x.FindAllProjectToAsync<TransactionDto>(
                        It.IsAny<Expression<Func<Transaction, bool>>>(),
                        It.IsAny<Func<IQueryable<Transaction>, IQueryable<Transaction>>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Expression<Func<Transaction, bool>> filterExpression, Func<IQueryable<Transaction>, IQueryable<Transaction>> queryOptions, CancellationToken ct) =>
                    {
                        var compiled = filterExpression.Compile();
                        var filtered = allTransactions.Where(compiled).ToList();
                        
                        // Create DTOs from filtered entities
                        var dtos = filtered.Select(t => new TransactionDto
                        {
                            TransactionDate = t.TransactionDate,
                            Status = t.Status,
                            // Only set other properties if needed for assertions
                            SalePrice = 0,
                            CommissionRate = 0,
                            ClosingAgent = "",
                            ClientId = Guid.Empty,
                            BuyerId = Guid.Empty,
                            SellerId = Guid.Empty,
                            AssetId = Guid.Empty
                        }).OrderBy(dto => dto.TransactionDate).ToList();
                        
                        return dtos;
                    });

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.All(result, dto => Assert.Equal("Completed", dto.Status));
                Assert.True(result.All(dto => dto.TransactionDate >= fromDate && dto.TransactionDate <= toDate));
            }

            [Fact]
            public async Task Handle_ReturnsEmpty_WhenNoTransactionsMatchCriteria()
            {
                // Arrange
                var fromDate = new DateTime(2023, 1, 1);
                var toDate = new DateTime(2023, 1, 31);
                var query = new GetCompletedTransactionsQuery(fromDate, toDate);
                
                // EDGE CASE: Transactions exist but don't match ALL filter criteria
                var allTransactions = new List<Transaction>
                {
                    new Transaction { Status = "Pending", TransactionDate = new DateTime(2023, 1, 15) },
                    new Transaction { Status = "Completed", TransactionDate = new DateTime(2023, 2, 15) },
                    new Transaction { Status = "Cancelled", TransactionDate = new DateTime(2023, 1, 20) }
                };

                _transactionRepositoryMock
                    .Setup(x => x.FindAllProjectToAsync<TransactionDto>(
                        It.IsAny<Expression<Func<Transaction, bool>>>(),
                        It.IsAny<Func<IQueryable<Transaction>, IQueryable<Transaction>>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Expression<Func<Transaction, bool>> filterExpression, Func<IQueryable<Transaction>, IQueryable<Transaction>> queryOptions, CancellationToken ct) =>
                    {
                        var compiled = filterExpression.Compile();
                        var filtered = allTransactions.Where(compiled).ToList();
                        return new List<TransactionDto>();
                    });

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
            ```

            ## Previous Error Message
            {{$previousError}}
            """;

        return promptTemplate;
    }

    private List<ICodebaseFile> GetInputFiles(IElement element)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();

        // PRIMARY: Handler/Command and its DTOs (the code we're testing)
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

        // INFRASTRUCTURE: Only include interfaces we'll mock (not implementations)
        // For unit tests, we only need to know method signatures to create mocks
        var handlerFiles = filesProvider.GetFilesForMetadata(element);
        var handlerContent = string.Join("\n", handlerFiles.Select(f => f.Content));

        // Only include repository interface if handler uses it
        if (handlerContent.Contains("Repository", StringComparison.OrdinalIgnoreCase))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
        }

        // Only include pagination if handler returns paged results
        if (handlerContent.Contains("PagedResult") || handlerContent.Contains("PagedList"))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Application.Dtos.Pagination.PagedResult"));
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.PagedListInterface"));
        }

        // Only include NotFoundException if handler throws it
        if (handlerContent.Contains("NotFoundException"))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
        }

        // Only include UnitOfWork if handler saves changes
        if (handlerContent.Contains("SaveChangesAsync") || handlerContent.Contains("IUnitOfWork"))
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));
        }

        // Solution file needed for test project path discovery
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
    }

    /// <summary>
    /// Takes the Command/Query and enumerates related elements including associations to help Intent know which files to include as input.
    /// </summary>
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