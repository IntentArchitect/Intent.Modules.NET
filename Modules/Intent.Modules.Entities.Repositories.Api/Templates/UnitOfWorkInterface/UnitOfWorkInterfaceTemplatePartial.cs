using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates.UnitOfWorkInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class UnitOfWorkInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.UnitOfWorkInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UnitOfWorkInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IUnitOfWork",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}