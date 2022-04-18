using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class PagedListTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.PagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedListTemplate(IOutputTarget outputTarget, object model = null)
            : base(TemplateId, outputTarget, model)
        {
        }

        public string PagedResultInterfaceName => GetTypeName(PagedResultInterfaceTemplate.TemplateId);

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"PagedList",
                @namespace: $"{this.GetNamespace()}");
        }
    }
}
