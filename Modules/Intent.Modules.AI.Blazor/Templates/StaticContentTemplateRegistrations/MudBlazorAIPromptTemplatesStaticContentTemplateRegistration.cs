using System.Collections.Generic;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.AI.Blazor.Tasks.Config;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class MudBlazorAIPromptTemplatesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        private readonly ISolutionConfig _solutionConfig;
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;


        public new const string TemplateId = "Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations.MudBlazorAIPromptTemplatesStaticContentTemplateRegistration";
    
        [IntentMerge]
        public MudBlazorAIPromptTemplatesStaticContentTemplateRegistration(ISolutionConfig solutionConfig, IApplicationConfigurationProvider applicationConfigurationProvider) : base(TemplateId)
        {
            _solutionConfig = solutionConfig;
            _applicationConfigurationProvider = applicationConfigurationProvider;
        }

        public override string ContentSubFolder => "MudBlazorAIPromptTemplates";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };

        public override string RelativeOutputPathPrefix => PromptConfig.GetTemplatePromptPath(_solutionConfig.SolutionRootLocation, _applicationConfigurationProvider.GetApplicationConfig().Name);

        protected override ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            if (fileFullPath.EndsWith(".json"))
            {
                //Taking this out for now JSON Weaving is not working so well or im doing it wrong
                //return new JsonStaticContentTemplate(fileFullPath, fileRelativePath, RelativeOutputPathPrefix, TemplateId, outputTarget, Replacements(outputTarget), defaultOverwriteBehaviour);
            }
            return base.CreateTemplate(outputTarget, fileFullPath, fileRelativePath, defaultOverwriteBehaviour);
        }
    }
}