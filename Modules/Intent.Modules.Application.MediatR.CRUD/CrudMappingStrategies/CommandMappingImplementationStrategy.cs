using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class CommandMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;


        public CommandMappingImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;
        }

        public bool IsMatch()
        {
            return _template.Model.CreateEntityActions().Any() || _template.Model.UpdateEntityActions().Any() || _template.Model.DeleteEntityActions().Any();
        }

        public void ApplyStrategy()
        {

            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var @class = _template.CSharpFile.Classes.First();
            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            var csharpMapping = new CSharpClassMappingManager(_template); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
            var domainInteractionManager = new DomainInteractionsManager(_template, csharpMapping);

            csharpMapping.SetFromReplacement(_template.Model, "request");
            _template.CSharpFile.AddMetadata("mapping-manager", csharpMapping);
            
            foreach (var createAction in _template.Model.CreateEntityActions())
            {
                handleMethod.AddStatements(domainInteractionManager.CreateEntity(createAction));
            }

            foreach (var updateAction in _template.Model.UpdateEntityActions())
            {
                var entity = updateAction.Element.AsClassModel() ?? updateAction.Element.AsOperationModel().ParentClass;

                handleMethod.AddStatements(domainInteractionManager.QueryEntity(entity, updateAction.InternalAssociationEnd));

                handleMethod.AddStatement(string.Empty);
                handleMethod.AddStatements(domainInteractionManager.UpdateEntity(updateAction));
            }

            foreach (var deleteAction in _template.Model.DeleteEntityActions())
            {
                var foundEntity = deleteAction.Element.AsClassModel();
                handleMethod.AddStatements(domainInteractionManager.QueryEntity(foundEntity, deleteAction.InternalAssociationEnd));
                handleMethod.AddStatements(domainInteractionManager.DeleteEntity(deleteAction));
            }

            foreach (var entity in domainInteractionManager.TrackedEntities.Values.Where(x => x.IsNew))
            {
                handleMethod.AddStatement($"{entity.RepositoryFieldName}.Add({entity.VariableName});");
            }

            if (_template.Model.TypeReference.Element != null)
            {
                var returnStatement = domainInteractionManager.GetReturnStatements(_template.Model.TypeReference);
                handleMethod.AddStatements(returnStatement);
            }
        }
    }
}