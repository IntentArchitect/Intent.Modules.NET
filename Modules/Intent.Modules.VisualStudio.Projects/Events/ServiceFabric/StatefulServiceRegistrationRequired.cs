using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public class StatefulServiceRegistrationRequired : ServiceRegistrationRequiredBase
{
    public StatefulServiceRegistrationRequired(
        string name,
        string serviceTypeName,
        string targetReplicaSetSize,
        string minReplicaSetSize,
        string partitionCount) : base(name, serviceTypeName)
    {
        TargetReplicaSetSize = targetReplicaSetSize;
        MinReplicaSetSize = minReplicaSetSize;
        PartitionCount = partitionCount;
    }

    /// <summary>
    /// If surrounded by [] will be treated as a parameter reference.
    /// </summary>
    public string TargetReplicaSetSize { get; }

    /// <summary>
    /// Only applicable if <see cref="TargetReplicaSetSizeParameterDefaultValue"/> is surrounded by [].
    /// </summary>
    public int? TargetReplicaSetSizeParameterDefaultValue { get; set; }

    /// <summary>
    /// Only applicable if <see cref="TargetReplicaSetSize"/> is surrounded by [].
    /// </summary>
    public Dictionary<string, int> TargetReplicaSizeParameterEnvironmentValues { get; } = [];

    /// <summary>
    /// If surrounded by [] will be treated as a parameter reference.
    /// </summary>
    public string MinReplicaSetSize { get; }

    /// <summary>
    /// Only applicable if <see cref="MinReplicaSetSize"/> is surrounded by [].
    /// </summary>
    public int? MinReplicaSetSizeParameterDefaultValue { get; set; }

    /// <summary>
    /// Only applicable if <see cref="MinReplicaSetSize"/> is surrounded by [].
    /// </summary>
    public Dictionary<string, int> MinReplicaSetSizeParameterEnvironmentValues { get; } = [];

    /// <summary>
    /// If surrounded by [] will be treated as a parameter reference.
    /// </summary>
    public string PartitionCount { get; set; }

    /// <summary>
    /// Only applicable if <see cref="PartitionCount"/> is surrounded by [].
    /// </summary>
    public int? PartitionCountParameterDefaultValue { get; set; }

    /// <summary>
    /// Only applicable if <see cref="PartitionCount"/> is surrounded by [].
    /// </summary>
    public Dictionary<string, int> PartitionCountParameterEnvironmentValues { get; } = [];
}