using System;
using System.Linq;
using Intent.AI;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.UnitTesting.Settings;
using Intent.Modules.UnitTesting.Templates;
using Intent.Modules.UnitTesting.Templates.CommandHandlerTest;
using Intent.Modules.UnitTesting.Templates.DomainEventHandlerTest;
using Intent.Modules.UnitTesting.Templates.DomainServiceTest;
using Intent.Modules.UnitTesting.Templates.IntegrationEventHandlerTest;
using Intent.Modules.UnitTesting.Templates.QueryHandlerTest;
using Intent.Modules.UnitTesting.Templates.ServiceOperationTest;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.UnitTesting.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UnitTestingAITasksFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.UnitTesting.UnitTestingAITasksFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            RegisterCommandHandlerTestAITasks(application);
            RegisterQueryHandlerTestAITasks(application);
            RegisterServiceOperationTestAITasks(application);
            RegisterIntegrationEventHandlerTestAITasks(application);
            RegisterDomainServiceTestAITasks(application);
            RegisterDomainEventHandlerTestAITasks(application);
        }

        private static void RegisterCommandHandlerTestAITasks(IApplication application)
        {
            var commandHandlerTestTemplates = application
                .FindTemplateInstances(CommandHandlerTestTemplate.TemplateId, _ => true)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var commandHandlerTestTemplate in commandHandlerTestTemplates)
            {
                if (!commandHandlerTestTemplate.TryGetModel(out IElement model))
                {
                    continue;
                }

                var handlerTemplate = application
                    .FindTemplateInstances(TemplateRoles.Application.Handler.Command, model)
                    .FirstOrDefault(t => t.CanRunTemplate());

                if (handlerTemplate == null)
                {
                    continue;
                }

                application.AITaskManager.RegisterTaskProvider(new UnitTestAITaskProvider((changes, outputFiles) =>
                {
                    if (changes.All(x => x.Template?.Equals(commandHandlerTestTemplate) != true && x.Template?.Equals(handlerTemplate) != true))
                    {
                        return null;
                    }

                    var mockFramework = application.Settings.GetUnitTestSettings().MockFramework().AsEnum();
                    var mockFrameworkName = UnitTestMockHelpers.GetMockFrameworkName(mockFramework);
                    var mockContext = UnitTestMockHelpers.GetMockFrameworkContext(mockFramework);

                    return new UnitTestAITask((IIntentTemplate)commandHandlerTestTemplate, handlerTemplate)
                    {
                        Type = "Implement Command Handler Unit Tests",
                        Title = $"Implement Unit Tests: {commandHandlerTestTemplate.ClassName}",
                        Instructions =
                            $"Implement comprehensive unit tests for the {model.Name}Handler command handler in the {commandHandlerTestTemplate.ClassName} class using {mockFrameworkName}.",
                        Context =
                            $"""
                              ## Mock Framework
                              Use **{mockFrameworkName}** for all mocking. Do NOT use any other mock framework.

                              {mockContext}
                              """
                    };
                }));
            }
        }

        private static void RegisterQueryHandlerTestAITasks(IApplication application)
        {
            var queryHandlerTestTemplates = application
                .FindTemplateInstances(QueryHandlerTestTemplate.TemplateId, _ => true)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var queryHandlerTestTemplate in queryHandlerTestTemplates)
            {
                if (!queryHandlerTestTemplate.TryGetModel(out IElement model))
                {
                    continue;
                }

                var handlerTemplate = application
                    .FindTemplateInstances(TemplateRoles.Application.Handler.Query, model)
                    .FirstOrDefault(t => t.CanRunTemplate());

                if (handlerTemplate == null)
                {
                    continue;
                }

                application.AITaskManager.RegisterTaskProvider(new UnitTestAITaskProvider((changes, outputFiles) =>
                {
                    if (changes.All(x => x.Template?.Equals(queryHandlerTestTemplate) != true && x.Template?.Equals(handlerTemplate) != true))
                    {
                        return null;
                    }

                    var mockFramework = application.Settings.GetUnitTestSettings().MockFramework().AsEnum();
                    var mockFrameworkName = UnitTestMockHelpers.GetMockFrameworkName(mockFramework);
                    var mockContext = UnitTestMockHelpers.GetMockFrameworkContext(mockFramework);

                    return new UnitTestAITask((IIntentTemplate)queryHandlerTestTemplate, handlerTemplate)
                    {
                        Type = "Implement Query Handler Unit Tests",
                        Title = $"Implement Unit Tests: {queryHandlerTestTemplate.ClassName}",
                        Instructions =
                            $"Implement comprehensive unit tests for the {model.Name}Handler query handler in the {queryHandlerTestTemplate.ClassName} class using {mockFrameworkName}.",
                        Context =
                            $"""
                              ## Mock Framework
                              Use **{mockFrameworkName}** for all mocking. Do NOT use any other mock framework.

                              {mockContext}
                              """
                    };
                }));
            }
        }

        private static void RegisterServiceOperationTestAITasks(IApplication application)
        {
            var serviceOperationTestTemplates = application
                .FindTemplateInstances(ServiceOperationTestTemplate.TemplateId, _ => true)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var serviceOperationTestTemplate in serviceOperationTestTemplates)
            {
                if (!serviceOperationTestTemplate.TryGetModel(out IElement model))
                {
                    continue;
                }

                var serviceTemplate = application
                    .FindTemplateInstances("Intent.Application.ServiceImplementations.ServiceImplementation", model)
                    .FirstOrDefault(t => t.CanRunTemplate());

                if (serviceTemplate == null)
                {
                    continue;
                }

                application.AITaskManager.RegisterTaskProvider(new UnitTestAITaskProvider((changes, outputFiles) =>
                {
                    if (changes.All(x => x.Template?.Equals(serviceOperationTestTemplate) != true && x.Template?.Equals(serviceTemplate) != true))
                    {
                        return null;
                    }

                    var mockFramework = application.Settings.GetUnitTestSettings().MockFramework().AsEnum();
                    var mockFrameworkName = UnitTestMockHelpers.GetMockFrameworkName(mockFramework);
                    var mockContext = UnitTestMockHelpers.GetMockFrameworkContext(mockFramework);

                    return new UnitTestAITask((IIntentTemplate)serviceOperationTestTemplate, serviceTemplate)
                    {
                        Type = "Implement Service Operation Unit Tests",
                        Title = $"Implement Unit Tests: {serviceOperationTestTemplate.ClassName}",
                        Instructions =
                            $"Implement comprehensive unit tests for the {model.Name} service in the {serviceOperationTestTemplate.ClassName} class using {mockFrameworkName}.",
                        Context =
                            $"""
                              ## Mock Framework
                              Use **{mockFrameworkName}** for all mocking. Do NOT use any other mock framework.

                              {mockContext}
                              """
                    };
                }));
            }
        }

        private static void RegisterIntegrationEventHandlerTestAITasks(IApplication application)
        {
            var integrationEventHandlerTestTemplates = application
                .FindTemplateInstances(IntegrationEventHandlerTestTemplate.TemplateId, _ => true)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var integrationEventHandlerTestTemplate in integrationEventHandlerTestTemplates)
            {
                if (!integrationEventHandlerTestTemplate.TryGetModel(out IElement model))
                {
                    continue;
                }

                var handlerTemplate = application
                    .FindTemplateInstances(TemplateRoles.Application.Eventing.EventHandler, model)
                    .FirstOrDefault(t => t.CanRunTemplate());

                if (handlerTemplate == null)
                {
                    continue;
                }

                application.AITaskManager.RegisterTaskProvider(new UnitTestAITaskProvider((changes, outputFiles) =>
                {
                    if (changes.All(x => x.Template?.Equals(integrationEventHandlerTestTemplate) != true && x.Template?.Equals(handlerTemplate) != true))
                    {
                        return null;
                    }

                    var mockFramework = application.Settings.GetUnitTestSettings().MockFramework().AsEnum();
                    var mockFrameworkName = UnitTestMockHelpers.GetMockFrameworkName(mockFramework);
                    var mockContext = UnitTestMockHelpers.GetMockFrameworkContext(mockFramework);

                    return new UnitTestAITask((IIntentTemplate)integrationEventHandlerTestTemplate, handlerTemplate)
                    {
                        Type = "Implement Integration Event Handler Unit Tests",
                        Title = $"Implement Unit Tests: {integrationEventHandlerTestTemplate.ClassName}",
                        Instructions =
                            $"Implement comprehensive unit tests for the {model.Name} integration event handler in the {integrationEventHandlerTestTemplate.ClassName} class using {mockFrameworkName}.",
                        Context =
                            $"""
                              ## Mock Framework
                              Use **{mockFrameworkName}** for all mocking. Do NOT use any other mock framework.

                              {mockContext}
                              """
                    };
                }));
            }
        }

        private static void RegisterDomainServiceTestAITasks(IApplication application)
        {
            var domainServiceTestTemplates = application
                .FindTemplateInstances(DomainServiceTestTemplate.TemplateId, _ => true)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var domainServiceTestTemplate in domainServiceTestTemplates)
            {
                if (!domainServiceTestTemplate.TryGetModel(out IElement model))
                {
                    continue;
                }

                var serviceTemplate = application
                    .FindTemplateInstances(TemplateRoles.Domain.DomainServices.Implementation, model)
                    .FirstOrDefault(t => t.CanRunTemplate());

                if (serviceTemplate == null)
                {
                    continue;
                }

                application.AITaskManager.RegisterTaskProvider(new UnitTestAITaskProvider((changes, outputFiles) =>
                {
                    if (changes.All(x => x.Template?.Equals(domainServiceTestTemplate) != true && x.Template?.Equals(serviceTemplate) != true))
                    {
                        return null;
                    }

                    var mockFramework = application.Settings.GetUnitTestSettings().MockFramework().AsEnum();
                    var mockFrameworkName = UnitTestMockHelpers.GetMockFrameworkName(mockFramework);
                    var mockContext = UnitTestMockHelpers.GetMockFrameworkContext(mockFramework);

                    return new UnitTestAITask((IIntentTemplate)domainServiceTestTemplate, serviceTemplate)
                    {
                        Type = "Implement Domain Service Unit Tests",
                        Title = $"Implement Unit Tests: {domainServiceTestTemplate.ClassName}",
                        Instructions =
                            $"Implement comprehensive unit tests for the {model.Name} domain service in the {domainServiceTestTemplate.ClassName} class using {mockFrameworkName}.",
                        Context =
                            $"""
                              ## Mock Framework
                              Use **{mockFrameworkName}** for all mocking. Do NOT use any other mock framework.

                              {mockContext}
                              """
                    };
                }));
            }
        }

        private static void RegisterDomainEventHandlerTestAITasks(IApplication application)
        {
            var domainEventHandlerTestTemplates = application
                .FindTemplateInstances(DomainEventHandlerTestTemplate.TemplateId, _ => true)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var domainEventHandlerTestTemplate in domainEventHandlerTestTemplates)
            {
                if (!domainEventHandlerTestTemplate.TryGetModel(out IElement model))
                {
                    continue;
                }

                var handlerTemplate = application
                    .FindTemplateInstances("Application.DomainEventHandler.Explicit", model)
                    .FirstOrDefault(t => t.CanRunTemplate());

                if (handlerTemplate == null)
                {
                    continue;
                }

                application.AITaskManager.RegisterTaskProvider(new UnitTestAITaskProvider((changes, outputFiles) =>
                {
                    if (changes.All(x => x.Template?.Equals(domainEventHandlerTestTemplate) != true && x.Template?.Equals(handlerTemplate) != true))
                    {
                        return null;
                    }

                    var mockFramework = application.Settings.GetUnitTestSettings().MockFramework().AsEnum();
                    var mockFrameworkName = UnitTestMockHelpers.GetMockFrameworkName(mockFramework);
                    var mockContext = UnitTestMockHelpers.GetMockFrameworkContext(mockFramework);

                    return new UnitTestAITask((IIntentTemplate)domainEventHandlerTestTemplate, handlerTemplate)
                    {
                        Type = "Implement Domain Event Handler Unit Tests",
                        Title = $"Implement Unit Tests: {domainEventHandlerTestTemplate.ClassName}",
                        Instructions =
                            $"Implement comprehensive unit tests for the {model.Name} domain event handler in the {domainEventHandlerTestTemplate.ClassName} class using {mockFrameworkName}.",
                        Context =
                            $"""
                              ## Mock Framework
                              Use **{mockFrameworkName}** for all mocking. Do NOT use any other mock framework.

                              {mockContext}
                              """
                    };
                }));
            }
        }
    }
}