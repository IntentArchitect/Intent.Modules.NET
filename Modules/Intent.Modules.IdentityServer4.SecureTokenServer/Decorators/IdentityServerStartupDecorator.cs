using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityServerStartupDecorator : StartupDecorator, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.IdentityServerStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public IdentityServerStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddTemplateDependency(IdentityServerConfigurationTemplate.TemplateId);
            Priority = -9;
        }

        public override string Configuration()
        {
            return "app.UseIdentityServer();";
        }

        public override string ConfigureServices()
        {
            return @"services.ConfigureIdentityServer(Configuration);";
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IdentityServer4
            };
        }
    }
}
