#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric
{
    public class ManifestImportRequiredEvent
    {
        public ManifestImportRequiredEvent(string serviceManifestName)
        {
            ServiceManifestName = serviceManifestName;
        }

        public string ServiceManifestName { get; }
        public IReadOnlyCollection<ManifestImportConfigOverride>? ConfigOverrides { get; set; }
        public IReadOnlyCollection<EnvironmentVariable>? EnvironmentOverrides { get; set; }
    }
}
