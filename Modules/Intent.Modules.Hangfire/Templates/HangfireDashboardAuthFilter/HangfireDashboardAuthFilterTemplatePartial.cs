using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Newtonsoft.Json.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HangfireDashboardAuthFilterTemplate : CSharpTemplateBase<IList<HangfireConfigurationModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.Hangfire.HangfireDashboardAuthFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HangfireDashboardAuthFilterTemplate(IOutputTarget outputTarget, IList<HangfireConfigurationModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Hangfire.Dashboard")
                .AddClass($"HangfireDashboardAuthFilter", @class =>
                {
                    @class.ImplementsInterface("IDashboardAuthorizationFilter");
                    @class.AddConstructor(constuctor =>
                        constuctor.AddAttribute(CSharpIntentManagedAttribute.Ignore())
                    );
                    @class.AddMethod("bool", "Authorize", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Ignore());
                        method.AddParameter("DashboardContext", "context");
                        method.AddStatement("// Add custom implementation here to filter dashboard access");
                        method.AddReturn(new CSharpStatement("true"));
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