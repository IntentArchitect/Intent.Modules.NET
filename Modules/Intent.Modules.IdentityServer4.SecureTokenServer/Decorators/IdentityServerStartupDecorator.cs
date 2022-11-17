using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityServerStartupDecorator : StartupDecorator, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.IdentityServerStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public IdentityServerStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -9;
            _template.AddTemplateDependency(IdentityServerConfigurationTemplate.TemplateId);
            var cp = _template.GetTemplate<IClassProvider>(IdentityServerConfigurationTemplate.TemplateId);
            if (cp != null)
            {
                _template.AddUsing(cp.Namespace);
            }
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
