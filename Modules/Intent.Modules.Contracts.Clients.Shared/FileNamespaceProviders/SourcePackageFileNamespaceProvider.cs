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
        return ExtensionMethods.GetPackageOnlyNamespace(template.Model);
    }

    public string GetFileLocation<TModel>(CSharpTemplateBase<TModel> template)
        where TModel : IElementWrapper, IHasFolder
    {
        return ExtensionMethods.GetPackageBasedRelativeLocation(template.Model, template.OutputTarget);
    }
}