using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders
{
    public class OutputTargetFileNamespaceProvider : IFileNamespaceProvider
    {
        public string GetFileNamespace<TModel>(CSharpTemplateBase<TModel> template)
            where TModel: IElementWrapper, IHasFolder
        {
            return ExtensionMethods.GetPackageBasedNamespace(template.Model, template.OutputTarget);
        }

        public string GetFileLocation<TModel>(CSharpTemplateBase<TModel> template)
            where TModel: IElementWrapper, IHasFolder
        {
            return ExtensionMethods.GetPackageBasedRelativeLocation(template.Model, template.OutputTarget);
        }
    }
}