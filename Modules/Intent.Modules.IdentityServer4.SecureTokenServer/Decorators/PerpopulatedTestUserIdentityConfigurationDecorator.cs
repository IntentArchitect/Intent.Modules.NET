using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class PerpopulatedTestUserIdentityConfigurationDecorator : IdentityConfigurationDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.PerpopulatedTestUserIdentityConfigurationDecorator";

        private readonly IdentityServerConfigurationTemplate _template;
        private readonly IApplication _application;
        private bool _prepopulatedUsersSpecified;

        [IntentManaged(Mode.Merge)]
        public PerpopulatedTestUserIdentityConfigurationDecorator(IdentityServerConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            application.EventDispatcher.Subscribe<PrepopulatedUsersSpecifiedEvent>(evt => Handle(evt));
        }

        private void Handle(PrepopulatedUsersSpecifiedEvent evt)
        {
            _prepopulatedUsersSpecified = true;
        }

        public override string ConfigureServices()
        {
            if (!_prepopulatedUsersSpecified)
            {
                return "idServerBuilder.PrepopulateWithTestUsers();";
            }
            return string.Empty;
        }

        public override string Methods()
        {
            return @"
[IntentManaged(Mode.Ignore)]
private static void PrepopulateWithTestUsers(this IIdentityServerBuilder idServerBuilder)
{
    idServerBuilder.AddTestUsers(new List<IdentityServer4.Test.TestUser>
    {
        new IdentityServer4.Test.TestUser
        {
            SubjectId = ""admin"",
            Username = ""admin"",
            Password = ""P@ssw0rd"",
            IsActive = true,
            Claims = new[] { new Claim(""role"", ""MyRole"") }
        }
    });
}";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "System.Security.Claims" };
        }
    }
}