using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    public abstract class AuthStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        private const string RazorScopedCssSuffix = ".razor.css";
        private const string ManageLayoutFileName = "ManageLayout.razor";
        private const string ManageLayoutCodeBehindFileName = "ManageLayout.razor.cs";

        protected AuthStaticContentTemplateRegistration(string templateId) : base(templateId)
        {
        }

        protected void RegisterAuthStaticContent(ITemplateInstanceRegistry registry, IApplication application, Func<string, bool> extensionFilter = null)
        {
            var assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location)!;
            var contentDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "content", ContentSubFolder));

            if (!Directory.Exists(contentDir))
            {
                return;
            }

            var allFiles = Directory.EnumerateFiles(contentDir, "*.*", System.IO.SearchOption.AllDirectories).ToArray();
            var binaryFiles = GetBinaryFiles(contentDir);
            var textFiles = allFiles.Except(binaryFiles).ToArray();

            foreach (var fileFullPath in textFiles)
            {
                var ext = Path.GetExtension(fileFullPath);
                if (extensionFilter != null && !extensionFilter(ext))
                {
                    continue;
                }

                var fileRelativePath = Path.GetRelativePath(contentDir, fileFullPath);
                var capturedPath = fileFullPath;
                var capturedRel = fileRelativePath;
                RegisterTemplate(registry, application,
                    outputTarget => CreateTemplate(outputTarget, capturedPath, capturedRel,
                        GetOverwriteBehaviour(outputTarget, capturedRel)));
            }

            foreach (var fileFullPath in binaryFiles)
            {
                var ext = Path.GetExtension(fileFullPath);
                if (extensionFilter != null && !extensionFilter(ext))
                {
                    continue;
                }

                var fileRelativePath = Path.GetRelativePath(contentDir, fileFullPath);
                var capturedPath = fileFullPath;
                var capturedRel = fileRelativePath;
                RegisterTemplate(registry, application,
                    outputTarget => CreateBinaryTemplate(outputTarget, capturedPath, capturedRel,
                        GetOverwriteBehaviour(outputTarget, capturedRel)));
            }
        }

        private OverwriteBehaviour GetOverwriteBehaviour(IOutputTarget outputTarget, string fileRelativePath)
        {
            var fileName = Path.GetFileName(fileRelativePath);
            return fileRelativePath.EndsWith(RazorScopedCssSuffix, StringComparison.OrdinalIgnoreCase)
                   || fileName.Equals(ManageLayoutFileName, StringComparison.OrdinalIgnoreCase)
                   || fileName.Equals(ManageLayoutCodeBehindFileName, StringComparison.OrdinalIgnoreCase)
                ? OverwriteBehaviour.Always
                : GetDefaultOverrideBehaviour(outputTarget);
        }

        private string[] GetBinaryFiles(string contentDir)
        {
            var getBinaryFiles = typeof(StaticContentTemplateRegistration)
                .GetMethod("GetBinaryFiles", BindingFlags.NonPublic | BindingFlags.Instance);
            return getBinaryFiles != null
                ? (string[])getBinaryFiles.Invoke(this, new object[] { contentDir })!
                : Array.Empty<string>();
        }
    }
}
