using System;
using System.Linq;
using System.Numerics;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
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
            if (_model.TypeReference?.Element != null && _model.TypeReference.Element.Name.Contains("PagedResult") && _model.Properties.Any(x => x.Name.ToLower() == "orderby"))
            {
                _template.AddUsing("static System.Linq.Dynamic.Core.DynamicQueryableExtensions");
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
            var @class = _template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
            var handleMethod = @class.FindMethod("Handle");
            var csharpMapping = handleMethod.GetMappingManager();
            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
			csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(_template));
			csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(_template));

            csharpMapping.SetFromReplacement(_model, "request");


            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddTypeSource(TemplateRoles.Domain.DataContract);

            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            handleMethod.ImplementInteractions(_model);

            if (_model.TypeReference.Element != null)
            {
                handleMethod.AddStatements(ExecutionPhases.Response, handleMethod.GetReturnStatements(_model.TypeReference));
            }
        }
    }
}