using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .OnBuild(c =>
                {
                    AddNugetDependency(NugetPackages.SwashbuckleAspNetCore(outputTarget));
                    AddNugetDependency(NugetPackages.MassTransit(outputTarget));
                })
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

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            // This block is because version 8.5.0 of MassTransit is only compatible with EF9 (not EF8). 
            // With Modular Monolith, EF is installed in the Modules, so we cannot know if EF is being installed nor which version.
            // We are making the assumption that if the Framework is NET8, then EF8 will be used and thus MT 8.4.1 must be used
            if (!OutputTarget.Parent.TargetFramework().Contains("9.0"))
            { 
                NugetRegistry.Register("MassTransit.Abstractions", v => new PackageVersion("8.4.1", true));
                NugetRegistry.Register(NugetPackages.MassTransitPackageName, v => new PackageVersion("8.4.1", true)
                        .WithNugetDependency("MassTransit.Abstractions", "8.4.1")
                        .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                        .WithNugetDependency("Microsoft.Extensions.Diagnostics.HealthChecks", "8.0.0")
                        .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")
                        .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                        .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0"));
            }
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