using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class IdentityServerStartupDecorator : StartupDecorator, IDeclareUsings
    {
        public const string DecoratorId = "IdentityServer4.SecureTokenServer.IdentityServerStartupDecorator";

        public IdentityServerStartupDecorator()
        {
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

        
    }
}
