using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ProgramTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.AspNetCore.Program";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Program",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Program";
    }
}
