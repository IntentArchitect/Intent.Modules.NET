using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping.Resolvers;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class CqrsHandlerCrudExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Application.MediatR.CRUD.CqrsHandlerCrudExtension";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        InstallOnCommandHandlers(application);
        InstallOnQueryHandlers(application);
    }

    private static void InstallOnCommandHandlers(IApplication application)
    {
        var templates = application.FindTemplateInstances<CommandHandlerTemplate>(CommandHandlerTemplate.TemplateId);
        foreach (var template in templates)
        {
            CSharpTemplateBase<CommandModel> targetTemplate =
                template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                    ? template.GetTemplate<CommandModelsTemplate>(CommandModelsTemplate.TemplateId, template.Model)!
                    : template;

            var legacyStrategy = StrategyFactory.GetMatchedCommandStrategy(targetTemplate);
            if (legacyStrategy is not null)
            {
                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ => legacyStrategy.ApplyStrategy());
            }

            var model = targetTemplate.Model;
            var interactions = model.GetInteractions().ToList();
            if (interactions.Count != 0)
            {
                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ =>
                {
                    var t = (ICSharpFileBuilderTemplate)targetTemplate;
                    t.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
                    t.AddTypeSource(TemplateRoles.Domain.ValueObject);
                    t.AddTypeSource(TemplateRoles.Domain.DataContract);
                    t.AddTypeSource(TemplateRoles.Domain.Entity
                        .Behaviour); // So that the mapping system can find the constructor when Separated State and Behaviours enabled in domain.

                    var @class = t.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
                    var handleMethod = @class.FindMethod("Handle")!;
                    if (legacyStrategy is null)
                    {
                        handleMethod.Statements.Clear();
                    }

                    handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

                    var csharpMapping = handleMethod.GetMappingManager();

                    if (model.GetStereotype("Http Settings")?.TryGetProperty("Verb", out var verb) == true && verb.Value == "PATCH")
                    {
                        csharpMapping.AddMappingResolver(new EntityPatchMappingTypeResolver(t));
                    }

                    csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new EnumCollectionMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(t));
                    csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(t));

                    csharpMapping.SetFromReplacement(model, "request");

                    handleMethod.ImplementInteractions(interactions);

                    if (legacyStrategy is null && model.TypeReference.Element != null)
                    {
                        var returnStatement = handleMethod.GetReturnStatements(model.TypeReference);
                        handleMethod.AddStatements(ExecutionPhases.Response, returnStatement);
                    }
                });
            }
        }
    }

    private static void InstallOnQueryHandlers(IApplication application)
    {
        var templates = application.FindTemplateInstances<QueryHandlerTemplate>(QueryHandlerTemplate.TemplateId);
        foreach (var template in templates)
        {
            CSharpTemplateBase<QueryModel> targetTemplate =
                template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                    ? template.GetTemplate<QueryModelsTemplate>(QueryModelsTemplate.TemplateId, template.Model)!
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
                if (model.TypeReference?.Element != null && model.TypeReference.Element.Name.Contains("PagedResult") &&
                    model.Properties.Any(x => x.Name.ToLower() == "orderby"))
                {
                    template.AddUsing("static System.Linq.Dynamic.Core.DynamicQueryableExtensions");
                    template.AddNugetDependency(SharedNuGetPackages.SystemLinqDynamicCore);
                }

                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ =>
                {
                    var t = (ICSharpFileBuilderTemplate)targetTemplate;
                    t.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
                    t.AddTypeSource(TemplateRoles.Domain.ValueObject);
                    t.AddTypeSource(TemplateRoles.Domain.DataContract);

                    var @class = t.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
                    var handleMethod = @class.FindMethod("Handle")!;

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

                    if (model.TypeReference?.Element != null
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