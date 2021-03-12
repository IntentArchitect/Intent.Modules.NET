using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.VisualStudio;
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

        public IdentityServerStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -9;
        }

        public override string ConfigureServices()
        {
            return @"var idServerBuilder = services.AddIdentityServer();";
        }

        public override string Configuration()
        {
            return "app.UseIdentityServer();";
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
