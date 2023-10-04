using System;
using System.Linq;
using System.Numerics;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class QueryMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly QueryHandlerTemplate _template;


        public QueryMappingImplementationStrategy(QueryHandlerTemplate template)
        {
            _template = template;
        }

        public bool IsMatch()
        {
            return _template.Model.QueryEntityActions().Any(x => x.Mappings.GetQueryEntityMapping() != null);
        }

        public void ApplyStrategy()
        {
            var csharpMapping = new CSharpClassMappingManager(_template); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
            var domainInteractionManager = new DomainInteractionsManager(_template, csharpMapping);

            csharpMapping.SetFromReplacement(_template.Model, "request");
            _template.CSharpFile.AddMetadata("mapping-manager", csharpMapping);


            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var @class = _template.CSharpFile.Classes.First();
            var handleMethod = @class.FindMethod("Handle");

            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();


            foreach (var queryAction in _template.Model.QueryEntityActions())
            {
                var foundEntity = queryAction.Element.AsClassModel();
                if (foundEntity != null && queryAction.Mappings.GetQueryEntityMapping() != null)
                {
                    handleMethod.AddStatements(domainInteractionManager.QueryEntity(foundEntity, queryAction.InternalAssociationEnd));
                }
            }

            if (_template.Model.TypeReference.Element != null && domainInteractionManager.TrackedEntities.Any())
            {
                handleMethod.AddStatements(domainInteractionManager.GetReturnStatements(_template.Model.TypeReference));
            }
        }
    }
}