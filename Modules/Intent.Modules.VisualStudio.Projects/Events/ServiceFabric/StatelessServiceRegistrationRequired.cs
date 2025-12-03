using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public class StatelessServiceRegistrationRequired : ServiceRegistrationRequiredBase
{
    public StatelessServiceRegistrationRequired(
        string name,
        string serviceTypeName,
        string instanceCount) : base(name, serviceTypeName)
    {
        InstanceCount = instanceCount;
    }

    /// <summary>
    /// If surrounded by [] will be treated as a parameter reference.
    /// </summary>
    public string InstanceCount { get; }

    /// <summary>
    /// Only applicable if <see cref="InstanceCount"/> is surrounded by [].
    /// </summary>
    public int? InstanceCountParameterDefaultValue { get; set; }

    /// <summary>
    /// Only applicable if <see cref="InstanceCount"/> is surrounded by [].
    /// </summary>
    public Dictionary<string, int> InstanceCountParameterEnvironmentValues { get; } = [];
}