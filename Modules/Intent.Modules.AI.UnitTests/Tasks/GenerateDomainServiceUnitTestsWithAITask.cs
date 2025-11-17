using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AI.UnitTests.Utilities;
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

namespace Intent.Modules.AI.UnitTests.Tasks
{
#nullable enable

    [IntentManaged(Mode.Merge)]
    public class GenerateDomainServiceUnitTestsWithAITask : IModuleTask
    {
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
        private readonly IMetadataManager _metadataManager;
        private readonly ISolutionConfig _solution;
        private readonly IOutputRegistry _outputRegistry;
        private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

        [IntentManaged(Mode.Merge)]
        public GenerateDomainServiceUnitTestsWithAITask(
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

        public string TaskTypeId => "Intent.AI.UnitTests.GenerateDomainServiceUnitTestsWithAITask";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "Auto-Implement Unit Tests with AI Task (Domain Service)";
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

            var domainDesigner = _metadataManager.GetDesigner(applicationId, "Domain");
            var domainService = domainDesigner.GetElementsOfType(SpecializationTypeIds.DomainService)
                .FirstOrDefault(x => x.Id == elementId);

            var inputFiles = GetInputFiles(applicationId, domainService);
            Logging.Log.Info($"Context files included: {inputFiles.Count} files");
            Logging.Log.Debug($"Files: {string.Join(", ", inputFiles.Select(f => Path.GetFileName(f.FilePath)))}");

            var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

            var promptTemplate = GetPromptTemplate();
            var fileChangesResult = kernel.InvokeFileChangesPrompt(promptTemplate, thinkingType, new KernelArguments()
            {
                ["inputFilesJson"] = jsonInput,
                ["userProvidedContext"] = userProvidedContext,
                ["targetFileName"] = domainService.Name + "Tests",
                ["mockFramework"] = UnitTestHelpers.GetMockFramework(_applicationConfigurationProvider),
                ["slnRelativePath"] = "/" + string.Join('/', domainService.GetParentPath().Select(x => x.Name)),
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
                You are a senior C# developer specializing in Domain-Driven Design (DDD). You're implementing unit tests for domain services that coordinate business logic across multiple aggregates and entities.

                ## Primary Objective
                Create or update the existing test class file with a set of unit tests that comprehensively test the {{$targetFileName}} domain service operation using xUnit and {{$mockFramework}}.

                ## Code File Modification Rules
                1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
                2. You may only create or update the test file
                3. Add using clauses for ALL classes that you use in your test (CRITICAL):
                   * Include the entity namespace (e.g., `using CleanArch1.Domain.Entities;`) when using domain entities in test data
                   * Include repository namespaces when mocking repositories
                   * Include DTO namespaces for DTOs used in assertions
                4. Focus on the handler implementation - all infrastructure types (repositories, mappers, etc.) should be mocked.

                ## Domain Service Testing Principles
                Domain services:
                - Coordinate business logic that doesn't naturally fit within a single aggregate
                - Work with repositories to load and manipulate domain entities
                - Call methods on domain entities to maintain domain invariants and modify their state
                - Should maintain transactional consistency through Unit of Work
                
                **CRITICAL for Testing**: Domain services orchestrate behavior by calling entity methods. Your tests MUST verify:
                1. The service loaded the correct entities (repository interaction)
                2. The entities' STATE changed correctly after calling their methods (property/collection assertions)
                3. The changes will be persisted (SaveChangesAsync called if needed)
                
                Verifying ONLY repository calls without inspecting entity state is an INCOMPLETE test.

                ## Test Coverage Requirements
                Generate tests that cover these scenarios (where applicable to the domain service operation):
                1. **Happy Path**: Operation executes successfully with valid input
                2. **Entity Not Found**: Test exceptions when repository.FindByIdAsync returns null
                3. **Domain Invariant Violations**: Test that domain rules are enforced (e.g., invalid state transitions)
                4. **Edge Cases**:
                   * Boundary values for value objects (e.g., zero amounts, min/max dates)
                   * Null or invalid input parameters
                   * Entity state preconditions (e.g., entity must be in certain state before operation)
                5. **Entity State Changes (CRITICAL)**: 
                   * ALWAYS verify that domain entities' state was actually modified by inspecting their properties/collections after the operation
                   * Use Callback to capture entities OR directly inspect the entity reference passed to the service
                   * Example: After calling `service.RevalueAsset(...)`, assert that `asset.CurrentValuation == expectedValue`
                   * Don't just verify repository methods were called - verify the entity's state changed correctly
                6. **Repository Interactions**: Verify correct repository method calls (Find, Add, Update, Remove)
                7. **Unit of Work**: Verify SaveChangesAsync is called when state changes occur
                
                ## Tests to AVOID (Anti-Patterns)
                - **Don't create separate tests ONLY for parameter verification** - Parameters are already verified in functional tests
                - **Don't test cancellationToken propagation** - Unless handler has specific cancellation logic, this is implicitly tested
                - **Don't create multiple tests for trivial variations** - Combine related scenarios or use Theory/InlineData if needed

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

                ## Test Class Structure for Domain Services
                Your test class should follow this structure:
                ```csharp
                public class {DomainServiceName}Tests
                {
                    private readonly Mock<IRepository1> _repository1Mock;
                    private readonly Mock<IRepository2> _repository2Mock; // If service uses multiple repositories
                    private readonly {DomainServiceName} _service;

                    public {DomainServiceName}Tests()
                    {
                        _repository1Mock = new Mock<IRepository1>();
                        _repository2Mock = new Mock<IRepository2>(); // Only if needed
                        _service = new {DomainServiceName}(_repository1Mock.Object, _repository2Mock.Object);
                    }

                    // Test methods follow...
                }
                ```

                ## Domain Service Test Examples (Follow These Patterns)
                
                **Note on DTO Construction in Examples:** 
                The examples below use `TDto.Create(...)` factory method pattern. If the DTO in your context uses property initialization instead, adapt the pattern to `new TDto { Property1 = value1, Property2 = value2 }`. Check the DTO file provided in the input context to determine which pattern to use.
                
                ### Example 1: Domain Service Operation with Entity Loading and Modification
                ```csharp
                // Domain Service: RevalueAssetAsync(clientId, assetId, newValuation)
                // Logic: Load client, call client.UpdateAssetValuation(assetId, valuation)
                
                [Fact]
                public async Task RevalueAssetAsync_UpdatesAssetValuation_WhenClientAndAssetExist()
                {
                    // Arrange
                    var clientId = Guid.NewGuid();
                    var assetId = Guid.NewGuid();
                    var newValuation = new Valuation 
                    { 
                        Amount = 250000m, 
                        ValuationDate = DateTime.UtcNow,
                        AppraiserName = "John Appraiser"
                    };
                    
                    // CRITICAL: Create test entity and ensure we can verify state changes
                    var client = new Client { Id = clientId };
                    var asset = new Asset { Id = assetId, CurrentValuation = 100000m };
                    client.AddAsset(asset); // Set up initial state
                    
                    _clientRepositoryMock
                        .Setup(x => x.FindByIdAsync(clientId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(client);

                    // Act
                    await _service.RevalueAssetAsync(clientId, assetId, newValuation);

                    // Assert - CRITICAL: Verify domain entity STATE was actually modified
                    _clientRepositoryMock.Verify(
                        x => x.FindByIdAsync(clientId, It.IsAny<CancellationToken>()), 
                        Times.Once);
                    
                    // Verify the asset's valuation was updated through the client aggregate
                    var updatedAsset = client.Assets.FirstOrDefault(a => a.Id == assetId);
                    Assert.NotNull(updatedAsset);
                    Assert.Equal(250000m, updatedAsset.CurrentValuation);
                    Assert.Equal(newValuation.ValuationDate, updatedAsset.ValuationDate);
                    Assert.Equal(newValuation.AppraiserName, updatedAsset.AppraiserName);
                }

                [Fact]
                public async Task RevalueAssetAsync_ThrowsException_WhenClientNotFound()
                {
                    // Arrange
                    var clientId = Guid.NewGuid();
                    var assetId = Guid.NewGuid();
                    var newValuation = new Valuation { Amount = 250000m };
                    
                    _clientRepositoryMock
                        .Setup(x => x.FindByIdAsync(clientId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Client)null);

                    // Act & Assert
                    await Assert.ThrowsAsync<InvalidOperationException>(
                        () => _sut.RevalueAssetAsync(clientId, assetId, newValuation));
                }
                
                [Fact]
                public async Task RevalueAssetAsync_ThrowsArgumentNullException_WhenValuationIsNull()
                {
                    // Arrange
                    var clientId = Guid.NewGuid();
                    var assetId = Guid.NewGuid();
                    Valuation nullValuation = null;

                    // Act & Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(
                        () => _sut.RevalueAssetAsync(clientId, assetId, nullValuation));
                }
                ```

                ### Example 2: Domain Service Creating New Entity
                ```csharp
                // Domain Service: ProcessTransactionAsync(clientId, transactionDetails)
                // Logic: Load client, create Transaction entity, add to repository
                
                [Fact]
                public async Task ProcessTransactionAsync_CreatesTransaction_WhenClientExists()
                {
                    // Arrange
                    var clientId = Guid.NewGuid();
                    var transactionDetails = new TransactionDetails 
                    { 
                        Amount = 100000m, 
                        TransactionDate = DateTime.UtcNow 
                    };
                    
                    var client = new Client { Id = clientId, IsActive = true };
                    Transaction capturedTransaction = null;
                    
                    _clientRepositoryMock
                        .Setup(x => x.FindByIdAsync(clientId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(client);
                    
                    _transactionRepositoryMock
                        .Setup(x => x.Add(It.IsAny<Transaction>()))
                        .Callback<Transaction>(t => capturedTransaction = t);
                    
                    _transactionRepositoryMock
                        .Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

                    // Act
                    await _sut.ProcessTransactionAsync(clientId, transactionDetails);

                    // Assert
                    Assert.NotNull(capturedTransaction);
                    Assert.Equal(clientId, capturedTransaction.ClientId);
                    Assert.Equal(100000m, capturedTransaction.Amount);
                    _transactionRepositoryMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
                    _transactionRepositoryMock.Verify(
                        x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), 
                        Times.Once);
                }
                ```

                ### Example 3: Domain Service Coordinating Multiple Aggregates
                ```csharp
                // Domain Service: TransferAssetAsync(fromClientId, toClientId, assetId)
                // Logic: Load both clients, verify rules, update asset ownership
                
                [Fact]
                public async Task TransferAssetAsync_TransfersAsset_WhenBothClientsExist()
                {
                    // Arrange
                    var fromClientId = Guid.NewGuid();
                    var toClientId = Guid.NewGuid();
                    var assetId = Guid.NewGuid();
                    
                    var asset = new Asset { Id = assetId, ClientId = fromClientId, Value = 500000m };
                    var fromClient = new Client { Id = fromClientId, IsActive = true };
                    fromClient.AddAsset(asset); // Establish ownership
                    
                    var toClient = new Client { Id = toClientId, IsActive = true };
                    
                    _clientRepositoryMock
                        .Setup(x => x.FindByIdAsync(fromClientId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(fromClient);
                    
                    _clientRepositoryMock
                        .Setup(x => x.FindByIdAsync(toClientId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(toClient);
                    
                    _assetRepositoryMock
                        .Setup(x => x.FindByIdAsync(assetId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(asset);

                    // Act
                    await _service.TransferAssetAsync(fromClientId, toClientId, assetId);

                    // Assert - Verify BOTH domain state changes AND repository interactions
                    // CRITICAL: Verify the asset ownership actually changed
                    Assert.Equal(toClientId, asset.ClientId);
                    
                    // Verify asset was removed from source client and added to destination
                    Assert.DoesNotContain(asset, fromClient.Assets);
                    Assert.Contains(asset, toClient.Assets);
                    
                    // Repository verifications (secondary to state assertions)
                    _clientRepositoryMock.Verify(
                        x => x.FindByIdAsync(fromClientId, It.IsAny<CancellationToken>()), 
                        Times.Once);
                    _clientRepositoryMock.Verify(
                        x => x.FindByIdAsync(toClientId, It.IsAny<CancellationToken>()), 
                        Times.Once);
                    _assetRepositoryMock.Verify(
                        x => x.FindByIdAsync(assetId, It.IsAny<CancellationToken>()), 
                        Times.Once);
                }
                
                [Fact]
                public async Task TransferAssetAsync_ThrowsException_WhenFromClientNotActive()
                {
                    // Arrange
                    var fromClientId = Guid.NewGuid();
                    var toClientId = Guid.NewGuid();
                    var assetId = Guid.NewGuid();
                    
                    var fromClient = new Client { Id = fromClientId, IsActive = false };
                    
                    _clientRepositoryMock
                        .Setup(x => x.FindByIdAsync(fromClientId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(fromClient);

                    // Act & Assert
                    await Assert.ThrowsAsync<InvalidOperationException>(
                        () => _sut.TransferAssetAsync(fromClientId, toClientId, assetId));
                }
                ```

                ### Example 4: Domain Service with Complex Business Rules
                ```csharp
                // Domain Service: CalculateCommissionAsync(transactionId)
                // Logic: Load transaction, apply business rules, create commission record
                
                [Fact]
                public async Task CalculateCommissionAsync_CalculatesCorrectAmount_ForStandardTransaction()
                {
                    // Arrange
                    var transactionId = Guid.NewGuid();
                    var transaction = new Transaction 
                    { 
                        Id = transactionId,
                        Amount = 500000m,
                        CommissionRate = 0.03m
                    };
                    Commission capturedCommission = null;
                    
                    _transactionRepositoryMock
                        .Setup(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(transaction);
                    
                    _commissionRepositoryMock
                        .Setup(x => x.Add(It.IsAny<Commission>()))
                        .Callback<Commission>(c => capturedCommission = c);

                    // Act
                    await _sut.CalculateCommissionAsync(transactionId);

                    // Assert
                    Assert.NotNull(capturedCommission);
                    Assert.Equal(15000m, capturedCommission.Amount); // 500000 * 0.03
                    Assert.Equal(transactionId, capturedCommission.TransactionId);
                    _commissionRepositoryMock.Verify(x => x.Add(It.IsAny<Commission>()), Times.Once);
                }
                
                [Fact]
                public async Task CalculateCommissionAsync_AppliesCap_ForHighValueTransaction()
                {
                    // Arrange
                    var transactionId = Guid.NewGuid();
                    var transaction = new Transaction 
                    { 
                        Id = transactionId,
                        Amount = 10000000m,  // High value
                        CommissionRate = 0.03m
                    };
                    Commission capturedCommission = null;
                    
                    _transactionRepositoryMock
                        .Setup(x => x.FindByIdAsync(transactionId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(transaction);
                    
                    _commissionRepositoryMock
                        .Setup(x => x.Add(It.IsAny<Commission>()))
                        .Callback<Commission>(c => capturedCommission = c);

                    // Act
                    await _sut.CalculateCommissionAsync(transactionId);

                    // Assert
                    Assert.NotNull(capturedCommission);
                    Assert.Equal(50000m, capturedCommission.Amount); // Capped at max
                    _commissionRepositoryMock.Verify(x => x.Add(It.IsAny<Commission>()), Times.Once);
                }
                ```

                ## Critical Domain Service Testing Reminders
                - **Mock ALL repositories**: Domain services should not touch real databases
                - **Focus on coordination logic**: Test how the service orchestrates between entities and repositories
                - **ALWAYS verify domain entity state changes (HIGHEST PRIORITY)**: 
                  * Domain services coordinate behavior across aggregates by calling methods on domain entities
                  * After the service executes, ALWAYS inspect the entity's properties/collections to verify state changed correctly
                  * Example: If service calls `client.UpdateAssetValuation(...)`, assert `client.Assets[x].CurrentValuation == expectedValue`
                  * Repository verification alone (e.g., `Verify(x => x.FindByIdAsync(...))`) is INSUFFICIENT - you must prove the entity's state changed
                  * Use the entity reference returned from repository mock to inspect its final state in assertions
                - **Test business rule enforcement**: Ensure domain invariants are maintained
                - **Test all exception paths**: Domain services often have multiple failure scenarios
                - **Minimal test data**: Only set properties relevant to the specific test scenario
                - **Clear test names**: Use pattern `{MethodName}_{ExpectedBehavior}_When{Condition}`
                
                {{$previousError}}
                """;

            return promptTemplate;
        }

        private List<ICodebaseFile> GetInputFiles(string applicationId, IElement domainService)
        {
            var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();

            // PRIMARY: Domain service implementation and interface
            var inputFiles = filesProvider.GetFilesForMetadata(domainService).ToList();
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(domainService));

            // Include each domain service operation and its parameter types (DTO + Class)
            foreach (var operation in domainService.ChildElements.Where(x => x.SpecializationTypeId == SpecializationTypeIds.DomainServiceOperation))
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(operation));
                // DTO parameters
                foreach (var paramDto in operation.ChildElements.Where(p => p.TypeReference?.Element?.SpecializationTypeId == SpecializationTypeIds.Dto).Select(p => p.TypeReference.Element))
                {
                    inputFiles.AddRange(filesProvider.GetFilesForMetadata(paramDto));
                }
                // Class (domain entity/value object) parameters
                foreach (var paramClass in operation.ChildElements.Where(p => p.TypeReference?.Element?.SpecializationTypeId == SpecializationTypeIds.Class).Select(p => p.TypeReference.Element))
                {
                    inputFiles.AddRange(filesProvider.GetFilesForMetadata(paramClass));
                }
            }

            // DOMAIN: Include related entities, value objects, and their repositories
            var relatedElements = UnitTestHelpers.GetRelatedElements(domainService);
            foreach (var relatedElement in relatedElements)
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(relatedElement));
            }

            // Get domain service implementation content to detect dependencies
            var serviceFiles = filesProvider.GetFilesForMetadata(domainService);
            var serviceContent = string.Join("\n", serviceFiles.Select(f => f.Content));

            // INFRASTRUCTURE: Include repository interfaces that the service uses
            if (serviceContent.Contains("Repository", StringComparison.OrdinalIgnoreCase))
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
            }

            // Include exception types if used
            if (serviceContent.Contains("NotFoundException"))
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
            }

            // Include UnitOfWork if service saves changes
            if (serviceContent.Contains("SaveChangesAsync") || serviceContent.Contains("IUnitOfWork"))
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));
            }

            // Solution file needed for test project path discovery
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

            return inputFiles.Distinct().ToList();
        }
    }
}