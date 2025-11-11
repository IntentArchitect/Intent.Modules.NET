using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using Intent.Modules.IdentityServer4.SecureTokenServer.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PrepopulatedUsersConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IdentityServer4.Identity.EFCore.PrepopulatedUsersConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetIdentityServerSettings().CreateTestUsers())
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Distribution.WebApi.Startup);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                file.Usings.Add(new CSharpUsing("Microsoft.AspNetCore.Identity"));
                file.Usings.Add(new CSharpUsing("System.Security.Claims"));

                @class.FindMethod("Configure").AddStatement("CreateTestUsers(app);");

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
            }, 100);
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.EventDispatcher.Publish(new PrepopulatedUsersSpecifiedEvent());
        }
    }
}