using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates.ApplicationSecurityConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationSecurityConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Identity.ApplicationSecurityConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationSecurityConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole("Security.Configuration");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ApplicationSecurityConfiguration")
                .OnBuild(file =>
                {
                    file.AddUsing("Microsoft.Extensions.DependencyInjection");
                    file.AddUsing("Microsoft.Extensions.Configuration");

                    var priClass = file.Classes.First();
                    priClass.Static();
                    priClass.AddMethod("IServiceCollection", "ConfigureApplicationSecurity", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", prop => prop.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement($"services.AddTransient<{this.GetCurrentUserServiceInterfaceName()}, {this.GetCurrentUserServiceName()}>();");
                        method.AddStatement("return services;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureApplicationSecurity", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseAuthentication")
                .WithPriority(-10));
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