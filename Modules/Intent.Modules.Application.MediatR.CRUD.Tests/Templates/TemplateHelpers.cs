using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates
{
    internal static class TemplateHelpers
    {
        private static readonly TemplateDiscoveryOptions TemplateDiscoveryOptions = new() { ThrowIfNotFound = false, TrackDependency = true };
        private static readonly TemplateDiscoveryOptions TemplateDiscoveryOptionsNoTracking = new() { ThrowIfNotFound = false, TrackDependency = false };

        public static CSharpTemplateBase<CommandModel> GetCommandHandlerTemplate(
            this ICSharpTemplate template,
            CommandModel model,
            bool trackDependency)
        {
            return ((IntentTemplateBase)template).GetTemplate<CSharpTemplateBase<CommandModel>>(
                dependency: new TemplateDependency<CommandModel>("Application.Command.Handler", model),
                options: trackDependency ? TemplateDiscoveryOptions : TemplateDiscoveryOptionsNoTracking);
        }

        public static CSharpTemplateBase<QueryModel> GetQueryHandlerTemplate(
            this ICSharpTemplate template,
            QueryModel model,
            bool trackDependency)
        {
            return ((IntentTemplateBase)template).GetTemplate<CSharpTemplateBase<QueryModel>>(
                dependency: new TemplateDependency<QueryModel>("Application.Query.Handler", model),
                options: trackDependency ? TemplateDiscoveryOptions : TemplateDiscoveryOptionsNoTracking);
        }

        public static bool TryGetCommandValidatorTemplate<T>(
            this CSharpTemplateBase<T> template,
            CommandModel model,
            out ICSharpFileBuilderTemplate validatorTemplate)
        {
            return template.ExecutionContext.Settings.GetCQRSSettings().GroupCommandsQueriesHandlersAndValidatorsIntoSingleFile()
                ? template.TryGetTemplate(CommandModelsTemplate.TemplateId, model, out validatorTemplate)
                : template.TryGetTemplate(CommandValidatorTemplate.TemplateId, model, out validatorTemplate);
        }

        private class TemplateDependency<TModel> : ITemplateDependency
            where TModel : IMetadataModel
        {
            private readonly TModel _model;

            public TemplateDependency(string templateIdOrRole, TModel model)
            {
                TemplateId = templateIdOrRole;
                _model = model;
            }

            public bool IsMatch(ITemplate template)
            {
                return template.CanRunTemplate() &&
                       template is CSharpTemplateBase<TModel> cSharpTemplateBaseOfT &&
                       cSharpTemplateBaseOfT.Model.Id == _model.Id;
            }

            public string TemplateId { get; }
        }
    }
}
