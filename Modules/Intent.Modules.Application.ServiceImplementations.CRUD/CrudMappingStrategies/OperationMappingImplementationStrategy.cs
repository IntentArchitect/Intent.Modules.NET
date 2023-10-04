using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Constants;
using Intent.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.CrudMappingStrategies
{
    public class OperationMappingImplementationStrategy : IImplementationStrategy
    {
        private readonly ServiceImplementationTemplate _template;
        private readonly DomainInteractionsManager _domainInteractionManager;
        private readonly CSharpClassMappingManager _csharpMapping;

        public OperationMappingImplementationStrategy(ServiceImplementationTemplate template)
        {
            _template = template;
        }

        public bool IsMatch(OperationModel operationModel)
        {
            return operationModel.CreateEntityActions().Any() 
                   || operationModel.UpdateEntityActions().Any() 
                   || operationModel.DeleteEntityActions().Any() 
                   || operationModel.QueryEntityActions().Any();
        }

        public void ApplyStrategy(OperationModel operationModel)
        {
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");
            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.Name.Equals(operationModel.Name, StringComparison.OrdinalIgnoreCase));
            method.Statements.Clear();
            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            var csharpMapping = new CSharpClassMappingManager(_template); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
            var domainInteractionManager = new DomainInteractionsManager(_template, csharpMapping);

            csharpMapping.SetFromReplacement(operationModel, null); // Ignore the method itself
            method.AddMetadata("mapping-manager", csharpMapping);

            foreach (var queryAction in operationModel.QueryEntityActions())
            {
                var entity = queryAction.Element.AsClassModel();
                method.AddStatements(domainInteractionManager.QueryEntity(entity, queryAction.InternalAssociationEnd));
            }

            foreach (var createAction in operationModel.CreateEntityActions())
            {
                method.AddStatements(domainInteractionManager.CreateEntity(createAction));
            }

            foreach (var updateAction in operationModel.UpdateEntityActions())
            {
                var entity = updateAction.Element.AsClassModel() 
                             ?? updateAction.Element.AsOperationModel()?.ParentClass
                             ?? throw new ElementException(updateAction.InternalAssociationEnd, "Target could not be cast to a Domain Class or Operation");

                method.AddStatements(domainInteractionManager.QueryEntity(entity, updateAction.InternalAssociationEnd));

                method.AddStatement(string.Empty);
                method.AddStatements(domainInteractionManager.UpdateEntity(updateAction));
            }

            foreach (var deleteAction in operationModel.DeleteEntityActions())
            {
                var foundEntity = deleteAction.Element.AsClassModel();
                method.AddStatements(domainInteractionManager.QueryEntity(foundEntity, deleteAction.InternalAssociationEnd));
                method.AddStatements(domainInteractionManager.DeleteEntity(deleteAction));
            }

            foreach (var entity in domainInteractionManager.TrackedEntities.Values.Where(x => x.IsNew))
            {
                method.AddStatement($"{entity.RepositoryFieldName}.Add({entity.VariableName});");
            }

            if (operationModel.TypeReference.Element != null)
            {
                var returnStatement = domainInteractionManager.GetReturnStatements(operationModel.TypeReference);
                method.AddStatements(returnStatement);
            }
        }
    }
}