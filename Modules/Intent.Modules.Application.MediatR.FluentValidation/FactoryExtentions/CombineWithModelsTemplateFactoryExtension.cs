using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.QueryValidator;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CombineWithModelsTemplateFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.FluentValidation.CombineWithModelsTemplateFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile())
            {
                return;
            }

            var queryModelTemplatesByModelId = application
                .FindTemplateInstances<QueryModelsTemplate>(TemplateDependency.OnTemplate(QueryModelsTemplate.TemplateId))
                .ToDictionary(x => x.Model.Id);
            var queryValidatorTemplates = application.FindTemplateInstances<QueryValidatorTemplate>(TemplateDependency.OnTemplate(QueryValidatorTemplate.TemplateId));
            foreach (var validatorTemplate in queryValidatorTemplates)
            {
                QueryValidatorTemplate.Configure(queryModelTemplatesByModelId[validatorTemplate.Model.Id], null);
            }

            var commandModelTemplatesByModelId = application
                .FindTemplateInstances<CommandModelsTemplate>(TemplateDependency.OnTemplate(CommandModelsTemplate.TemplateId))
                .ToDictionary(x => x.Model.Id);
            var commandValidatorTemplates = application
                .FindTemplateInstances<CommandValidatorTemplate>(TemplateDependency.OnTemplate(CommandValidatorTemplate.TemplateId));
            foreach (var validatorTemplate in commandValidatorTemplates)
            {
                CommandValidatorTemplate.Configure(commandModelTemplatesByModelId[validatorTemplate.Model.Id], null);
            }
        }
    }
}