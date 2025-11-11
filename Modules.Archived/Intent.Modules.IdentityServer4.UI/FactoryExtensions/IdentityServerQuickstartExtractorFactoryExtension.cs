using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.IdentityServer4.UI.Resources;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

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
        public void OnStep(IApplication application, string step)
        {
            const string ROLE_DISTRIBUTION = "Distribution";

            if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                Logging.Log.Info($"Extracting content of {ResourceHelper.QuickstartFileName}.zip ...");

                var targetApp = application.OutputTargets.SingleOrDefault(p => p.HasRole(ROLE_DISTRIBUTION));
                if (targetApp == null)
                {
                    Logging.Log.Warning("No host application found to output zip file content");
                    return;
                }

                var filecheckpath = Path.Combine(targetApp.Location, "wwwroot", "favicon.ico");
                if (File.Exists(filecheckpath)) { return; }

                ResourceHelper.ReadQuickstartFileContents(archive =>
                {
                    foreach (var entry in archive.Entries
                        .Where(p => p.Name != null && ResourceHelper.ImageFiles.Contains(p.Name)))
                    {
                        var pathComponents = entry.FullName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        var outpath = Path.Combine(targetApp.Location, entry.FullName.Replace(ResourceHelper.QuickstartFileName + "/", string.Empty));
                        var fileInfo = new FileInfo(outpath);
                        fileInfo.Directory.Create();
                        entry.ExtractToFile(outpath);
                    }
                });

                Logging.Log.Info($"Extraction complete.");
            }
        }
    }
}