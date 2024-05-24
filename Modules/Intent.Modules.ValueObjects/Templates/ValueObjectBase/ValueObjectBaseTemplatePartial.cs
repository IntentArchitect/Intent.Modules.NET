using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.ValueObjects.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObjectBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValueObjectBaseTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.ValueObjects.ValueObjectBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValueObjectBaseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ValueObject",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetValueObjectSettings().ValueObjectType().IsClass();
        }
    }
}