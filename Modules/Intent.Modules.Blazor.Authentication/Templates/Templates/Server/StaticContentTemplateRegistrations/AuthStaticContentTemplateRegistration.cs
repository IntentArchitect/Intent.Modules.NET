using System;
using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    public abstract class AuthStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        private const string RazorScopedCssSuffix = ".razor.css";
        private const string RazorCodeBehindSuffix = ".razor.cs";

        protected AuthStaticContentTemplateRegistration(string templateId) : base(templateId)
        {
        }

        protected void RegisterAuthStaticContent(ITemplateInstanceRegistry registry, IApplication application, Func<string, bool> extensionFilter = null, Func<string, bool> pathFilter = null)
        {
            var assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location)!;
            var contentDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "content", ContentSubFolder));

            if (!Directory.Exists(contentDir))
            {
                return;
            }

            foreach (var fileFullPath in Directory.EnumerateFiles(contentDir, "*.*", System.IO.SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(fileFullPath);
                if (extensionFilter != null && !extensionFilter(ext))
                {
                    continue;
                }

                var fileRelativePath = Path.GetRelativePath(contentDir, fileFullPath);
                // pathFilter receives the forward-slash relative path (e.g. "_Imports.razor",
                // "Manage/Index.razor"), letting a registration ship only a subset of its folder —
                // used to give JWT-mode apps just the mode-independent account shell (layout
                // wiring + skin) without the Identity-only pages.
                if (pathFilter != null && !pathFilter(fileRelativePath.Replace('\\', '/')))
                {
                    continue;
                }

                var capturedPath = fileFullPath;
                var capturedRel = fileRelativePath;
                RegisterTemplate(registry, application,
                    outputTarget => CreateTemplate(outputTarget, capturedPath, capturedRel,
                        GetOverwriteBehaviour(outputTarget, capturedRel)));
            }
        }

        private OverwriteBehaviour GetOverwriteBehaviour(IOutputTarget outputTarget, string fileRelativePath)
        {
            return fileRelativePath.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
                   || fileRelativePath.EndsWith(RazorScopedCssSuffix, StringComparison.OrdinalIgnoreCase)
                   || fileRelativePath.EndsWith(RazorCodeBehindSuffix, StringComparison.OrdinalIgnoreCase)
                ? OverwriteBehaviour.Always
                : GetDefaultOverrideBehaviour(outputTarget);
        }
    }
}
