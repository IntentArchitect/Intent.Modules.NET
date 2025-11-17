using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AI.UnitTests;
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

namespace Intent.Modules.AI.UnitTests.Tasks;

#nullable enable

[IntentManaged(Mode.Merge)]
public class GenerateIntegrationEventHandlerUnitTestsWithAITask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
    private readonly IMetadataManager _metadataManager;
    private readonly ISolutionConfig _solution;
    private readonly IOutputRegistry _outputRegistry;
    private readonly IntentSemanticKernelFactory _intentSemanticKernelFactory;

    [IntentManaged(Mode.Merge)]
    public GenerateIntegrationEventHandlerUnitTestsWithAITask(
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

    public string TaskTypeId => "Intent.AI.UnitTests.GenerateIntegrationEventHandlerUnitTestsWithAITask";
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public string TaskTypeName => "Auto-Implement Unit Tests with AI Task (Integration Event Handler)";
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

        var servicesDesigner = _metadataManager.GetDesigner(applicationId, "Services");
        var eventHandlerElement = servicesDesigner.GetElementsOfType(SpecializationTypeIds.IntegrationEventHandler)
            .FirstOrDefault(x => x.Id == elementId);
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
            You are a senior C# developer specializing in event-driven architecture, integration event handlers, and comprehensive unit testing. You're implementing unit tests that mock all external dependencies using {{$mockFramework}}.

            ## Primary Objective
            Create or update the existing test class file with a set of unit tests that comprehensively test the `HandleAsync` method using xUnit and {{$mockFramework}} in the {{$targetFileName}} class.

            ## Test Coverage Requirements
            Generate tests that cover these scenarios (where applicable to the handler):
            1. **Happy Path**: Handler processes the message successfully and performs all expected operations
            2. **Service Interactions**: Verify all service methods are called with correct parameters extracted from the message
            3. **Event Bus Operations**: Verify events are published and commands are sent with correct data
            4. **Error Handling**: Service throws exception, verify it propagates correctly
            5. **Edge Cases**:
               * Empty or null string properties in the message
               * Default/zero values for numeric or boolean properties
               * Multiple service calls with different parameters
            6. **Negative Testing**:
               * Invalid message states
               * Service method failures
               * Partial operation failures (e.g., first service succeeds, second fails)
            
            ## Tests to AVOID (Anti-Patterns)
            - **Don't test async/await mechanics** - Assume proper async implementation
            - **Don't test cancellationToken propagation** - Unless handler has specific cancellation logic
            - **Don't create trivial variations** - Combine related scenarios when possible

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
            - Integration Event Handler class - the implementation you're testing
            - Integration Event Message class - the incoming message type with properties
            - `IIntegrationEventHandler<T>` interface - the base handler contract
            
            **SERVICE DEPENDENCIES** (For mocking):
            - Service interfaces (e.g., `IReceiveService`, `IOrderService`) - mock these in tests
            - Event bus interface (`IEventBus`) - mock Publish() and Send() methods
            - Repository interfaces (if handler uses them directly) - mock these in tests
            
            **MESSAGE TYPES** (For test assertions):
            - Additional event/command message classes that are published or sent by the handler
            - Use these types when verifying Publish() and Send() calls
            
            **SOLUTION FILE**:
            - .sln file - use to determine the correct test project path
            
            {{$inputFilesJson}}

            {{$fileChangesSchema}}
            
            2. The Content must contain:
            2.1. Your test file as pure code (no markdown).
            2.2. The file must have an appropriate path in the appropriate Tests project. Look for a project in the .sln file that would be appropriate and use the following relative path: '{{$slnRelativePath}}'.
            
            ## Important Reminders for Unit Testing
            - **You are writing UNIT tests**: Mock ALL external dependencies (services, event bus, repositories, external services).
            - **Service interfaces are for mocking**: Use them to understand method signatures, then mock the calls.
            - **Event Bus mocking**: Mock `Publish<T>()` and `Send<T>()` methods to verify correct messages are sent.
            - **Message validation**: Verify handler extracts correct data from incoming message and passes to services.
            - **Test data minimalism**: Only set message properties that are actually used by the handler.
            - **Unknown dependencies**: If you encounter injected dependencies that aren't in the input files, add a comment like:
              ```csharp
              // NOTE: Unable to locate files for dependency 'IUnknownService'.
              // You may need to provide additional context or mock this manually.
              ```
            - **Code preservation**: If an existing test file exists, update it according to 'Code Preservation Requirements' below.
            - **Attributes**: Never add your own [IntentManaged] attributes to tests.

            ## Test Data Quality Rules
            - **Test Data Minimalism (HIGHEST PRIORITY)**:
              * BAD: Setting ALL properties on test data when only 1-2 are relevant to the test
              * GOOD: Only set properties that are directly used in assertions or affect test behavior
              * Use minimal identifiers or empty strings when property value doesn't matter: "", Guid.Empty, 0
              * Only use meaningful values when the actual value is relevant to the test
            
            - **Avoid Meaningless Placeholders**:
              * Don't use placeholder strings like "Name 1", "Test Value", "Description Here"
              * Keep test data focused on what matters for the specific test scenario
            
            - **Avoid Redundant Tests**:
              * Don't create separate tests ONLY to verify mock interactions if they're already verified in the happy path
              * Create separate tests for: exceptions, empty results, edge cases, different logical scenarios
              * Don't create separate tests for: re-verifying the same mock calls, testing the same code path with trivial variations
            
            - **Code preservation**: If an existing test file exists, update it according to 'Code Preservation Requirements' below.
            - **Attributes**: Never add your own [IntentManaged] attributes to tests.

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
                private readonly Mock<IServiceDependency> _serviceMock;
                private readonly Mock<IEventBus> _eventBusMock;
                private readonly {HandlerName} _handler;

                public {HandlerName}Tests()
                {
                    _serviceMock = new Mock<IServiceDependency>();
                    _eventBusMock = new Mock<IEventBus>();
                    _handler = new {HandlerName}(_serviceMock.Object, _eventBusMock.Object);
                }

                // Test methods follow...
            }
            ```

            ## Test Examples (Follow These Patterns)

            ### Example 1: Basic Event Handler with Service Call
            ```csharp
            // Handler implementation:
            // public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
            // {
            //     private readonly IOrderService _orderService;
            //     public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
            //     {
            //         await _orderService.ProcessOrder(message.OrderId, message.CustomerId, cancellationToken);
            //     }
            // }

            [Fact]
            public async Task Handle_CallsOrderService_WithCorrectParameters()
            {
                // Arrange
                var message = new OrderCreatedEvent
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid()
                };

                _orderServiceMock
                    .Setup(x => x.ProcessOrder(message.OrderId, message.CustomerId, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                _orderServiceMock.Verify(
                    x => x.ProcessOrder(message.OrderId, message.CustomerId, It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            [Fact]
            public async Task Handle_PropagatesException_WhenServiceFails()
            {
                // Arrange
                var message = new OrderCreatedEvent
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid()
                };

                _orderServiceMock
                    .Setup(x => x.ProcessOrder(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Service error"));

                // Act & Assert
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => _handler.HandleAsync(message, CancellationToken.None));
            }
            ```

            ### Example 2: Event Handler with Event Bus Operations
            ```csharp
            // Handler implementation:
            // public class ClientCreatedEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
            // {
            //     private readonly IReceiveService _receiveService;
            //     private readonly IEventBus _eventBus;
            //     public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken)
            //     {
            //         await _receiveService.ReceiveClient(message.Name, message.Location, message.IsActive, cancellationToken);
            //         _eventBus.Publish(new ClientProcessedEvent { Name = message.Name });
            //         _eventBus.Send(new NotifyTeamCommand { ClientName = message.Name });
            //     }
            // }

            [Fact]
            public async Task Handle_ProcessesMessageSuccessfully()
            {
                // Arrange
                var message = new ClientCreatedEvent
                {
                    Name = "Acme Corp",
                    Location = "New York",
                    IsActive = true
                };

                _receiveServiceMock
                    .Setup(x => x.ReceiveClient(message.Name, message.Location, message.IsActive, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                _receiveServiceMock.Verify(
                    x => x.ReceiveClient(message.Name, message.Location, message.IsActive, It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            [Fact]
            public async Task Handle_PublishesEvent_WithCorrectData()
            {
                // Arrange
                var message = new ClientCreatedEvent
                {
                    Name = "Acme Corp",
                    Location = "New York",
                    IsActive = true
                };

                ClientProcessedEvent? capturedEvent = null;
                _eventBusMock
                    .Setup(x => x.Publish(It.IsAny<ClientProcessedEvent>()))
                    .Callback<ClientProcessedEvent>(e => capturedEvent = e);

                _receiveServiceMock
                    .Setup(x => x.ReceiveClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                Assert.NotNull(capturedEvent);
                Assert.Equal(message.Name, capturedEvent.Name);
                _eventBusMock.Verify(x => x.Publish(It.IsAny<ClientProcessedEvent>()), Times.Once);
            }

            [Fact]
            public async Task Handle_SendsCommand_WithCorrectData()
            {
                // Arrange
                var message = new ClientCreatedEvent
                {
                    Name = "Acme Corp",
                    Location = "New York",
                    IsActive = true
                };

                NotifyTeamCommand? capturedCommand = null;
                _eventBusMock
                    .Setup(x => x.Send(It.IsAny<NotifyTeamCommand>()))
                    .Callback<NotifyTeamCommand>(c => capturedCommand = c);

                _receiveServiceMock
                    .Setup(x => x.ReceiveClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                Assert.NotNull(capturedCommand);
                Assert.Equal(message.Name, capturedCommand.ClientName);
                _eventBusMock.Verify(x => x.Send(It.IsAny<NotifyTeamCommand>()), Times.Once);
            }

            [Fact]
            public async Task Handle_PerformsAllOperations_InCorrectOrder()
            {
                // Arrange
                var message = new ClientCreatedEvent
                {
                    Name = "Acme Corp",
                    Location = "New York",
                    IsActive = true
                };

                var callOrder = new List<string>();

                _receiveServiceMock
                    .Setup(x => x.ReceiveClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                    .Callback(() => callOrder.Add("ReceiveClient"))
                    .Returns(Task.CompletedTask);

                _eventBusMock
                    .Setup(x => x.Publish(It.IsAny<ClientProcessedEvent>()))
                    .Callback(() => callOrder.Add("Publish"));

                _eventBusMock
                    .Setup(x => x.Send(It.IsAny<NotifyTeamCommand>()))
                    .Callback(() => callOrder.Add("Send"));

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                Assert.Equal(3, callOrder.Count);
                Assert.Equal("ReceiveClient", callOrder[0]);
                Assert.Equal("Publish", callOrder[1]);
                Assert.Equal("Send", callOrder[2]);
            }
            ```

            ### Example 3: Edge Cases and Negative Testing
            ```csharp
            [Fact]
            public async Task Handle_HandlesEmptyStringProperties()
            {
                // Arrange
                var message = new ClientCreatedEvent
                {
                    Name = "",
                    Location = "",
                    IsActive = false
                };

                _receiveServiceMock
                    .Setup(x => x.ReceiveClient("", "", false, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                _receiveServiceMock.Verify(
                    x => x.ReceiveClient("", "", false, It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            [Fact]
            public async Task Handle_DoesNotPublishOrSend_WhenServiceThrowsException()
            {
                // Arrange
                var message = new ClientCreatedEvent
                {
                    Name = "Acme Corp",
                    Location = "New York",
                    IsActive = true
                };

                _receiveServiceMock
                    .Setup(x => x.ReceiveClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Service error"));

                // Act & Assert
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => _handler.HandleAsync(message, CancellationToken.None));

                // Verify event bus operations were never called
                _eventBusMock.Verify(x => x.Publish(It.IsAny<ClientProcessedEvent>()), Times.Never);
                _eventBusMock.Verify(x => x.Send(It.IsAny<NotifyTeamCommand>()), Times.Never);
            }
            ```

            ### Example 4: Handler with Repository Access
            ```csharp
            // Handler implementation:
            // public class ProductUpdatedEventHandler : IIntegrationEventHandler<ProductUpdatedEvent>
            // {
            //     private readonly IProductRepository _productRepository;
            //     public async Task HandleAsync(ProductUpdatedEvent message, CancellationToken cancellationToken)
            //     {
            //         var product = await _productRepository.FindByIdAsync(message.ProductId, cancellationToken);
            //         if (product == null) throw new NotFoundException($"Product {message.ProductId} not found");
            //         product.UpdatePrice(message.NewPrice);
            //         await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            //     }
            // }

            [Fact]
            public async Task Handle_UpdatesProduct_WhenProductExists()
            {
                // Arrange
                var productId = Guid.NewGuid();
                var message = new ProductUpdatedEvent
                {
                    ProductId = productId,
                    NewPrice = 99.99m
                };

                var product = new Product { Id = productId };

                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(product);

                _productRepositoryMock
                    .Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                // Act
                await _handler.HandleAsync(message, CancellationToken.None);

                // Assert
                _productRepositoryMock.Verify(x => x.FindByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
                _productRepositoryMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task Handle_ThrowsNotFoundException_WhenProductNotFound()
            {
                // Arrange
                var message = new ProductUpdatedEvent
                {
                    ProductId = Guid.NewGuid(),
                    NewPrice = 99.99m
                };

                _productRepositoryMock
                    .Setup(x => x.FindByIdAsync(message.ProductId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Product?)null);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(
                    () => _handler.HandleAsync(message, CancellationToken.None));

                _productRepositoryMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            }
            ```

            {{$previousError}}
            """;

        return promptTemplate;
    }

    private List<ICodebaseFile> GetInputFiles(string applicationId, IElement eventHandlerElement)
    {
        var filesProvider = _applicationConfigurationProvider.GeneratedFilesProvider();

        // PRIMARY: Event handler implementation
        var inputFiles = filesProvider.GetFilesForMetadata(eventHandlerElement).ToList();

        // Get the event handler content for dependency scanning
        var handlerFiles = filesProvider.GetFilesForMetadata(eventHandlerElement);
        var handlerContent = string.Join("\n", handlerFiles.Select(f => f.Content));

        // PRIMARY: Integration event message (the incoming message type)
        if (eventHandlerElement.TypeReference?.ElementId != null)
        {
            inputFiles.AddRange(filesProvider.GetFilesForMetadata(eventHandlerElement.TypeReference.Element));
        }

        // INFRASTRUCTURE: IIntegrationEventHandler<T> interface
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Eventing.Contracts.IntegrationEventHandlerInterface"));

        // DEPENDENCIES: Scan handler content for injected services and event bus
        var detectedDependencies = DetectDependencies(handlerContent);

        // Include IEventBus if detected
        if (detectedDependencies.UsesEventBus)
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.Eventing.Contracts.EventBusInterface"));
        }

        // Include Repository interface if handler uses it
        if (detectedDependencies.UsesRepository)
        {
            inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface"));
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

        // MESSAGES: Include any additional messages that are published or sent
        var publishedMessageTypes = DetectPublishedMessageTypes(handlerContent);
        foreach (var messageTypeName in publishedMessageTypes)
        {
            // Try to find the message by scanning for matching elements in Services metadata
            var servicesDesigner = _metadataManager.GetDesigner(applicationId, "Services");
            var allElements = servicesDesigner.GetElementsOfType(SpecializationTypeIds.Service)
                .Concat(servicesDesigner.GetElementsOfType(SpecializationTypeIds.Command))
                .Concat(servicesDesigner.GetElementsOfType(SpecializationTypeIds.Query))
                .ToList();
            var messageElement = allElements.FirstOrDefault(e => e.Name.Equals(messageTypeName, StringComparison.Ordinal));
            if (messageElement != null)
            {
                inputFiles.AddRange(filesProvider.GetFilesForMetadata(messageElement));
            }
        }

        // Solution file needed for test project path discovery
        inputFiles.AddRange(filesProvider.GetFilesForTemplate("Intent.VisualStudio.Projects.VisualStudioSolution"));

        return inputFiles;
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