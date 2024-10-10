using System;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class EndpointApiVersionModel : IApiVersionModel
{
    public EndpointApiVersionModel(string definitionName, string version, bool isDeprecated)
    {
        DefinitionName = definitionName;
        Version = version;
        IsDeprecated = isDeprecated;
    }

    public EndpointApiVersionModel(ICanBeReferencedType element)
    {
        var versionModel = element.AsVersionModel();
        if (versionModel == null)
        {
            throw new InvalidOperationException($"Element {element.Id} [{element.Name}] is not a VersionModel.");
        }

        DefinitionName = versionModel.ApiVersion?.Name;
        Version = versionModel.Name;
        IsDeprecated = versionModel.GetVersionSettings()?.IsDeprecated() == true;
    }

    public string DefinitionName { get; }
    public string Version { get; }
    public bool IsDeprecated { get; }

    protected bool Equals(EndpointApiVersionModel other)
    {
        return DefinitionName == other.DefinitionName && 
               Version == other.Version && 
               IsDeprecated == other.IsDeprecated;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((EndpointApiVersionModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DefinitionName, Version, IsDeprecated);
    }
}