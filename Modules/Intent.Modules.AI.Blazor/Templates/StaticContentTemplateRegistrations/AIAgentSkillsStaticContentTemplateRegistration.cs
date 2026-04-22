using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates.AIStaticContent;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations
{
    [IntentMerge]
    public class AIAgentSkillsStaticContentTemplateRegistration : AIStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations.AIAgentSkillsStaticContentTemplateRegistration";

        [IntentMerge]
        public AIAgentSkillsStaticContentTemplateRegistration(IApplicationConfigurationProvider configurationProvider) : base(TemplateId, configurationProvider)
        {
        }

        public override string ContentSubFolder => "AgentSkills";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}
