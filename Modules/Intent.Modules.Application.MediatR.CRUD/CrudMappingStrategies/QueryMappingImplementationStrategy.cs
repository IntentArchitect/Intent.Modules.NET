using System;
using System.Linq;
using System.Numerics;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class QueryMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private readonly QueryModel _model;


        public QueryMappingImplementationStrategy(CSharpTemplateBase<QueryModel> template)
        {
            _template = (ICSharpFileBuilderTemplate)template;
            _model = template.Model;
        }

        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            _template.AddKnownType("System.Linq.Dynamic.Core.PagedResult");
            if (_model.TypeReference?.Element != null && _model.TypeReference.Element.Name.Contains("PagedResult") && _model.Properties.Any(x => x.Name.ToLower() == "orderby"))
            {
                _template.UseType("System.Linq.Dynamic.Core.OrderBy");
                _template.AddNugetDependency(SharedNuGetPackages.SystemLinqDynamicCore);
            }
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }


        public bool IsMatch()
        {
            return _model.HasDomainInteractions();
        }

        public void ApplyStrategy()
        {
            var csharpMapping = new CSharpClassMappingManager(_template); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
			csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(_template));
			csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(_template));
            var domainInteractionManager = new DomainInteractionsManager(_template, csharpMapping);

            csharpMapping.SetFromReplacement(_model, "request");


            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddTypeSource(TemplateRoles.Domain.DataContract);

            var @class = _template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
            var handleMethod = @class.FindMethod("Handle");
            handleMethod.AddMetadata("mapping-manager", csharpMapping);

            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            handleMethod.AddStatements(domainInteractionManager.CreateInteractionStatements(@class, _model));

            if (_model.TypeReference.Element != null)
            {
                handleMethod.AddStatements(domainInteractionManager.GetReturnStatements(@class, _model.TypeReference));
            }
        }
    }
}