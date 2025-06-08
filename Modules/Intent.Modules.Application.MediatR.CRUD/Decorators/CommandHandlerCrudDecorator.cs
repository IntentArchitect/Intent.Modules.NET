using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.InteractionStrategies;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;
using System.Linq;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Common.CSharp.Mapping.Resolvers;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class CommandHandlerCrudDecorator : CommandHandlerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.MediatR.CRUD.CommandHandlerCrudDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly CommandHandlerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;


        [IntentManaged(Mode.Merge)]
        public CommandHandlerCrudDecorator(CommandHandlerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            CSharpTemplateBase<CommandModel> targetTemplate = template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
                ? template.GetTemplate<CommandModelsTemplate>(CommandModelsTemplate.TemplateId, template.Model)
                : template;

            var legacyStrategy = StrategyFactory.GetMatchedCommandStrategy(targetTemplate);
            if (legacyStrategy is not null)
            {
                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ => legacyStrategy.ApplyStrategy());
            }

            var model = targetTemplate.Model;
            var interactions = model.GetInteractions().ToList();
            if (interactions.Any())
            {
                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ =>
                {
                    var t = (ICSharpFileBuilderTemplate)targetTemplate;
                    t.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
                    t.AddTypeSource(TemplateRoles.Domain.ValueObject);
                    t.AddTypeSource(TemplateRoles.Domain.DataContract);
                    t.AddTypeSource(TemplateRoles.Domain.Entity.Behaviour); // So that the mapping system can find the constructor when Separated State and Behaviours enabled in domain.

                    var @class = t.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);
                    var handleMethod = @class.FindMethod("Handle");
                    if (legacyStrategy is null)
                    {
                        handleMethod.Statements.Clear();
                    }

                    handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

                    var csharpMapping = handleMethod.GetMappingManager();
                    //[REVISIT]: This is a hack to get things working properly
                    csharpMapping.ClearMappingResolvers();
                    csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new EnumCollectionMappingTypeResolver(t));
                    csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(t));
                    csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(t));
                    var domainInteractionManager = new DomainInteractionsManager(t, csharpMapping);

                    csharpMapping.SetFromReplacement(model, "request");

                    handleMethod.ImplementInteractions(interactions);

                    if (legacyStrategy is null && model.TypeReference.Element != null)
                    {
                        var returnStatement = domainInteractionManager.GetReturnStatements(handleMethod, model.TypeReference);
                        handleMethod.AddStatements(ExecutionPhases.Response, returnStatement);
                    }
                });
            }
        }
    }
}