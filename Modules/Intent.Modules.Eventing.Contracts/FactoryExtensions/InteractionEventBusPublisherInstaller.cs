using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InteractionEventBusPublisherInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Contracts.InteractionEventBusPublisherInstaller";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // PUBLISH EVENTS FROM COMMAND:
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Application.Command.Handler");
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    foreach (var processingHandler in template.CSharpFile.GetProcessingHandlers())
                    {
                        if (processingHandler.Model.PublishedIntegrationEvents().Any())
                        {
                            AddPublishingLogic(template, processingHandler.Method, "request", processingHandler.Model.PublishedIntegrationEvents());
                        }

                        if (processingHandler.Model.SentIntegrationCommands().Any())
                        {
                            AddSendingLogic(template, processingHandler.Method, "request", processingHandler.Model.SentIntegrationCommands());
                        }
                    }
                });
            }

            // PUBLISH EVENTS FROM SERVICES:
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.Implementation);
            foreach (var template in templates)
            {
                if (!template.TryGetModel(out ServiceModel serviceModel))
                {
                    continue;
                }

                foreach (var model in serviceModel.Operations)
                {
                    if (model.PublishedIntegrationEvents().Any())
                    {
                        template.CSharpFile.AfterBuild(file =>
                        {
                            AddPublishingLogic(template, file.Classes.First().GetReferenceForModel(model) as CSharpClassMethod, null, model.PublishedIntegrationEvents());
                        });
                    }

                    if (model.SentIntegrationCommands().Any())
                    {
                        template.CSharpFile.AfterBuild(file =>
                        {
                            AddSendingLogic(template, file.Classes.First().GetReferenceForModel(model) as CSharpClassMethod, null, model.SentIntegrationCommands());
                        });
                    }
                }
            }

            // PUBLISH EVENTS FROM DOMAIN EVENT HANDLERS:
            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DomainEventHandler.Explicit);
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    foreach (var processingHandler in template.CSharpFile.GetProcessingHandlers())
                    {
                        if (processingHandler.Model.PublishedIntegrationEvents().Any())
                        {
                            var method = processingHandler.Method;
                            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
                            if (!method.Statements.Any(x => x.ToString().Equals("var domainEvent = notification.DomainEvent;")))
                            {
                                method.AddStatement("var domainEvent = notification.DomainEvent;");
                            }

                            AddPublishingLogic(template, method, "domainEvent", processingHandler.Model.PublishedIntegrationEvents());
                        }

                        if (processingHandler.Model.SentIntegrationCommands().Any())
                        {
                            var method = processingHandler.Method;
                            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
                            if (!method.Statements.Any(x => x.ToString().Equals("var domainEvent = notification.DomainEvent;")))
                            {
                                method.AddStatement("var domainEvent = notification.DomainEvent;");
                            }
                            AddSendingLogic(template, method, "domainEvent", processingHandler.Model.SentIntegrationCommands());
                        }
                    }
                });
            }

            // PUBLISH EVENTS FROM INTEGRATION EVENT HANDLERS:
            templates = application.FindTemplateInstances<ITemplate>(TemplateRoles.Application.Eventing.EventHandler)
                // Have to do this at the moment, otherwise SF break because it can't cast.
                .OfType<ICSharpFileBuilderTemplate>();
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    foreach (var processingHandler in template.CSharpFile.GetProcessingHandlers())
                    {
                        if (processingHandler.Model.PublishedIntegrationEvents().Any())
                        {
                            var method = processingHandler.Method;
                            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
                            var eventVariableName = method.Parameters.FirstOrDefault()?.Name ??
                                                    throw new Exception("Expected at least one parameter on Integration Event Handler method.");

                            AddPublishingLogic(template, method, eventVariableName, processingHandler.Model.PublishedIntegrationEvents());
                        }

                        if (processingHandler.Model.SentIntegrationCommands().Any())
                        {
                            var method = processingHandler.Method;
                            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
                            var eventVariableName = method.Parameters.FirstOrDefault()?.Name ??
                                                    throw new Exception("Expected at least one parameter on Integration Event Handler method.");
                            AddSendingLogic(template, method, eventVariableName, processingHandler.Model.SentIntegrationCommands());
                        }
                    }
                });
            }
        }

        private static void AddPublishingLogic(ICSharpFileBuilderTemplate template, CSharpClassMethod method, string replacement,
            IEnumerable<PublishIntegrationEventTargetEndModel> publishes)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            AddEventBusDependency(template, method);

            if (!method.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
            {
                csharpMapping = new CSharpClassMappingManager(template);
            }

            csharpMapping.AddMappingResolver(new MessageCreationMappingTypeResolver(template));

            template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            template.AddTypeSource(IntegrationEventDtoTemplate.TemplateId);
            template.AddTypeSource(IntegrationEventEnumTemplate.TemplateId);
            foreach (var publish in publishes)
            {
                CSharpStatement newMessageStatement;
                var mapping = publish.Mappings.SingleOrDefault();
                if (mapping != null)
                {
                    csharpMapping.SetFromReplacement(publish.OtherEnd().TypeReference.Element, replacement);
                    newMessageStatement = csharpMapping.GenerateCreationStatement(publish.Mappings.Single());
                }
                else
                {
                    var eventName = template.GetTypeName(IntegrationEventMessageTemplate.TemplateId, publish.Element.Id);
                    newMessageStatement = new CSharpObjectInitializerBlock($"new {eventName}");
                }

                AddIntegrationDispatchStatement(method, new CSharpInvocationStatement("_eventBus.Publish").AddArgument(newMessageStatement));
            }
        }

        private static void AddEventBusDependency(ICSharpFileBuilderTemplate template, CSharpClassMethod method)
        {
            var constructor = method.Class.Constructors.First();
            if (constructor.Parameters.All(x => x.Type != template.GetTypeName(EventBusInterfaceTemplate.TemplateId)))
            {
                constructor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", ctor => ctor.IntroduceReadonlyField());
            }
        }

        private static void AddSendingLogic(
            ICSharpFileBuilderTemplate template,
            CSharpClassMethod method,
            string replacement,
            IList<SendIntegrationCommandTargetEndModel> commandsToSend)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            AddEventBusDependency(template, method);

            if (!method.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
            {
                csharpMapping = new CSharpClassMappingManager(template);
            }

            csharpMapping.AddMappingResolver(new MessageCreationMappingTypeResolver(template));

            template.AddTypeSource(IntegrationCommandTemplate.TemplateId);
            template.AddTypeSource(IntegrationEventDtoTemplate.TemplateId);
            template.AddTypeSource(IntegrationEventEnumTemplate.TemplateId);

            foreach (var commandToSend in commandsToSend)
            {
                CSharpStatement newMessageStatement;
                var mapping = commandToSend.Mappings.SingleOrDefault();
                if (mapping != null)
                {
                    csharpMapping.SetFromReplacement(commandToSend.OtherEnd().TypeReference.Element, replacement);
                    newMessageStatement = csharpMapping.GenerateCreationStatement(commandToSend.Mappings.Single());
                }
                else
                {
                    var commandName = template.GetTypeName(IntegrationCommandTemplate.TemplateId, commandToSend.Element.Id);
                    newMessageStatement = new CSharpObjectInitializerBlock($"new {commandName}");
                }
                AddIntegrationDispatchStatement(method, new CSharpInvocationStatement("_eventBus.Send")
                    .AddArgument(newMessageStatement));
            }
        }

        private static void AddIntegrationDispatchStatement(CSharpClassMethod method, CSharpStatement publishStatement)
        {
            var notImplementedStatement = method.Statements.FirstOrDefault(p => p.GetText("").Contains("throw new NotImplementedException"));
            if (notImplementedStatement is not null)
            {
                notImplementedStatement.Remove();
                method.Attributes.OfType<CSharpIntentManagedAttribute>().FirstOrDefault()?.WithBodyFully();

                var toDoStatement = method.Statements.FirstOrDefault(p => p.GetText("").Contains("// TODO:"));
                if (toDoStatement is not null)
                {
                    toDoStatement.Remove();
                }
            }

            var returnClause = method.Statements.FirstOrDefault(p => p.GetText("").Trim().StartsWith("return"));
            if (returnClause != null)
            {
                returnClause.InsertAbove(publishStatement);
            }
            else
            {
                method.Statements.Add(publishStatement);
            }
        }
    }
}