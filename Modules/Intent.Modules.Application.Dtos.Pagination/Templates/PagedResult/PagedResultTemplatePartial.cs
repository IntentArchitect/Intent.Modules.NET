using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.PagedResult
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class PagedResultTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.PagedResult";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedResultTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            // This will ensure that all template instances will know about our new type
            // and will resolve the namespace correctly. However, this is not the ideal
            // method and will be relooked in the future using some "Global Type Source"
            // idea.
            var templates = ExecutionContext
                .OutputTargets
                .SelectMany(s => s.TemplateInstances)
                .OfType<IntentTemplateBase>()
                .Where(p => p.HasTypeResolver())
                .ToArray();
            foreach (var template in templates)
            {
                template.AddTypeSource(new PagedResultTypeSource(this));
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"PagedResult",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}