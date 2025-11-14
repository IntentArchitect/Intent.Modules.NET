using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Services.Api;
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
    public class GenerateDomainEventHandlerUnitTestWithAITask : IModuleTask
    {
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
        private readonly IMetadataManager _metadataManager;
        private readonly ISolutionConfig _solution;
        private readonly IOutputRegistry _outputRegistry;
        private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

        [IntentManaged(Mode.Merge)]
        public GenerateDomainEventHandlerUnitTestWithAITask(
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

        public string TaskTypeId => "Intent.AI.UnitTests.GenerateDomainEventHandlerUnitTestWithAITask";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "Auto-Implement Unit Tests with AI Task (Domain Event Handler)";
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

            var eventHandlerElement = _metadataManager.Services(applicationId).Elements.FirstOrDefault(x => x.Id == elementId);
            if (eventHandlerElement == null)
            {
                throw new Exception($"An element was selected to be executed upon but could not be found. Please ensure you have saved your designer and try again.");
            }

            var inputFiles = GetInputFiles(applicationId, eventHandlerElement);
            Logging.Log.Info($"Context files included: {inputFiles.Count} files");
            Logging.Log.Debug($"Files: {string.Join(", ", inputFiles.Select(f => Path.GetFileName(f.FilePath)))}");

            var jsonInput = JsonConvert.SerializeObject(inputFiles, Formatting.Indented);

            var promptTemplate = GetPromptTemplate();
            var fileChangesResult = kernel.InvokeFileChangesPrompt(promptTemplate, thinkingType, new KernelArguments()
            {
                ["inputFilesJson"] = jsonInput,
                ["userProvidedContext"] = userProvidedContext,
                ["targetFileName"] = eventHandlerElement.Name,
                ["mockFramework"] = UnitTestHelpers.GetMockFramework(_applicationConfigurationProvider),
                ["slnRelativePath"] = "/" + string.Join('/', eventHandlerElement.GetParentPath().Select(x => x.Name)),
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
                You are a senior C# developer specializing in Domain-Driven Design (DDD), domain event handlers, and comprehensive unit testing. You're implementing unit tests that mock all external dependencies using {{$mockFramework}}.

                ## Primary Objective
                Create or update the existing test class file with a set of unit tests that comprehensively test the `Handle` method using xUnit and {{$mockFramework}} in the {{$targetFileName}} class.

                ## Domain Event Handler Context
                Domain event handlers in DDD respond to events that occur within the domain model (bounded context). They:
                - Handle `DomainEventNotification<TDomainEvent>` wrapped events via MediatR's `INotificationHandler<T>`
                - Access domain event properties through `notification.DomainEvent.PropertyName`
                - May interact with domain entities, repositories, domain services, and application services
                - Can trigger side effects: creating/updating entities, publishing integration events, sending commands
                - Enforce business rules and maintain domain consistency

                ## Test Coverage Requirements
                Generate tests that cover these scenarios (where applicable to the handler):
                1. **Happy Path**: Handler processes the domain event successfully and performs all expected operations
                2. **Entity Interactions**: 
                   * Verify entities are created with correct properties from domain event
                   * Verify entities are retrieved from repositories
                   * Verify entities are added to repositories
                   * Verify entity state changes (if entities have methods that modify state)
                3. **Repository Operations**:
                   * Verify Add/Update/Remove operations with correct entities
                   * Verify FindById/Query operations with correct parameters
                   * Verify UnitOfWork.SaveChangesAsync is called when needed
                4. **Service Interactions**: Verify all service methods are called with correct parameters from domain event
                5. **Event Bus Operations**: Verify integration events are published and commands are sent with correct data
                6. **Domain Logic**: Verify business rules and domain constraints are enforced
                7. **Error Handling**: 
                   * Entity not found scenarios (NotFoundException)
                   * Service throws exception, verify it propagates correctly
                   * Repository failures
                8. **Edge Cases**:
                   * Empty or null properties in domain event
                   * Default/zero values for numeric or boolean properties
                   * Multiple operations with different parameters
                9. **Operation Ordering**: Verify operations happen in correct sequence (critical for domain consistency)
                
                ## Tests to AVOID (Anti-Patterns)
                - **Don't test async/await mechanics** - Assume proper async implementation
                - **Don't test cancellationToken propagation** - Unless handler has specific cancellation logic
                - **Don't create trivial variations** - Combine related scenarios when possible
                - **Don't test MediatR infrastructure** - Focus on handler logic, not MediatR's dispatch mechanism

                ## Code File Modification Rules
                1. PRESERVE all [IntentManaged] Attributes on the existing test file's constructor, class or file.
                2. You may only create or update the test file
                3. Add using clauses for ALL classes that you use in your test (CRITICAL):
                   * Include domain event namespace (e.g., `using CleanArch1.Domain.Events;`)
                   * Include `DomainEventNotification<T>` namespace
                   * Include entity namespaces for domain entities being tested
                   * Include service interface namespaces when mocking services
                   * Include repository interface namespaces for repository mocking
                   * Include event bus namespace for `IEventBus` mocking
                   * Include any integration event/command message types that are published/sent
                4. Focus on the handler implementation - all services, repositories, event buses, and external dependencies should be mocked.

                ## Input Code Files (Organized by Priority):
                The files below include various types of code files. Understand their purpose:
                
                **PRIMARY FILES** (Code under test):
                - Domain Event Handler class - the implementation you're testing (handles `INotificationHandler<DomainEventNotification<T>>`)
                - Domain Event class - the event with properties accessed via `notification.DomainEvent.PropertyName`
                - `DomainEventNotification<T>` wrapper - wraps the domain event for MediatR
                
                **DOMAIN ENTITIES** (For understanding state):
                - Entity classes - domain entities that may be created, modified, or queried by the handler
                - Understand entity properties, constructors, and methods for state verification
                
                **REPOSITORY DEPENDENCIES** (For mocking):
                - Repository interfaces (e.g., `IStoreRepository`, `IClientRepository`) - mock these in tests
                - Verify Add(), Update(), Remove(), FindByIdAsync(), and query operations
                - Mock UnitOfWork.SaveChangesAsync() when persistence is needed
                
                **SERVICE DEPENDENCIES** (For mocking):
                - Domain service interfaces - mock these in tests
                - Application service interfaces - mock these in tests
                - Verify service method calls with correct parameters
                
                **EVENT BUS** (For mocking):
                - Event bus interface (`IEventBus`) - mock Publish() and Send() methods
                - Integration event/command message classes that are published or sent
                
                **SOLUTION FILE**:
                - .sln file - use to determine the correct test project path
                
                {{$inputFilesJson}}

                {{$fileChangesSchema}}
                
                2. The Content must contain:
                2.1. Your test file as pure code (no markdown).
                2.2. The file must have an appropriate path in the appropriate Tests project. Look for a project in the .sln file that would be appropriate and use the following relative path: '{{$slnRelativePath}}'.
                
                ## Important Reminders for Unit Testing Domain Event Handlers
                - **You are writing UNIT tests**: Mock ALL external dependencies (repositories, services, event bus, external services).
                - **Domain Event Structure**: Handler receives `DomainEventNotification<TDomainEvent>`. Access event properties via `notification.DomainEvent.PropertyName`.
                - **Entity State Verification**: When entities are created or modified, verify their properties are set correctly.
                - **Repository Mocking**: Mock repository methods (Add, FindByIdAsync, etc.) and UnitOfWork.SaveChangesAsync().
                - **Service Mocking**: Mock domain and application services to verify they're called with correct parameters from domain event.
                - **Event Bus Mocking**: Mock `Publish<T>()` and `Send<T>()` to verify integration events and commands are dispatched.
                - **Test Data**: Create domain event instances with relevant properties for each test scenario.
                - **Unknown Dependencies**: If you encounter injected dependencies that aren't in the input files, add a comment like:
                  ```csharp
                  // NOTE: Unable to locate files for dependency 'IUnknownService'.
                  // You may need to provide additional context or mock this manually.
                  ```
                - **Code Preservation**: If an existing test file exists, update it according to 'Code Preservation Requirements' below.
                - **Attributes**: Never add your own [IntentManaged] attributes to tests.

                ## Code Preservation Rules (CRITICAL)
                1. **NEVER remove or modify existing class members, methods, or properties, including their attributes or annotations**
                2. **NEVER change existing method signatures or implementations**
                3. **ONLY add new test methods when necessary**
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
                    private readonly Mock<IDomainService> _domainServiceMock;
                    private readonly Mock<IEventBus> _eventBusMock;
                    private readonly {HandlerName} _handler;

                    public {HandlerName}Tests()
                    {
                        _repositoryMock = new Mock<IRepository>();
                        _domainServiceMock = new Mock<IDomainService>();
                        _eventBusMock = new Mock<IEventBus>();
                        _handler = new {HandlerName}(_repositoryMock.Object, _domainServiceMock.Object, _eventBusMock.Object);
                    }

                    // Test methods follow...
                }
                ```

                ## Test Examples (Follow These Patterns)

                ### Example 1: Domain Event Handler Creating Entity
                ```csharp
                // Handler implementation:
                // public class ClientNameChangedDomainEventHandler : INotificationHandler<DomainEventNotification<ClientNameChangedDomainEvent>>
                // {
                //     private readonly IStoreRepository _storeRepository;
                //     public async Task Handle(DomainEventNotification<ClientNameChangedDomainEvent> notification, CancellationToken cancellationToken)
                //     {
                //         var domainEvent = notification.DomainEvent;
                //         var store = new Store { ClientName = domainEvent.NewName };
                //         _storeRepository.Add(store);
                //     }
                // }

                [Fact]
                public async Task Handle_CreatesStore_WithNewClientName()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("OldName", "NewName");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    Store? capturedStore = null;
                    _storeRepositoryMock
                        .Setup(x => x.Add(It.IsAny<Store>()))
                        .Callback<Store>(s => capturedStore = s);

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    Assert.NotNull(capturedStore);
                    Assert.Equal("NewName", capturedStore.ClientName);
                    _storeRepositoryMock.Verify(x => x.Add(It.IsAny<Store>()), Times.Once);
                }
                ```

                ### Example 2: Domain Event Handler with Service and Event Bus
                ```csharp
                // Handler implementation:
                // public class ClientNameChangedDomainEventHandler : INotificationHandler<DomainEventNotification<ClientNameChangedDomainEvent>>
                // {
                //     private readonly IStoreRepository _storeRepository;
                //     private readonly IInteractService _interactService;
                //     private readonly IEventBus _eventBus;
                //     public async Task Handle(DomainEventNotification<ClientNameChangedDomainEvent> notification, CancellationToken cancellationToken)
                //     {
                //         var domainEvent = notification.DomainEvent;
                //         var store = new Store { ClientName = domainEvent.NewName };
                //         await _interactService.ReceiveInteraction(domainEvent.OldName, domainEvent.NewName, cancellationToken);
                //         _storeRepository.Add(store);
                //         _eventBus.Publish(new ClientChangedMessageEvent { OldName = domainEvent.OldName, NewName = domainEvent.NewName });
                //         _eventBus.Send(new UpdateSystemCommand { OldName = domainEvent.OldName, NewName = domainEvent.NewName });
                //     }
                // }

                [Fact]
                public async Task Handle_CallsInteractService_WithCorrectParameters()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("John", "Jane");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    _interactServiceMock
                        .Setup(x => x.ReceiveInteraction("John", "Jane", It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    _interactServiceMock.Verify(
                        x => x.ReceiveInteraction("John", "Jane", It.IsAny<CancellationToken>()),
                        Times.Once);
                }

                [Fact]
                public async Task Handle_PublishesClientChangedMessageEvent_WithCorrectData()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("John", "Jane");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    ClientChangedMessageEvent? capturedEvent = null;
                    _eventBusMock
                        .Setup(x => x.Publish(It.IsAny<ClientChangedMessageEvent>()))
                        .Callback<ClientChangedMessageEvent>(e => capturedEvent = e);

                    _interactServiceMock
                        .Setup(x => x.ReceiveInteraction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    Assert.NotNull(capturedEvent);
                    Assert.Equal("John", capturedEvent.OldName);
                    Assert.Equal("Jane", capturedEvent.NewName);
                    _eventBusMock.Verify(x => x.Publish(It.IsAny<ClientChangedMessageEvent>()), Times.Once);
                }

                [Fact]
                public async Task Handle_SendsUpdateSystemCommand_WithCorrectData()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("John", "Jane");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    UpdateSystemCommand? capturedCommand = null;
                    _eventBusMock
                        .Setup(x => x.Send(It.IsAny<UpdateSystemCommand>()))
                        .Callback<UpdateSystemCommand>(c => capturedCommand = c);

                    _interactServiceMock
                        .Setup(x => x.ReceiveInteraction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    Assert.NotNull(capturedCommand);
                    Assert.Equal("John", capturedCommand.OldName);
                    Assert.Equal("Jane", capturedCommand.NewName);
                    _eventBusMock.Verify(x => x.Send(It.IsAny<UpdateSystemCommand>()), Times.Once);
                }

                [Fact]
                public async Task Handle_PerformsAllOperations_InCorrectOrder()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("John", "Jane");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    var callOrder = new List<string>();

                    _interactServiceMock
                        .Setup(x => x.ReceiveInteraction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .Callback(() => callOrder.Add("ReceiveInteraction"))
                        .Returns(Task.CompletedTask);

                    _storeRepositoryMock
                        .Setup(x => x.Add(It.IsAny<Store>()))
                        .Callback(() => callOrder.Add("AddStore"));

                    _eventBusMock
                        .Setup(x => x.Publish(It.IsAny<ClientChangedMessageEvent>()))
                        .Callback(() => callOrder.Add("PublishEvent"));

                    _eventBusMock
                        .Setup(x => x.Send(It.IsAny<UpdateSystemCommand>()))
                        .Callback(() => callOrder.Add("SendCommand"));

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    Assert.Equal(4, callOrder.Count);
                    Assert.Equal("ReceiveInteraction", callOrder[0]);
                    Assert.Equal("AddStore", callOrder[1]);
                    Assert.Equal("PublishEvent", callOrder[2]);
                    Assert.Equal("SendCommand", callOrder[3]);
                }
                ```

                ### Example 3: Domain Event Handler with Repository Query and Entity Update
                ```csharp
                // Handler implementation:
                // public class OrderShippedDomainEventHandler : INotificationHandler<DomainEventNotification<OrderShippedDomainEvent>>
                // {
                //     private readonly ICustomerRepository _customerRepository;
                //     public async Task Handle(DomainEventNotification<OrderShippedDomainEvent> notification, CancellationToken cancellationToken)
                //     {
                //         var domainEvent = notification.DomainEvent;
                //         var customer = await _customerRepository.FindByIdAsync(domainEvent.CustomerId, cancellationToken);
                //         if (customer == null) throw new NotFoundException($"Customer {domainEvent.CustomerId} not found");
                //         customer.IncrementOrderCount();
                //         await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                //     }
                // }

                [Fact]
                public async Task Handle_IncrementsCustomerOrderCount_WhenCustomerExists()
                {
                    // Arrange
                    var customerId = Guid.NewGuid();
                    var domainEvent = new OrderShippedDomainEvent(customerId);
                    var notification = new DomainEventNotification<OrderShippedDomainEvent>(domainEvent);

                    var customer = new Customer { Id = customerId, OrderCount = 5 };

                    _customerRepositoryMock
                        .Setup(x => x.FindByIdAsync(customerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(customer);

                    _customerRepositoryMock
                        .Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(1);

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    Assert.Equal(6, customer.OrderCount);
                    _customerRepositoryMock.Verify(x => x.FindByIdAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
                    _customerRepositoryMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
                }

                [Fact]
                public async Task Handle_ThrowsNotFoundException_WhenCustomerNotFound()
                {
                    // Arrange
                    var customerId = Guid.NewGuid();
                    var domainEvent = new OrderShippedDomainEvent(customerId);
                    var notification = new DomainEventNotification<OrderShippedDomainEvent>(domainEvent);

                    _customerRepositoryMock
                        .Setup(x => x.FindByIdAsync(customerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Customer?)null);

                    // Act & Assert
                    await Assert.ThrowsAsync<NotFoundException>(
                        () => _handler.Handle(notification, CancellationToken.None));

                    _customerRepositoryMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
                }
                ```

                ### Example 4: Edge Cases and Error Handling
                ```csharp
                [Fact]
                public async Task Handle_HandlesEmptyStrings_InDomainEvent()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("", "");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    _interactServiceMock
                        .Setup(x => x.ReceiveInteraction("", "", It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

                    // Act
                    await _handler.Handle(notification, CancellationToken.None);

                    // Assert
                    _interactServiceMock.Verify(
                        x => x.ReceiveInteraction("", "", It.IsAny<CancellationToken>()),
                        Times.Once);
                }

                [Fact]
                public async Task Handle_DoesNotPublishOrSend_WhenServiceFails()
                {
                    // Arrange
                    var domainEvent = new ClientNameChangedDomainEvent("John", "Jane");
                    var notification = new DomainEventNotification<ClientNameChangedDomainEvent>(domainEvent);

                    _interactServiceMock
                        .Setup(x => x.ReceiveInteraction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new InvalidOperationException("Service error"));

                    // Act & Assert
                    await Assert.ThrowsAsync<InvalidOperationException>(
                        () => _handler.Handle(notification, CancellationToken.None));

                    // Verify subsequent operations were never called
                    _storeRepositoryMock.Verify(x => x.Add(It.IsAny<Store>()), Times.Never);
                    _eventBusMock.Verify(x => x.Publish(It.IsAny<ClientChangedMessageEvent>()), Times.Never);
                    _eventBusMock.Verify(x => x.Send(It.IsAny<UpdateSystemCommand>()), Times.Never);
                }
                ```

                ## Previous Error Message
                {{$previousError}}
                """;

            return promptTemplate;
        }

        private List<ICodebaseFile> GetInputFiles(string applicationId, IElement eventHandlerElement)
        {
            var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();

            // PRIMARY: Domain event handler implementation
            var inputFiles = filesProvider.GetFilesForMetadata(eventHandlerElement).ToList();

            // Get the handler content for dependency scanning
            var handlerFiles = filesProvider.GetFilesForMetadata(eventHandlerElement);
            var handlerContent = string.Join("\n", handlerFiles.Select(f => f.Content));

            // PRIMARY: Domain Event class (the event being handled)
            // Domain event handlers are connected to domain events via associations
            // The association has a targetEnd with a typeReference pointing to the domain event
            var domainEventAssociation = eventHandlerElement.AssociatedElements
                .FirstOrDefault(a => a.SpecializationType == "Domain Event Handler Association Target End");
            
            if (domainEventAssociation != null && domainEventAssociation.TypeReference?.Element != null)
            {
                var domainEvent = domainEventAssociation.TypeReference.Element;
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(domainEvent));
            }

            // INFRASTRUCTURE: DomainEventNotification<T> wrapper
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.MediatR.DomainEvents.DomainEventNotification"));

            // INFRASTRUCTURE: INotificationHandler<T> interface
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.MediatR.DomainEvents.DomainEventHandler"));

            // DOMAIN ENTITIES: Check associations for ClassModels (domain entities)
            var entityAssociations = eventHandlerElement.AssociatedElements
                .Where(a => a.SpecializationTypeId == "04e12b51-ed12-42a3-9667-a6aa81bb6d10"); // Class specialization
            
            foreach (var entity in entityAssociations)
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(entity));
            }

            // DEPENDENCIES: Scan handler content for injected services and event bus
            var detectedDependencies = DetectDependencies(handlerContent);

            // Include IEventBus if detected
            if (detectedDependencies.UsesEventBus)
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Eventing.Contracts.EventBusInterface"));
            }

            // Include Repository interfaces if handler uses them
            if (detectedDependencies.UsesRepository)
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
                
                // Try to find specific repository interfaces used
                var repositoryMatches = Regex.Matches(handlerContent, @"I(\w+)Repository", RegexOptions.Compiled);
                foreach (Match match in repositoryMatches)
                {
                    var repositoryName = match.Groups[0].Value; // Full interface name like IStoreRepository
                    var entityName = match.Groups[1].Value; // Entity name like Store
                    
                    // Try to find the entity in metadata - search across all designers
                    var servicesElements = _metadataManager.Services(applicationId).Elements;
                    var entityElement = servicesElements.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.Ordinal));
                    if (entityElement != null)
                    {
                        inputFiles.AddRange(filesProvider.GetFilesForMetadata(entityElement));
                    }
                }
            }

            // Include NotFoundException if handler throws it
            if (detectedDependencies.UsesNotFoundException)
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.NotFoundException"));
            }

            // Include UnitOfWork if handler saves changes
            if (detectedDependencies.UsesUnitOfWork)
            {
                inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface"));
            }

            // SERVICES: Include service interfaces that are injected
            var serviceMatches = Regex.Matches(handlerContent, @"I(\w+)Service", RegexOptions.Compiled);
            foreach (Match match in serviceMatches)
            {
                var serviceName = match.Groups[0].Value;
                
                // Try to find the service interface in metadata
                var servicesElements = _metadataManager.Services(applicationId).Elements;
                var serviceElement = servicesElements.FirstOrDefault(e => e.Name.Equals(serviceName.Substring(1), StringComparison.Ordinal) ||
                                                                      e.Name.Equals(serviceName, StringComparison.Ordinal));
                if (serviceElement != null)
                {
                    inputFiles.AddRange(filesProvider.GetFilesForMetadata(serviceElement));
                }
            }

            // MESSAGES: Include any integration events or commands that are published/sent
            var publishedMessageTypes = DetectPublishedMessageTypes(handlerContent);
            foreach (var messageTypeName in publishedMessageTypes)
            {
                var servicesElements = _metadataManager.Services(applicationId).Elements;
                var messageElement = servicesElements.FirstOrDefault(e => e.Name.Equals(messageTypeName, StringComparison.Ordinal));
                if (messageElement != null)
                {
                    inputFiles.AddRange(filesProvider.GetFilesForMetadata(messageElement));
                }
            }

            // ENTITIES: Detect entity instantiation patterns (new EntityName { ... })
            var entityInstantiationMatches = Regex.Matches(handlerContent, @"new\s+(\w+)\s*\{", RegexOptions.Compiled);
            foreach (Match match in entityInstantiationMatches)
            {
                var entityName = match.Groups[1].Value;
                
                // Skip common types
                if (entityName == "var" || entityName == "string" || entityName == "int" || 
                    entityName.EndsWith("Event") || entityName.EndsWith("Command"))
                {
                    continue;
                }
                
                // Try to find the entity
                var servicesElements = _metadataManager.Services(applicationId).Elements;
                var entityElement = servicesElements.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.Ordinal));
                if (entityElement != null)
                {
                    inputFiles.AddRange(filesProvider.GetFilesForMetadata(entityElement));
                }
            }

            // Solution file needed for test project path discovery
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

            return inputFiles.Distinct().ToList();
        }

        private static DependencyInfo DetectDependencies(string handlerContent)
        {
            return new DependencyInfo
            {
                UsesEventBus = handlerContent.Contains("IEventBus", StringComparison.Ordinal) ||
                              handlerContent.Contains("_eventBus", StringComparison.Ordinal),
                UsesRepository = handlerContent.Contains("Repository", StringComparison.OrdinalIgnoreCase),
                UsesNotFoundException = handlerContent.Contains("NotFoundException", StringComparison.Ordinal),
                UsesUnitOfWork = handlerContent.Contains("SaveChangesAsync", StringComparison.Ordinal) ||
                                handlerContent.Contains("IUnitOfWork", StringComparison.Ordinal)
            };
        }

        private static List<string> DetectPublishedMessageTypes(string handlerContent)
        {
            var messageTypes = new List<string>();

            // Detect Publish<T>() calls
            var publishMatches = Regex.Matches(handlerContent, @"\.Publish<(\w+)>\s*\(", RegexOptions.Compiled);
            foreach (Match match in publishMatches)
            {
                if (match.Groups.Count > 1)
                {
                    messageTypes.Add(match.Groups[1].Value);
                }
            }

            // Detect Publish(new MessageType { ... })
            var publishNewMatches = Regex.Matches(handlerContent, @"\.Publish\s*\(\s*new\s+(\w+)", RegexOptions.Compiled);
            foreach (Match match in publishNewMatches)
            {
                if (match.Groups.Count > 1)
                {
                    messageTypes.Add(match.Groups[1].Value);
                }
            }

            // Detect Send<T>() calls
            var sendMatches = Regex.Matches(handlerContent, @"\.Send<(\w+)>\s*\(", RegexOptions.Compiled);
            foreach (Match match in sendMatches)
            {
                if (match.Groups.Count > 1)
                {
                    messageTypes.Add(match.Groups[1].Value);
                }
            }

            // Detect Send(new MessageType { ... })
            var sendNewMatches = Regex.Matches(handlerContent, @"\.Send\s*\(\s*new\s+(\w+)", RegexOptions.Compiled);
            foreach (Match match in sendNewMatches)
            {
                if (match.Groups.Count > 1)
                {
                    messageTypes.Add(match.Groups[1].Value);
                }
            }

            return messageTypes.Distinct().ToList();
        }

        private class DependencyInfo
        {
            public bool UsesEventBus { get; set; }
            public bool UsesRepository { get; set; }
            public bool UsesNotFoundException { get; set; }
            public bool UsesUnitOfWork { get; set; }
        }
    }
}