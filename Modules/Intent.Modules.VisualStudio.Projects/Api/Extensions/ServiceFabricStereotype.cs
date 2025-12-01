using System;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api.Extensions;

public class ServiceFabricStereotype
{
    public static readonly string Id = "1d9e433d-888c-4b97-b1cf-c61d565336d4";

    public ServiceFabricStereotype(IStereotype stereotype)
    {
        var typeValue = stereotype.GetProperty("1acc29d6-3b2e-427c-967d-77b807a42891").Value;
        Type = typeValue switch
        {
            "Actor" => ServiceFabricType.Actor,
            "Stateful" => ServiceFabricType.Stateful,
            "Stateless" => ServiceFabricType.Stateless,
            _ => throw new InvalidOperationException($"Unknown type: {typeValue}")
        };
    }

    public ServiceFabricType Type { get; }
}