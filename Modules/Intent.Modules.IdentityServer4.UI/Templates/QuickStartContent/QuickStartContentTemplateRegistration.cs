using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.UI.Resources;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Templates.QuickStartContent
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class QuickStartContentTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public QuickStartContentTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => QuickStartContentTemplate.TemplateId;

        [IntentManaged(Mode.Ignore)]
        private static readonly IReadOnlyCollection<string> TargetFolders = new string[]
        {
            "Quickstart",
            "Views",
            "wwwroot"
        };

        [IntentManaged(Mode.Ignore)]
        private static readonly HashSet<string> FilesToIgnore = new HashSet<string>
        {
            Path.Combine(ResourceHelper.QuickstartFileName, "Quickstart", "Account", "AccountController.cs").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "Quickstart", "Account", "ExternalController.cs").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "Quickstart", "TestUsers.cs").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "wwwroot", "favicon.ico").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "wwwroot", "icon.jpg").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "wwwroot", "icon.png").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "getmain.ps1").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "getmain.sh").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "LICENSE").Replace("\\", "/"),
            Path.Combine(ResourceHelper.QuickstartFileName, "README.md").Replace("\\", "/")
        };

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            ResourceHelper.ReadQuickstartFileContents(archive =>
            {
                foreach (var entry in archive.Entries.Where(p => p.Name != string.Empty))
                {
                    if (FilesToIgnore.Contains(entry.FullName)) { continue; }

                    registry.RegisterTemplate(TemplateId, project => new QuickStartContentTemplate(project, new ZipEntry
                    {
                        FullFileNamePath = entry.FullName
                            .Replace(ResourceHelper.QuickstartFileName + "/", string.Empty)
                            .Replace("Quickstart/", "Controllers/"),
                        Content = new StreamReader(entry.Open()).ReadToEnd()
                    }));
                }
            });
        }
    }
}