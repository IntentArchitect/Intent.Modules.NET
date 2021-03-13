using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    public class IdentityServerQuickstartExtractorFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.IdentityServer4.UI.IdentityServerQuickstartExtractorFactoryExtension";
        public override int Order => 0;

        [IntentManaged(Mode.Ignore)]
        const string ZipFileResourcePath = "Intent.Modules.IdentityServer4.UI" + ".Resources." + RootFolder + ".zip";
        [IntentManaged(Mode.Ignore)]
        const string RootFolder = "IdentityServer4.Quickstart.UI-main";

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
            Path.Combine(RootFolder, "Quickstart", "Account", "AccountController.cs").Replace("\\", "/"),
            Path.Combine(RootFolder, "Quickstart", "Account", "ExternalController.cs").Replace("\\", "/"),
            Path.Combine(RootFolder, "Quickstart", "TestUsers.cs").Replace("\\", "/")
        };

        [IntentManaged(Mode.Ignore)]
        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                Logging.Log.Info($"Extracting content of {RootFolder}.zip ...");

                var targetApp = application.OutputTargets.SingleOrDefault(p => p.Parent == null);
                if (targetApp == null)
                {
                    Logging.Log.Warning("No host application found to output zip file content");
                    return;
                }

                var filecheckpath = Path.Combine(targetApp.Location, "Quickstart", "Extracted.txt");
                if (File.Exists(filecheckpath)) { return; }

                File.WriteAllText(filecheckpath, "This file is used to indicate whether the UI contents have been extracted by Intent Architect");

                var zipFileStream = GetFileFromResource(ZipFileResourcePath);

                ExtractFilesToRootPath(zipFileStream, targetApp.Location);

                Logging.Log.Info($"Extraction complete.");
            }
        }

        [IntentManaged(Mode.Ignore)]
        private void ExtractFilesToRootPath(Stream zipFileStream, string rootLocation)
        {
            using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries.Where(p => p.Name != string.Empty))
                {
                    if (FilesToIgnore.Contains(entry.FullName)) { continue; }

                    var pathComponents = entry.FullName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (pathComponents.Intersect(TargetFolders).Any())
                    {
                        var outpath = Path.Combine(rootLocation, entry.FullName.Replace(RootFolder + "/", string.Empty));
                        var fileInfo = new FileInfo(outpath);
                        fileInfo.Directory.Create();
                        entry.ExtractToFile(outpath);
                    }
                }
            }
        }

        [IntentManaged(Mode.Ignore)]
        private Stream GetFileFromResource(string resourcePath)
        {
            var stream = typeof(IdentityServerQuickstartExtractorFactoryExtension).Assembly.GetManifestResourceStream(resourcePath);
            return stream;
        }
    }
}