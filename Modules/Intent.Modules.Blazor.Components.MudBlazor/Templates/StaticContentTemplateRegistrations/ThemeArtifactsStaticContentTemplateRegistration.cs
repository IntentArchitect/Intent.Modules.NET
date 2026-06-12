using System;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System.Collections.Generic;
using System.IO;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    public class ThemeArtifactsStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.ThemeArtifactsStaticContentTemplateRegistration";
        private const string ThemeToggleFileName = "ThemeToggle.razor";

        public ThemeArtifactsStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Theme";

        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OnceOff;
        }

        [IntentIgnore]
        protected override ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            var overwriteBehaviour = Path.GetFileName(fileRelativePath).Equals(ThemeToggleFileName, StringComparison.OrdinalIgnoreCase)
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
