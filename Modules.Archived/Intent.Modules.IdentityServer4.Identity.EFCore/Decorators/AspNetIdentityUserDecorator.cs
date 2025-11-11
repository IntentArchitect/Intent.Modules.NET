using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.IdentityServer4.Identity.EFCore.FactoryExtensions;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AspNetIdentityUserDecorator : IdentityConfigurationDecorator, IDeclareUsings, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EFCore.AspNetIdentityUserDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly IdentityServerConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public AspNetIdentityUserDecorator(IdentityServerConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -7;
        }

        public override string ConfigureServices()
        {
            return $"idServerBuilder.AddAspNetIdentity<{_template.GetIdentityUserClass()}>();";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "IdentityServer4.AspNetIdentity",
                "Microsoft.AspNetCore.Identity"
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IdentityServer4AspNetIdentity,
                // IdentityServer4.EntityFramework.Storage is no longer in production and
                // has a hard dependency on Automapper 10, only way to resolve compilation
                // issue with newer Automapper is to actually install it on this project
                NugetPackages.Automapper
            };
        }
    }
}