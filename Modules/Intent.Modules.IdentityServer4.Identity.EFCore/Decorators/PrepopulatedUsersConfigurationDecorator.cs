using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using Intent.Modules.IdentityServer4.SecureTokenServer.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class PrepopulatedUsersConfigurationDecorator : StartupDecorator, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EFCore.PrepopulatedUsersConfigurationDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public PrepopulatedUsersConfigurationDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = 100;

            _template.CSharpFile.AfterBuild(file =>
            {
                if (!_application.Settings.GetIdentityServerSettings().CreateTestUsers())
                {
                    return;
                }
                
                var @class = file.Classes.First();
                
                file.Usings.Add(new CSharpUsing("Microsoft.AspNetCore.Identity"));
                file.Usings.Add(new CSharpUsing("System.Security.Claims"));

                @class.AddMethod("void", "CreateTestUsers", method =>
                {
                    method.Private()
                        .WithComments("// The creation of test users can be disabled in Intent Architect's application settings.")
                        .AddAttribute("IntentManaged(Mode.Fully, Body = Mode.Ignore)")
                        .AddParameter("IApplicationBuilder", "app")
                        .AddStatements(@"
using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
{
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    if (!userManager.Users.Any())
    {
        var identityUser = new IdentityUser(""admin"");
        userManager.CreateAsync(identityUser, ""P@ssw0rd"").Wait();
        userManager.AddClaimsAsync(identityUser, new[] 
            {
                new Claim(""role"", ""MyRole""), 
                new Claim(""__tenant__"", ""tenant1"") // e.g. if claim-strategy based Multitenancy is installed, set tenant identifier
            }).Wait();
    }
}");
                });
            });
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new PrepopulatedUsersSpecifiedEvent());
        }

        public override string Configuration()
        {
            if (!_application.Settings.GetIdentityServerSettings().CreateTestUsers())
            {
                return base.Configuration();
            }
            return "CreateTestUsers(app);";
        }
    }
}