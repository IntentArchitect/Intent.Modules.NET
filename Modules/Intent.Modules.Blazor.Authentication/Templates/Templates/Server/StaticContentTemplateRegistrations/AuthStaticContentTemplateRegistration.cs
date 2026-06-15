using System;
using System.IO;
using System.Linq;
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
            return fileRelativePath.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
                   || fileRelativePath.EndsWith(RazorScopedCssSuffix, StringComparison.OrdinalIgnoreCase)
                   || fileRelativePath.EndsWith(RazorCodeBehindSuffix, StringComparison.OrdinalIgnoreCase)
                ? OverwriteBehaviour.Always
                : GetDefaultOverrideBehaviour(outputTarget);
        }

        private string[] GetBinaryFiles(string contentDir)
        {
            if (BinaryFileGlobbingPatterns.Length == 0)
            {
                return Array.Empty<string>();
            }

            return Directory.EnumerateFiles(contentDir, "*.*", System.IO.SearchOption.AllDirectories)
                .Where(IsBinaryFile)
                .ToArray();
        }

        // All BinaryFileGlobbingPatterns are simple "*.ext" globs, so a suffix match is sufficient. This
        // avoids both a Microsoft.Extensions.FileSystemGlobbing dependency and reflecting into the base
        // class's private binary-file detection.
        private bool IsBinaryFile(string fileFullPath)
        {
            var extension = Path.GetExtension(fileFullPath);
            return BinaryFileGlobbingPatterns.Any(pattern =>
                pattern.StartsWith("*.", StringComparison.Ordinal)
                && extension.Equals(pattern.Substring(1), StringComparison.OrdinalIgnoreCase));
        }
    }
}
