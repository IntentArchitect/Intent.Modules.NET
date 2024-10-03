using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HangfireDashboardAuthFilterTemplate : CSharpTemplateBase<IList<HangfireConfigurationModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.Hangfire.HangfireDashboardAuthFilter";

        private readonly IList<HangfireConfigurationModel> _model;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HangfireDashboardAuthFilterTemplate(IOutputTarget outputTarget, IList<HangfireConfigurationModel> model) : base(TemplateId, outputTarget, model)
        {
            _model = model;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Hangfire.Dashboard")
                .AddClass($"HangfireDashboardAuthFilter", @class =>
                {
                    @class.ImplementsInterface("IDashboardAuthorizationFilter");
                    @class.AddConstructor();
                    @class.AddMethod("bool", "Authorize", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Ignore());
                        method.AddParameter("DashboardContext", "context");

                        if (ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.Application.Identity"))
                        {
                            AddUsing("Microsoft.Extensions.DependencyInjection");

                            method.AddStatement($"var currentUser = context.GetHttpContext().RequestServices.GetRequiredService<{GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface")}>();");
                            method.AddStatement("return currentUser.UserId is not null;");
                        }
                        else
                        {
                            method.AddReturn(new CSharpStatement("false"));
                        }
                    });

                });
        }

        public override bool CanRunTemplate()
        {
            if (_model.Count == 0)
            {
                return false;
            }

            // showdashboard must be checked AND it must be a web application
            return _model.First().HasHangfireOptions() && _model.First().GetHangfireOptions().ShowDashboard()
                && ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.AspNetCore");
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