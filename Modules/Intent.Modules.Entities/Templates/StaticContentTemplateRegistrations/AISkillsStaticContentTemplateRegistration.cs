using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates.AIStaticContent;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.StaticContentTemplateRegistrations
{
    public class AISkillsStaticContentTemplateRegistration : AIStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Entities.Templates.StaticContentTemplateRegistrations.AISkillsStaticContentTemplateRegistration";

        public AISkillsStaticContentTemplateRegistration(IApplicationConfigurationProvider applicationConfigurationProvider) : base(TemplateId, applicationConfigurationProvider)
        {
        }

        public override string ContentSubFolder => "AISkills";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}