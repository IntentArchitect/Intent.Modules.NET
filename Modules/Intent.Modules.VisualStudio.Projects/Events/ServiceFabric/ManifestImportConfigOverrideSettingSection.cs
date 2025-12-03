#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public class ManifestImportConfigOverrideSettingSection
{
    public ManifestImportConfigOverrideSettingSection(string name, IReadOnlyCollection<Parameter> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    public string Name { get; }
    public IReadOnlyCollection<Parameter> Parameters { get; }
}