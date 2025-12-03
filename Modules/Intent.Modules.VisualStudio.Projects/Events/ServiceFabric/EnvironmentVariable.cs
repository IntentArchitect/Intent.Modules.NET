#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public class EnvironmentVariable
{
    public EnvironmentVariable(string name, string value, string? parameterDefaultValue)
    {
        Name = name;
        Value = value;
        ParameterDefaultValue = parameterDefaultValue;
    }

    public string Name { get; set; }

    /// <summary>
    /// If value is wrapped in [] then it is a reference to a parameter.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Only applicable to if <see cref="Value"/> is wrapped in [].
    /// </summary>
    public string? ParameterDefaultValue { get; set; }

    public Dictionary<string, string> EnvironmentParameterValues { get; set; } = [];
}