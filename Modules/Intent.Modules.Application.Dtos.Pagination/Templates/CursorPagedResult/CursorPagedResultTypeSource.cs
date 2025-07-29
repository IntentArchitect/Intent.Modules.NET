using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Application.Dtos.Pagination.Templates.CursorPagedResult;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Application.Dtos.Pagination.Templates.PagedResult;

public class CursorPagedResultTypeSource : ITypeSource
{
    private readonly CursorPagedResultTemplate _template;

    public CursorPagedResultTypeSource(CursorPagedResultTemplate template)
    {
        _template = template;
    }

    public IResolvedTypeInfo GetType(ITypeReference typeInfo)
    {
        const string cursorPagedResultTypeDefinitionId = "2a11c92d-d27f-4faa-b6fb-c33b93a6ff12";
        if (typeInfo?.Element?.Id == cursorPagedResultTypeDefinitionId)
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