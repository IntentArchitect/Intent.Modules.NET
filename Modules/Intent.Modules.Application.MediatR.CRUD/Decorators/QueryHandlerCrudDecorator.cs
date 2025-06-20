using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class QueryHandlerCrudDecorator : QueryHandlerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.MediatR.CRUD.QueryHandlerCrudDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly QueryHandlerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public QueryHandlerCrudDecorator(QueryHandlerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            CSharpTemplateBase<QueryModel> targetTemplate = template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                ? template.GetTemplate<QueryModelsTemplate>(QueryModelsTemplate.TemplateId, template.Model)
                : template;

            var model = targetTemplate.Model;
            var legacyStrategy = StrategyFactory.GetMatchedQueryStrategy(targetTemplate, application);
            if (legacyStrategy is not null)
            {
                legacyStrategy.BindToTemplate((ICSharpFileBuilderTemplate)targetTemplate);
            }

            var interactions = model.GetInteractions().ToList();
            if (interactions.Any())
            {
                if (model.TypeReference?.Element != null && model.TypeReference.Element.Name.Contains("PagedResult") && model.Properties.Any(x => x.Name.ToLower() == "orderby"))
                {
                    _template.AddUsing("static System.Linq.Dynamic.Core.DynamicQueryableExtensions");
                    _template.AddNugetDependency(SharedNuGetPackages.SystemLinqDynamicCore);
                }

                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ =>
                {
                    var t = (ICSharpFileBuilderTemplate)targetTemplate;
                    t.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
                    t.AddTypeSource(TemplateRoles.Domain.ValueObject);
                    t.AddTypeSource(TemplateRoles.Domain.DataContract);

                    var @class = t.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
                    var handleMethod = @class.FindMethod("Handle");

                    if (legacyStrategy is null)
                    {
                        handleMethod.Statements.Clear();
                    }
                    handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

                    var csharpMapping = handleMethod.GetMappingManager();
                    csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(t));
                    csharpMapping.SetFromReplacement(model, "request");

                    handleMethod.ImplementInteractions(interactions);

                    if (model.TypeReference.Element != null
                        && legacyStrategy is null
                        && !handleMethod.GetStatementsInPhase(ExecutionPhases.Response).Any())
                    {
                        handleMethod.AddStatements(ExecutionPhases.Response, handleMethod.GetReturnStatements(model.TypeReference));
                    }
                });
            }
        }
    }
}