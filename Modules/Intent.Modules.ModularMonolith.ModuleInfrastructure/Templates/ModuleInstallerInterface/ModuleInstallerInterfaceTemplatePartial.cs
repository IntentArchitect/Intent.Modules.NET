using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.ModularMonolith.ModuleInfrastructure.Templates.ModuleInstallerInterface
{
    //Manually Added
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleInstallerInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentIgnore]
        public const string TemplateId = "Intent.ModularMonolith.Host.ModuleInstallerInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleInstallerInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IModuleInstaller", @interface =>
                {
                    CSharpFile.AddUsing("Microsoft.Extensions.Configuration");
                    CSharpFile.AddUsing("Microsoft.Extensions.DependencyInjection");
                    @interface.AddMethod("void", "ConfigureContainer", method =>
                    {
                        method.AddParameter("IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");
                    });
                    CSharpFile.AddUsing("Swashbuckle.AspNetCore.SwaggerGen");
                    @interface.AddMethod("void", "ConfigureSwagger", method =>
                    {
                        method.AddParameter("SwaggerGenOptions", "options");
                    });
                    CSharpFile.AddUsing("MassTransit");
                    @interface.AddMethod("void", "ConfigureIntegrationEventConsumers", method =>
                    {
                        method.AddParameter("IRegistrationConfigurator", "cfg");
                    });
                });
            //Need for Discoverability (Could make a common module but feels heavy for 1 template)
            CanRun = false;
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