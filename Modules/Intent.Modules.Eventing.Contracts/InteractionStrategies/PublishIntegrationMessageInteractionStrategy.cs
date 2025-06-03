using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Exceptions;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using Intent.Modules.Application.MediatR.CRUD.Eventing.MappingTypeResolvers;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;

namespace Intent.Modules.Application.Contracts.InteractionStrategies
{
    public class PublishIntegrationMessageInteractionStrategy : IInteractionStrategy
    {
        public bool IsMatch(IAssociationEnd interaction)
        {
            return interaction.IsPublishIntegrationEventTargetEndModel();
        }

        public IEnumerable<CSharpStatement> CreateInteractionStatements(CSharpClassMethod method, IAssociationEnd interaction)
        {
            var handlerClass = method.Class;
            var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;
            var @class = handlerClass;
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            AddEventBusDependency(template, method);


            var csharpMapping = method.GetMappingManager();
            csharpMapping.AddMappingResolver(new MessageCreationMappingTypeResolver(template));

            template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
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
                var eventName = template.GetTypeName(IntegrationEventMessageTemplate.TemplateId, interaction.TypeReference.Element.Id);
                newMessageStatement = new CSharpObjectInitializerBlock($"new {eventName}");
            }

            return [GetIntegrationDispatchStatement(method, new CSharpInvocationStatement("_eventBus.Publish").AddArgument(newMessageStatement))];
        }

        private static void AddEventBusDependency(ICSharpFileBuilderTemplate template, CSharpClassMethod method)
        {
            var constructor = method.Class.Constructors.First();
            if (constructor.Parameters.All(x => x.Type != template.GetTypeName(EventBusInterfaceTemplate.TemplateId)))
            {
                constructor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", ctor => ctor.IntroduceReadonlyField());
            }
        }

        private static CSharpStatement GetIntegrationDispatchStatement(CSharpClassMethod method, CSharpStatement publishStatement)
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

            //var returnClause = method.Statements.FirstOrDefault(p => p.GetText("").Trim().StartsWith("return"));
            //if (returnClause != null)
            //{
            //    returnClause.InsertAbove(publishStatement);
            //}
            //else
            //{
            //    method.Statements.Add(publishStatement);
            //}
            return publishStatement;
        }
    }


}

public class CommandQueryMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CommandQueryMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType == "Command" || mappingModel.Model.SpecializationType == "Query")
        {
            return new ConstructorMapping(mappingModel, _template);
        }
        //if (mappingModel.Model.TypeReference?.Element?.SpecializationType == "DTO")
        //{
        //    return new ObjectInitializationMapping(mappingModel, _template);
        //}
        //if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
        //    || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
        //{
        //    return new TypeConvertingCSharpMapping(mappingModel, _template);
        //}
        return null;
    }
}

public record StaticMetadata(string id) : IMetadataModel
{
    public string Id { get; } = id;
}
