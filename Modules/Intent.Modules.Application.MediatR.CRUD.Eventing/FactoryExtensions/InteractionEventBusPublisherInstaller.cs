using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InteractionEventBusPublisherInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.CRUD.Eventing.EventBusPublisherInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Application.Command.Handler");
            foreach (var template in templates)
            {
                var model = (template as ITemplateWithModel)?.Model as CommandModel ?? throw new Exception("Unable to resolve CommandModel for Application.Command.Handler template");
                if (!model.PublishedIntegrationEvents().Any())
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    AddPublishingLogic(template, file.Classes.First().FindMethod("Handle"), model.PublishedIntegrationEvents());
                });
            }

            templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Application.DomainEventHandler.Explicit");
            foreach (var template in templates)
            {
                var model = (template as ITemplateWithModel)?.Model as DomainEventHandlerModel ?? throw new Exception("Unable to resolve CommandModel for Application.Command.Handler template");
                if (!model.HandledDomainEvents().Any(x => x.PublishedIntegrationEvents().Any()))
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    foreach (var handledDomainEvent in model.HandledDomainEvents())
                    {
                        AddPublishingLogic(template, file.GetReferenceForModel(handledDomainEvent) as CSharpClassMethod, handledDomainEvent.PublishedIntegrationEvents());
                    }
                });
            }
        }

        private static void AddPublishingLogic(ICSharpFileBuilderTemplate template, CSharpClassMethod method, IEnumerable<PublishIntegrationEventTargetEndModel> publishes)
        {
            var constructor = method.Class.Constructors.First();
            if (constructor.Parameters.All(x => x.Type != template.GetTypeName(EventBusInterfaceTemplate.TemplateId)))
            {
                constructor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", ctor => ctor.IntroduceReadonlyField());
            }

            if (!method.TryGetMetadata<CSharpClassMappingManager>("mapping-manager", out var csharpMapping))
            {
                csharpMapping = new CSharpClassMappingManager(template);
            }

            csharpMapping.AddMappingResolver(new MessageCreationMappingTypeResolver(template));

            template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            foreach (var publish in publishes)
            {
                var mapping = publish.Mappings.SingleOrDefault();
                if (mapping == null)
                {
                    throw new ElementException(publish.InternalAssociationEnd, "Mapping not specified.");
                }
                csharpMapping.SetFromReplacement(publish.OtherEnd().TypeReference.Element, "request");
                var newMessageStatement = csharpMapping.GenerateCreationStatement(publish.Mappings.Single());
                AddPublishStatement(method, new CSharpInvocationStatement("_eventBus.Publish").AddArgument(newMessageStatement));
            }
        }


        private static void AddPublishStatement(CSharpClassMethod method, CSharpStatement publishStatement)
        {
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