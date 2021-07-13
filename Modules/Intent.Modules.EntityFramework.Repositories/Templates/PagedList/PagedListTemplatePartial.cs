using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.PagedList
{
    partial class PagedListTemplate : CSharpTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.PagedList";

        public PagedListTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget)
        {
        }

        public string PagedResultInterfaceName => GetTypeName(PagedResultInterfaceTemplate.Identifier);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "PagedList",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
