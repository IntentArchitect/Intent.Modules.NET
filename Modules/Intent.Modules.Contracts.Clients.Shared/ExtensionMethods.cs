using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Contracts.Clients.Shared;

public static class ExtensionMethods
{
    public static string GetPackageOnlyNamespace<T>(T model)
        where T : IHasFolder, IElementWrapper
    {
        var parts = model.InternalElement.Package.Name
            .Split('.')
            .Concat(GetParentFolders(model))
            .ToArray();
        var result = string.Join(".", parts);
        return result;
    }
    
    public static string GetPackageBasedRelativeLocation(this CSharpTemplateBase<EnumModel> template)
    {
        return GetPackageBasedRelativeLocation(template.Model, template.OutputTarget);
    }

    public static string GetPackageBasedRelativeLocation(this CSharpTemplateBase<DTOModel> template)
    {
        return GetPackageBasedRelativeLocation(template.Model, template.OutputTarget);
    }

    public static string GetPackageBasedNamespace(this CSharpTemplateBase<EnumModel> template)
    {
        return GetPackageBasedNamespace(template.Model, template.OutputTarget);
    }

    public static string GetPackageBasedNamespace(this CSharpTemplateBase<DTOModel> template)
    {
        return GetPackageBasedNamespace(template.Model, template.OutputTarget);
    }

    public static string GetPackageBasedRelativeLocation<T>(T model, IOutputTarget outputTarget)
        where T : IHasFolder
    {
        return string.Join('/', Enumerable.Empty<string>()
            .Concat(GetElementPackageParts(model, outputTarget))
            .Concat(GetParentFolders(model))
        );
    }

    public static string GetPackageBasedNamespace<T>(T model, IOutputTarget outputTarget)
        where T : IHasFolder
    {
        return string.Join('.', Enumerable.Empty<string>()
            .Concat(outputTarget.GetNamespace().Split('.'))
            .Concat(GetElementPackageParts(model, outputTarget))
            .Concat(GetParentFolders(model))
        );
    }

    private static IEnumerable<string> GetElementPackageParts<T>(T model, IOutputTarget outputTarget)
    {
        var element = model switch
        {
            EnumModel enumModel => enumModel.InternalElement,
            DTOModel dtoModel => dtoModel.InternalElement,
            _ => throw new InvalidOperationException()
        };

        var outputTargetParts = new Queue<string>(outputTarget.GetNamespace().Split('.'));
        var packageParts = new Queue<string>(element.Package.Name.Split("."));

        while (outputTargetParts.TryPeek(out var outputTargetPart) &&
               packageParts.TryPeek(out var packagePart) &&
               outputTargetPart == packagePart)
        {
            outputTargetParts.Dequeue();
            packageParts.Dequeue();
        }

        return packageParts.Select(x => x.ToCSharpIdentifier());
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