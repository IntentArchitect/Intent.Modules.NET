using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValidationServiceInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Application.FluentValidation.Dtos.ValidationServiceInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IValidationService",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}