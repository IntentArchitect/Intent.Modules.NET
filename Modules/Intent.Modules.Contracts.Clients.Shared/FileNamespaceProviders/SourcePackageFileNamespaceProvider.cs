using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;

public class SourcePackageFileNamespaceProvider : IFileNamespaceProvider
{
    public string GetFileNamespace<TModel>(CSharpTemplateBase<TModel> template)
        where TModel : IElementWrapper, IHasFolder
    {
        var parts = template.Model.InternalElement.Package.Name
            .Split('.')
            .Concat(GetParentFolders(template.Model))
            .ToArray();
        var result = string.Join(".", parts);
        return result;
    }

    public string GetFileLocation<TModel>(CSharpTemplateBase<TModel> template)
        where TModel : IElementWrapper, IHasFolder
    {
        return ExtensionMethods.GetPackageBasedRelativeLocation(template.Model, template.OutputTarget);
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