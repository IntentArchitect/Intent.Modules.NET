using Intent.Modules.IdentityServer4.Selfhost.Templates.Startup;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class InMemoryStartupDecorator : StartupDecorator
    {
        public const string Identifier = "IdentityServer4.InMemoryStore.StartupDecorator";

        public InMemoryStartupDecorator(StartupTemplate template)
        {
            this.Priority = 1;
        }

        public override IReadOnlyCollection<string> GetServicesConfigurationStatements()
        {
            return new[]
            {
                "AddTestUsers(TestUsers.Users)"
            };
        }
    }
}
