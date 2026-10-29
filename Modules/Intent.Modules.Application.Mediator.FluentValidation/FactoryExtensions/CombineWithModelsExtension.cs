using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Mediator.FluentValidation.Templates;
using Intent.Modules.Application.Mediator.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.Mediator.FluentValidation.Templates.QueryValidator;
using Intent.Modules.Application.Mediator.Templates.CommandModels;
using Intent.Modules.Application.Mediator.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CombineWithModelsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Mediator.FluentValidation.CombineWithModelsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // With "Consolidate Command/Query associated files into single file" on, each validator is built into
            // its command's or query's own file, and the validator templates do not run (mirrors the MediatR
            // FluentValidation module). The core's templates are found by id and handled as SDK types only:
            // the core project is referenced with PrivateAssets=All, so only its inlined constants are used.
            if (!application.Settings.ConsolidateIntoSingleFile())
            {
                return;
            }

            var commandTemplatesByModelId = application
                .FindTemplateInstances<CSharpTemplateBase<CommandModel>>(TemplateDependency.OnTemplate(CommandModelsTemplate.TemplateId))
                .ToDictionary(x => x.Model.Id);
            foreach (var validatorTemplate in application.FindTemplateInstances<CommandValidatorTemplate>(TemplateDependency.OnTemplate(CommandValidatorTemplate.TemplateId)))
            {
                if (commandTemplatesByModelId.TryGetValue(validatorTemplate.Model.Id, out var commandTemplate))
                {
                    CommandValidatorTemplate.Configure(commandTemplate);
                }
            }

            var queryTemplatesByModelId = application
                .FindTemplateInstances<CSharpTemplateBase<QueryModel>>(TemplateDependency.OnTemplate(QueryModelsTemplate.TemplateId))
                .ToDictionary(x => x.Model.Id);
            foreach (var validatorTemplate in application.FindTemplateInstances<QueryValidatorTemplate>(TemplateDependency.OnTemplate(QueryValidatorTemplate.TemplateId)))
            {
                if (queryTemplatesByModelId.TryGetValue(validatorTemplate.Model.Id, out var queryTemplate))
                {
                    QueryValidatorTemplate.Configure(queryTemplate);
                }
            }
        }
    }
}
