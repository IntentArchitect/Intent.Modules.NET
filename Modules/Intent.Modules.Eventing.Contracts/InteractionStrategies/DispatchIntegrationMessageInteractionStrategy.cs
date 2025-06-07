using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies
{
    public class DispatchIntegrationMessageInteractionStrategy : IInteractionStrategy
    {
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsPublishIntegrationEventTargetEndModel() || interaction.IsSendIntegrationCommandTargetEndModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var interaction = (IAssociationEnd)interactionElement;
            var handlerClass = method.Class;
            var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;
            var @class = handlerClass;
            @class.InjectService(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus");

            var csharpMapping = method.GetMappingManager();
            csharpMapping.AddMappingResolver(new MessageCreationMappingTypeResolver(template));

            template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            template.AddTypeSource(IntegrationCommandTemplate.TemplateId);
            template.AddTypeSource(IntegrationEventDtoTemplate.TemplateId);
            template.AddTypeSource(IntegrationEventEnumTemplate.TemplateId);
            CSharpStatement newMessageStatement;
            var mapping = interaction.Mappings.SingleOrDefault();
            if (mapping != null)
            {
                var eventVariableName = method.Parameters.FirstOrDefault()?.Name ??
                                        throw new Exception("Expected at least one parameter on Integration Event Handler method.");

                csharpMapping.SetFromReplacement(interaction.OtherEnd().TypeReference.Element, eventVariableName);
                newMessageStatement = csharpMapping.GenerateCreationStatement(interaction.Mappings.Single());
            }
            else
            {
                var messageName = template.GetTypeName((IElement)interaction.TypeReference.Element);
                newMessageStatement = new CSharpObjectInitializerBlock($"new {messageName}");
            }

            if (interaction.IsPublishIntegrationEventTargetEndModel())
            {
                AddIntegrationDispatchStatements(method, new CSharpInvocationStatement("_eventBus.Publish").AddArgument(newMessageStatement));
            } else if (interaction.IsSendIntegrationCommandTargetEndModel())
            {
                AddIntegrationDispatchStatements(method, new CSharpInvocationStatement("_eventBus.Send").AddArgument(newMessageStatement));
            }
        }

        private static void AddIntegrationDispatchStatements(CSharpClassMethod method, CSharpStatement publishStatement)
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

            method.AddStatement(ExecutionPhases.IntegrationEvents, publishStatement);

            //var returnClause = method.Statements.FirstOrDefault(p => p.GetText("").Trim().StartsWith("return"));
            //if (returnClause != null)
            //{
            //    returnClause.InsertAbove(publishStatement);
            //}
            //else
            //{
            //    method.Statements.Add(publishStatement);
            //}
        }
    }
}