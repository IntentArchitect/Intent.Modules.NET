using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class StartupTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IAppStartupTemplate
    {
        private readonly IAppStartupFile _startupFile;

        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Startup";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Options")

                .AddClass("Startup", @class =>
                {
                    @class.AddAttribute("[IntentManaged(Mode.Merge)]");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration",
                            param => param.IntroduceProperty(prop => { prop.ReadOnly(); }));
                    });
                    @class.AddMethod("void", "ConfigureServices", method =>
                    {
                        method.AddParameter("IServiceCollection", "services");
                    });
                    @class.AddMethod("void", "Configure", method =>
                    {
                        method.WithComments("// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.");

                        method.AddParameter("IApplicationBuilder", "app");
                        method.AddParameter("IWebHostEnvironment", "env");

                        method.AddIfStatement("env.IsDevelopment()", s => s
                            .AddStatement("app.UseDeveloperExceptionPage();"));
                    });
                }, int.MinValue);

            if (!UseMinimalHostingModel)
            {
                _startupFile = new AppStartupFile(this);
            }
        }

        public IAppStartupFile StartupFile =>
            _startupFile ?? throw new InvalidOperationException(
                $"Based on options chosen in the Visual Studio designer, \"{TemplateId}\" " +
                $"is not responsible for app startup, ensure that you resolve the template with " +
                $"the role \"{IAppStartupTemplate.RoleName}\" to get the correct template.");

        public override bool CanRunTemplate()
        {
            return !UseMinimalHostingModel && base.CanRunTemplate();
        }

        private bool UseMinimalHostingModel => OutputTarget.GetProject().InternalElement.AsCSharpProjectNETModel()?.GetNETSettings()?.UseMinimalHostingModel() == true;

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "Startup",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}