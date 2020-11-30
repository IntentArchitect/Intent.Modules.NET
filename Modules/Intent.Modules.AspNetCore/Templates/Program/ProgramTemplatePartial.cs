using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ProgramTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.AspNetCore.Program";

        public ProgramTemplate(IOutputTarget project)
            : base(Identifier, project, null)
        {
        }

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
