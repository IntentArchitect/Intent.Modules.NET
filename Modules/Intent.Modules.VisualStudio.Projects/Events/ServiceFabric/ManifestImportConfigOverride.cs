#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public class ManifestImportConfigOverride
{
    public ManifestImportConfigOverride(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public IReadOnlyCollection<ManifestImportConfigOverrideSettingSection> SettingSections { get; set; } = new List<ManifestImportConfigOverrideSettingSection>();
}