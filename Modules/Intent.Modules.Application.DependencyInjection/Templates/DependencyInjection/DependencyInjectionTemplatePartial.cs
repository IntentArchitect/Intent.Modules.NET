using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DependencyInjectionTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Application.DependencyInjection.DependencyInjection";

        public DependencyInjectionTemplate(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DependencyInjection",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

    }
}