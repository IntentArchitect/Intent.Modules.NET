using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations
{
    public class ThemeArtifactsStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Components.MudBlazor.Templates.StaticContentTemplateRegistrations.ThemeArtifactsStaticContentTemplateRegistration";

        // [IntentIgnore] so the Software Factory does not strip this when it regenerates this Mode.Fully file
        // (the [IntentIgnore] CreateTemplate below depends on it).
        [IntentIgnore]
        private const string ThemeToggleFileName = "ThemeToggle.razor";

        public ThemeArtifactsStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Theme";
        [IntentIgnore]

        [IntentIgnore]
        protected override ITemplate CreateTemplate(IOutputTarget outputTarget, string fileFullPath, string fileRelativePath, OverwriteBehaviour defaultOverwriteBehaviour)
        {
            // StartsWith covers both ThemeToggle.razor and its co-located ThemeToggle.razor.css.
            var overwriteBehaviour = Path.GetFileName(fileRelativePath).StartsWith(ThemeToggleFileName, StringComparison.OrdinalIgnoreCase)
                                     || fileRelativePath.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
                                     || fileRelativePath.EndsWith(".razor.css", StringComparison.OrdinalIgnoreCase)
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
