#nullable enable

namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public class Parameter
{
    public Parameter(string name, string value, string? parameterDefaultValue)
    {
        Name = name;
        Value = value;
        ParameterDefaultValue = parameterDefaultValue;
    }

    public string Name { get; }
    public string Value { get; }
    public string? ParameterDefaultValue { get; }
}