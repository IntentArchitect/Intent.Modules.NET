using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations
{
    public class AccountFolderSharedFolderFilesStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Authentication.Templates.Templates.Server.StaticContentTemplateRegistrations.AccountFolderSharedFolderFilesStaticContentTemplateRegistration";

        public AccountFolderSharedFolderFilesStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "Components/Account/Shared";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => ReplacementsPrivate(outputTarget);

        [IntentIgnore]
        private Dictionary<string, string> ReplacementsPrivate(IOutputTarget outputTarget)
        {
            var replacements = new Dictionary<string, string>();

            replacements.Add("Namespace", outputTarget.GetNamespace().Replace("Components.Account.Shared", ""));

            if (!outputTarget.ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
            {
                replacements.Add("IdentityClass", "ApplicationUser");
                replacements.Add("NamespaceData", $"@using {outputTarget.GetNamespace().Replace("Components.Account.Shared", "")}Data");
                replacements.Add("IdentityClassNamespace", $"{outputTarget.GetNamespace().Replace("Components.Account.Shared", "")}Data");
            }
            else
            {
                var startup = outputTarget.ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
                var identityClass = IdentityHelperExtensions.GetIdentityUserClassTuple(startup);
                replacements.Add("IdentityClass", identityClass.Name);
                replacements.Add("NamespaceData", $"@using {identityClass.Namespace}");
                replacements.Add("IdentityClassNamespace", identityClass.Namespace);
            }

            return replacements;
        }

        [IntentIgnore]
        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (!application.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
            {
                return;
            }

            var mudBlazorInstalled = application.InstalledModules.Any(im => im.ModuleId == "Intent.Blazor.Components.MudBlazor");

            if (!mudBlazorInstalled)
            {
                base.Register(registry, application);
                return;
            }

            RegisterFiltered(registry, application, ext => ext == ".cs");
        }

        [IntentIgnore]
        private void RegisterFiltered(ITemplateInstanceRegistry registry, IApplication application, Func<string, bool> extensionFilter)
        {
            var assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location)!;
            var contentDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "content", ContentSubFolder));

            if (!Directory.Exists(contentDir))
            {
                return;
            }

            var allFiles = Directory.EnumerateFiles(contentDir, "*.*", System.IO.SearchOption.AllDirectories).ToArray();
            var getBinaryFiles = typeof(StaticContentTemplateRegistration)
                .GetMethod("GetBinaryFiles", BindingFlags.NonPublic | BindingFlags.Instance);
            var binaryFiles = getBinaryFiles != null
                ? (string[])getBinaryFiles.Invoke(this, new object[] { contentDir })!
                : Array.Empty<string>();

            var textFiles = allFiles.Except(binaryFiles).ToArray();

            foreach (var fileFullPath in textFiles)
            {
                var ext = Path.GetExtension(fileFullPath);
                if (!extensionFilter(ext))
                {
                    continue;
                }

                var fileRelativePath = Path.GetRelativePath(contentDir, fileFullPath);
                var capturedPath = fileFullPath;
                var capturedRel = fileRelativePath;
                RegisterTemplate(registry, application,
                    outputTarget => CreateTemplate(outputTarget, capturedPath, capturedRel,
                        GetDefaultOverrideBehaviour(outputTarget)));
            }

            foreach (var fileFullPath in binaryFiles)
            {
                var ext = Path.GetExtension(fileFullPath);
                if (!extensionFilter(ext))
                {
                    continue;
                }

                var fileRelativePath = Path.GetRelativePath(contentDir, fileFullPath);
                var capturedPath = fileFullPath;
                var capturedRel = fileRelativePath;
                RegisterTemplate(registry, application,
                    outputTarget => CreateBinaryTemplate(outputTarget, capturedPath, capturedRel,
                        GetDefaultOverrideBehaviour(outputTarget)));
            }
        }
    }
}