using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityCustomStartupDecorator : StartupDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.IdentityCustomStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        public IdentityCustomStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -7;
        }

        public override string ConfigureServices()
        {
            return "CustomIdentityServerConfiguration(idServerBuilder);";
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

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "System.Security.Claims" };
        }
    }
}