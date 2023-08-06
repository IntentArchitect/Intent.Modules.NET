using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Contracts.Clients.Shared;

public static class ExtensionMethods
{
    public static string GetPackageBasedRelativeLocation(this CSharpTemplateBase<EnumModel> template)
    {
        return GetPackageBasedRelativeLocation<EnumModel>(template);
    }

    public static string GetPackageBasedRelativeLocation(this CSharpTemplateBase<DTOModel> template)
    {
        return GetPackageBasedRelativeLocation<DTOModel>(template);
    }

    public static string GetPackageBasedNamespace(this CSharpTemplateBase<EnumModel> template)
    {
        return GetPackageBasedNamespace<EnumModel>(template);
    }

    public static string GetPackageBasedNamespace(this CSharpTemplateBase<DTOModel> template)
    {
        return GetPackageBasedNamespace<DTOModel>(template);
    }

    private static string GetPackageBasedRelativeLocation<T>(CSharpTemplateBase<T> template)
        where T : IHasFolder
    {
        return string.Join('/', Enumerable.Empty<string>()
            .Concat(GetElementPackageParts(GetElement(template)))
            .Concat(GetParentFolders(template.Model))
        );
    }

    private static string GetPackageBasedNamespace<T>(CSharpTemplateBase<T> template)
        where T : IHasFolder
    {
        return string.Join('.', Enumerable.Empty<string>()
            .Concat(template.OutputTarget.GetNamespace().Split('.'))
            .Concat(GetElementPackageParts(GetElement(template)))
            .Concat(GetParentFolders(template.Model))
        );
    }

    private static IElement GetElement<TModel>(CSharpTemplateBase<TModel> template) =>
        template.Model switch
        {
            EnumModel model => model.InternalElement,
            DTOModel model => model.InternalElement,
            _ => throw new InvalidOperationException()
        };

    private static IEnumerable<string> GetElementPackageParts(this IElement element)
    {
        return element.Package.Name.Split(".").Select(x => x.ToCSharpIdentifier());
    }

    private static IEnumerable<string> GetParentFolders(IHasFolder hasFolder) => hasFolder.GetParentFolders()
        .Where(x =>
        {
            if (string.IsNullOrWhiteSpace(x.Name))
            {
                return false;
            }

            return !x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider();
        })
        .Select(x => x.Name);

}