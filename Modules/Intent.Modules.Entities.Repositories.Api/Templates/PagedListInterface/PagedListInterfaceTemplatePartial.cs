using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates.PagedListInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class PagedListInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.PagedListInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedListInterfaceTemplate(IOutputTarget outputTarget, object model = null)
            : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Repository.Interface.PagedResult);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IPagedList",
                @namespace: $"{this.GetNamespace()}");
        }
    }
}
