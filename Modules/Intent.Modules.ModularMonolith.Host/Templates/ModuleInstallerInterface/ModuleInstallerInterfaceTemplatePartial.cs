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

namespace Intent.Modules.ModularMonolith.Host.Templates.ModuleInstallerInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleInstallerInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModularMonolith.Host.ModuleInstallerInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleInstallerInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.SwashbuckleAspNetCore(outputTarget));
            AddNugetDependency(NugetPackages.MassTransit(outputTarget));

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
                })
                .AddClass("IModuleInstallerExtensions", @class => 
                {
                    @class.Static();
                    @class.AddMethod("void", "ConfigureContainer", method =>
                    {
                        method.Static();
                        method.AddParameter($"IEnumerable<{this.GetModuleInstallerInterfaceName()}>", "installers", p => p.WithThisModifier());
                        method.AddParameter("IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("installers.ForEach(i => i.ConfigureContainer(services, configuration));");
                    });
                    @class.AddMethod("void", "ConfigureSwagger", method =>
                    {
                        method.Static();
                        method.AddParameter($"IEnumerable<{this.GetModuleInstallerInterfaceName()}>", "installers", p => p.WithThisModifier());
                        method.AddParameter("SwaggerGenOptions", "options");
                        method.AddStatement("installers.ForEach(i => i.ConfigureSwagger(options));");

                    });
                    @class.AddMethod("void", "ConfigureIntegrationEventConsumers", method =>
                    {
                        method.Static();
                        method.AddParameter($"IEnumerable<{this.GetModuleInstallerInterfaceName()}>", "installers", p => p.WithThisModifier());
                        method.AddParameter("IRegistrationConfigurator", "cfg");
                        method.AddStatement("installers.ForEach(i => i.ConfigureIntegrationEventConsumers(cfg));");
                    });
                    @class.AddMethod("void", "ForEach", method =>
                    {
                        method.Private().Static();
                        method.AddParameter($"IEnumerable<{this.GetModuleInstallerInterfaceName()}>", "installers", p => p.WithThisModifier());
                        method.AddParameter($"Action<{this.GetModuleInstallerInterfaceName()}>", "action");
                        method.AddForEachStatement("installer", "installers", x => x.AddStatement("action(installer);"));
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