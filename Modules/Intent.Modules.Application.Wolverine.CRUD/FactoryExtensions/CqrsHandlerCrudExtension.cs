using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.Wolverine.Templates.CommandHandler;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
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

namespace Intent.Modules.Application.Wolverine.CRUD.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class CqrsHandlerCrudExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Application.Wolverine.CRUD.CqrsHandlerCrudExtension";

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
            var model = template.Model;
            var interactions = model.GetInteractions().ToList();
            if (interactions.Count == 0)
            {
                continue;
            }

            template.CSharpFile.AfterBuild(_ =>
            {
                template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
                template.AddTypeSource(TemplateRoles.Domain.ValueObject);
                template.AddTypeSource(TemplateRoles.Domain.DataContract);
                template.AddTypeSource(TemplateRoles.Domain.Entity.Behaviour);

                var @class = template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
                var handleMethod = @class.FindMethod("Handle")!;
                handleMethod.Statements.Clear();
                handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

                var csharpMapping = handleMethod.GetMappingManager();
                csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new EnumCollectionMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(template));
                csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(template));

                csharpMapping.SetFromReplacement(model, "command");

                handleMethod.ImplementInteractions(interactions);

                if (model.TypeReference.Element != null)
                {
                    handleMethod.AddStatements(ExecutionPhases.Response, handleMethod.GetReturnStatements(model.TypeReference));
                }
            });
        }
    }

    private static void InstallOnQueryHandlers(IApplication application)
    {
        var templates = application.FindTemplateInstances<QueryHandlerTemplate>(QueryHandlerTemplate.TemplateId);
        foreach (var template in templates)
        {
            var model = template.Model;
            var interactions = model.GetInteractions().ToList();
            if (interactions.Count == 0)
            {
                // No modelled interactions: fall back to convention-based "get all" generation
                // (collection of a domain-mapped DTO), mirroring the MediatR CRUD module.
                CrudStrategies.ConventionGetAllStrategy.TryApply(template);
                continue;
            }

            if (model.TypeReference?.Element != null &&
                model.TypeReference.Element.Name.Contains("PagedResult") &&
                model.Properties.Any(x => x.Name.ToLower() == "orderby"))
            {
                template.AddUsing("static System.Linq.Dynamic.Core.DynamicQueryableExtensions");
                template.AddNugetDependency(SharedNuGetPackages.SystemLinqDynamicCore);
            }

            template.CSharpFile.AfterBuild(_ =>
            {
                template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
                template.AddTypeSource(TemplateRoles.Domain.ValueObject);
                template.AddTypeSource(TemplateRoles.Domain.DataContract);

                var @class = template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
                var handleMethod = @class.FindMethod("Handle")!;
                handleMethod.Statements.Clear();
                handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

                var csharpMapping = handleMethod.GetMappingManager();
                csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new EnumCollectionMappingTypeResolver(template));
                csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(template));
                csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(template));

                csharpMapping.SetFromReplacement(model, "query");

                handleMethod.ImplementInteractions(interactions);

                if (model.TypeReference?.Element != null &&
                    !handleMethod.GetStatementsInPhase(ExecutionPhases.Response).Any())
                {
                    handleMethod.AddStatements(ExecutionPhases.Response, handleMethod.GetReturnStatements(model.TypeReference));
                }
            });
        }
    }
}
