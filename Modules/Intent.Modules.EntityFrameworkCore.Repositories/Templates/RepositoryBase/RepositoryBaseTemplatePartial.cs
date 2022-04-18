using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class RepositoryBaseTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.RepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryBaseTemplate(IOutputTarget outputTarget, object model = null)
            : base(TemplateId, outputTarget, model)
        {
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        public string PagedListClassName => GetTypeName(PagedListTemplate.TemplateId);

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"RepositoryBase",
                @namespace: $"{this.GetNamespace()}");
        }
    }
}
