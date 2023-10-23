using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;

public class PagedResultTypeSource : ITypeSource
{
    private readonly PagedResultTemplateBase _template;

    public PagedResultTypeSource(PagedResultTemplateBase template)
    {
        _template = template;
    }

    public static void ApplyTo<T>(CSharpTemplateBase<T> template, string pagedResultTemplateId)
    {
        var pagedResultTemplate = template.ExecutionContext.FindTemplateInstance<PagedResultTemplateBase>(pagedResultTemplateId);
        template.AddTypeSource(new PagedResultTypeSource(pagedResultTemplate));
    }

    public IResolvedTypeInfo GetType(ITypeReference typeInfo)
    {
        if (typeInfo?.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId)
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

    public IEnumerable<ITemplateDependency> GetTemplateDependencies() => Enumerable.Empty<ITemplateDependency>();
    public ICollectionFormatter CollectionFormatter => null!;
    public INullableFormatter NullableFormatter => null!;
}