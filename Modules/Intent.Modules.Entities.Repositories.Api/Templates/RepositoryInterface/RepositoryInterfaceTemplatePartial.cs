using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class RepositoryInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.RepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryInterfaceTemplate(IOutputTarget outputTarget, object model = null)
            : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IRepository",
                @namespace: $"{this.GetNamespace()}");
        }
    }
}
