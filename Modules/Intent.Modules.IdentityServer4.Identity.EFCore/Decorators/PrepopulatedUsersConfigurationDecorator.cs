using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class PrepopulatedUsersConfigurationDecorator : StartupDecorator, IDecoratorExecutionHooks, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EFCore.PrepopulatedUsersConfigurationDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public PrepopulatedUsersConfigurationDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = 30;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new PrepopulatedUsersSpecifiedEvent());
        }

        public override string Configuration()
        {
            return "PrepopulateWithIdentityUsers(app);";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.AspNetCore.Identity";
            yield return "System.Security.Claims";
        }

        public override string Methods()
        {
            return @"
[IntentManaged(Mode.Ignore)]
private static void PrepopulateWithIdentityUsers(IApplicationBuilder app)
{
    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
    {
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        if (!userManager.Users.Any())
        {
            var identityUser = new IdentityUser(""admin"");
            userManager.CreateAsync(identityUser, ""P@ssw0rd"").Wait();
            userManager.AddClaimsAsync(identityUser, new Claim[] { new Claim(""role"", ""MyRole"") }).Wait();
        }
    }
}
";
        }
    }
}