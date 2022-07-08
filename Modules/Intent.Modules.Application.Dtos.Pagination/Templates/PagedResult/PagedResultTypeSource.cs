using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Application.Dtos.Pagination.Templates.PagedResult;

public class PagedResultTypeSource : ITypeSource
{
    private readonly PagedResultTemplate _template;

    public PagedResultTypeSource(PagedResultTemplate template)
    {
        _template = template;
    }

    public IResolvedTypeInfo GetType(ITypeReference typeInfo)
    {
        const string pagedResultTypeDefinitionId = "9204e067-bdc8-45e7-8970-8a833fdc5253";
        if (typeInfo.Element.Id == pagedResultTypeDefinitionId)
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