using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.AzureFunctions.Templates.ReturnTypes.ResourceLocationClass;

public class ResourceLocationTypeSource : ITypeSource
{
    private readonly ResourceLocationClassTemplate _template;

    public ResourceLocationTypeSource(ResourceLocationClassTemplate template)
    {
        _template = template;
    }

    public IResolvedTypeInfo GetType(ITypeReference typeInfo)
    {
        if (typeInfo?.Element?.Id == TypeDefinitionIds.ResourceLocationVoidTypeDefId
            || typeInfo?.Element?.Id == TypeDefinitionIds.ResourceLocationPayloadTypeDefId)
        {
            return CSharpResolvedTypeInfo.Create(
                resolvedTypeInfo: ResolvedTypeInfo.Create(
                    name: _template.ClassName,
                    isPrimitive: false,
                    typeReference: typeInfo,
                    template: _template),
                genericTypeParameters: null);
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