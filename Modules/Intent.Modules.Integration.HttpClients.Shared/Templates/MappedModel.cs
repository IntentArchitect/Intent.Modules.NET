using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;

public class MappedModel
{
    public List<MappedOperation> Operations { get; set; }

}

public class MappedOperation
{
    public ITypeReference ReturnType { get; set; }
    public string Name { get; set; }
    public List<MappedParameter> Parameters { get; set; }
    public string RelativeUri { get; set; }
}

public class MappedParameter
{
    public ITypeReference Type { get; set; }
    public string Name { get; set; }
    public SourceOptionsEnum Source { get; set; }
}


public enum SourceOptionsEnum
{
    Default,
    FromQuery,
    FromBody,
    FromForm,
    FromRoute,
    FromHeader,
}