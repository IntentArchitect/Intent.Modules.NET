using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Wolverine.Common.Templates.WolverineConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Wolverine.Common.WolverineConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.WolverineFx(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Wolverine")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass("WolverineConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("void", "Configure", method =>
                    {
                        method.Static();
                        method.AddParameter("WolverineOptions", "opts");
                        method.AddParameter("IConfiguration", "configuration");

                        // Contributed by Intent.Application.Wolverine and Intent.Eventing.Wolverine: each
                        // finds this template and adds its own private method here, plus one call
                        // statement into this method's body. This class deliberately owns no logic of
                        // its own beyond the anchor those contributions attach to.
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
