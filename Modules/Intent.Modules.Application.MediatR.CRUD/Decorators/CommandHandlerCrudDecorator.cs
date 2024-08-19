using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

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

            var matchedStrategy = StrategyFactory.GetMatchedCommandStrategy(targetTemplate);
            if (matchedStrategy is not null)
            {
                targetTemplate.AddKnownType("System.Linq.Dynamic.Core.PagedResult");
                ((ICSharpFileBuilderTemplate)targetTemplate).CSharpFile.AfterBuild(_ => matchedStrategy.ApplyStrategy());
            };
        }
    }
}