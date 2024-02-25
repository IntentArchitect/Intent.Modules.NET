using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Contracts.Clients.Shared
{
    public interface IFileNamespaceProvider
    {
        string GetFileNamespace<TModel>(CSharpTemplateBase<TModel> template) where TModel: IElementWrapper, IHasFolder;
        string GetFileLocation<TModel>(CSharpTemplateBase<TModel> template) where TModel: IElementWrapper, IHasFolder;
    }
}