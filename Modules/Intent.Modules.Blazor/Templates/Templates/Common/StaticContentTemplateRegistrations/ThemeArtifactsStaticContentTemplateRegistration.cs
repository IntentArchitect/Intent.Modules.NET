using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations
{
    public class ThemeArtifactsStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Common.StaticContentTemplateRegistrations.ThemeArtifactsStaticContentTemplateRegistration";

        [IntentIgnore]
        private const string MudBlazorModuleId = "Intent.Blazor.Components.MudBlazor";
        [IntentIgnore]
        private const string ThemeStorageScriptFileName = "theme-storage.js";
        [IntentIgnore]
        private const string ThemeToggleFileName = "ThemeToggle.razor";

        public ThemeArtifactsStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Theme";

        [IntentIgnore]
        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return OverwriteBehaviour.OnceOff;
        }

        // The MudBlazor module ships its own Mud-flavoured ThemeToggle (.razor + .razor.css) to the same output
        // path, so when it is installed we skip ours to avoid two modules emitting the same file. Everything else
        // (theme-storage.js, the design-token CSS) is emitted regardless. theme-storage.js and the ThemeToggle
        // files are generated infrastructure (Always); the design-token CSS stays OnceOff so customisations survive.
        [IntentIgnore]
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            var assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location)!;
            var contentDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "content", ContentSubFolder));

            if (!Directory.Exists(contentDir))
            {
                return;
            }

            var mudBlazorInstalled = application.InstalledModules.Any(module => module.ModuleId == MudBlazorModuleId);

            foreach (var fileFullPath in Directory.EnumerateFiles(contentDir, "*.*", System.IO.SearchOption.AllDirectories))
            {
                var fileRelativePath = Path.GetRelativePath(contentDir, fileFullPath);
                var fileName = Path.GetFileName(fileRelativePath);

                // StartsWith covers both ThemeToggle.razor and its co-located ThemeToggle.razor.css.
                if (mudBlazorInstalled && fileName.StartsWith(ThemeToggleFileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var capturedPath = fileFullPath;
                var capturedRel = fileRelativePath;
                if (IsBinaryFile(fileRelativePath))
                {
                    RegisterTemplate(registry, application,
                        outputTarget => CreateBinaryTemplate(outputTarget, capturedPath, capturedRel, GetOverwriteBehaviour(outputTarget, capturedRel)));
                }
                else
                {
                    RegisterTemplate(registry, application,
                        outputTarget => CreateTemplate(outputTarget, capturedPath, capturedRel, GetOverwriteBehaviour(outputTarget, capturedRel)));
                }
            }
        }

        [IntentIgnore]
        private OverwriteBehaviour GetOverwriteBehaviour(IOutputTarget outputTarget, string fileRelativePath)
        {
            var fileName = Path.GetFileName(fileRelativePath);
            return fileName.Equals(ThemeStorageScriptFileName, StringComparison.OrdinalIgnoreCase)
                   || fileName.StartsWith(ThemeToggleFileName, StringComparison.OrdinalIgnoreCase)
                ? OverwriteBehaviour.Always
                : GetDefaultOverrideBehaviour(outputTarget);
        }

        [IntentIgnore]
        private bool IsBinaryFile(string fileRelativePath)
        {
            var extension = Path.GetExtension(fileRelativePath);
            return BinaryFileGlobbingPatterns.Any(pattern =>
                pattern.StartsWith("*.", StringComparison.Ordinal)
                && extension.Equals(pattern.Substring(1), StringComparison.OrdinalIgnoreCase));
        }

        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}
