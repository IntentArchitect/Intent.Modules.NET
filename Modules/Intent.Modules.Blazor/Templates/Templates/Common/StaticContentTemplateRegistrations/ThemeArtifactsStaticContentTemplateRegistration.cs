using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations
{
    public class ThemeArtifactsStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations.ThemeArtifactsStaticContentTemplateRegistration";
        private const string ThemeScriptFileName = "theme.js";

        public ThemeArtifactsStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Theme";

        [IntentIgnore]
        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OnceOff;
        }

        // theme.js is a generated utility file; CSS remains OnceOff so design-token customisations survive regeneration.
        [IntentIgnore]
        protected override ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            var overwriteBehaviour = Path.GetFileName(fileRelativePath).Equals(ThemeScriptFileName, StringComparison.OrdinalIgnoreCase)
                ? OverwriteBehaviour.Always
                : defaultOverwriteBehaviour;

            return base.CreateTemplate(outputTarget, fileFullPath, fileRelativePath, overwriteBehaviour);
        }

        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}
