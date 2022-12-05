using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.FluentValidation.Templates.ValidationProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValidationProviderTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Application.ServiceImplementations.FluentValidation.ValidationProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ValidationProvider",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}