using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.NetTopologySuite.TypeSources;

public class GeometryTypeSource : ITypeSource
{
    private readonly ICSharpTemplate _template;

    public GeometryTypeSource(ICSharpTemplate template)
    {
        _template = template;
    }

    public IResolvedTypeInfo? GetType(ITypeReference? typeReference)
    {
        const string pointTypeDefinitionId = "553b44ae-1d22-4b27-aec9-23f61328d2b8";
        if (typeReference?.Element?.Id == pointTypeDefinitionId)
        {
            _template.AddNugetDependency(NugetPackages.NetTopologySuite);
        }

        return null;
    }

    public IEnumerable<ITemplateDependency> GetTemplateDependencies()
    {
        yield break;
    }

    public ICollectionFormatter CollectionFormatter { get; }
    public INullableFormatter NullableFormatter { get; }
}