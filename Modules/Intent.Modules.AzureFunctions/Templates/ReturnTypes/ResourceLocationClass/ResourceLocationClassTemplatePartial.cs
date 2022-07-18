using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.ReturnTypes.ResourceLocationClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ResourceLocationClassTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AzureFunctions.ReturnTypes.ResourceLocationClass";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ResourceLocationClassTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            var templates = ExecutionContext
                .OutputTargets
                .SelectMany(s => s.TemplateInstances)
                .OfType<IntentTemplateBase>()
                .Where(p => p.HasTypeResolver())
                .ToArray();
            foreach (var template in templates)
            {
                template.AddTypeSource(new ResourceLocationTypeSource(this));
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ResourceLocation",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}