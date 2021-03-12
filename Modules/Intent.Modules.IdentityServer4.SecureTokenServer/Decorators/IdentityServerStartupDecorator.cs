using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityServerStartupDecorator : StartupDecorator, IDeclareUsings, IHasNugetDependencies
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
            return @"var idServerBuilder = services.AddIdentityServer();
CustomIdentityServerConfiguration(idServerBuilder);";
        }

        public override string Configuration()
        {
            return "app.UseIdentityServer();";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "System.Security.Claims" };
        }

        public override string Methods()
        {
            return @"
[IntentManaged(Mode.Ignore)]
private void CustomIdentityServerConfiguration(IIdentityServerBuilder idServerBuilder)
{
    // Uncomment to have a test user handy
    /* idServerBuilder.AddTestUsers(new List<IdentityServer4.Test.TestUser>
    {
        new IdentityServer4.Test.TestUser
        {
            SubjectId = ""testuser"",
            Username = ""testuser"",
            Password = ""P@ssw0rd"",
            IsActive = true,
            Claims = new[] { new Claim(""role"", ""MyRole"") }
        }
    });*/
}";
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
