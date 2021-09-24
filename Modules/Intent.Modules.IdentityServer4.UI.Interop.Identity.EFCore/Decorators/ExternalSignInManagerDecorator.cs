using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.UI.Events;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Interop.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ExternalSignInManagerDecorator : ExternalAuthProviderDecorator, IDecoratorExecutionHooks, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.Interop.Identity.EFCore.ExternalSignInManagerDecorator";

        private readonly ExternalControllerTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Fully)]
        public ExternalSignInManagerDecorator(ExternalControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override string GetAuthProviderVariableName()
        {
            return "userManager";
        }

        public override string GetAuthProviderType()
        {
            return "UserManager<IdentityUser>";
        }

        public override string GetAuthProviderUserType()
        {
            return "IdentityUser";
        }

        public override string GetAutoProvisionUserMethodCode()
        {
            return @"private async Task<IdentityUser> AutoProvisionUser(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            // create dummy internal account (you can do something more complex)
            var user = new IdentityUser(Guid.NewGuid().ToString());
            await _userManager.CreateAsync(user);

            // add external user ID to new account
            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            return user;
        }";
        }

        public override string GetUserLookupCodeExpression()
        {
            return "await _userManager.FindByLoginAsync(provider, providerUserId)";
        }

        public override string GetUserMappingCode()
        {
            return @"var user_username = user.UserName;
                    var user_subjectId = user.Id;";
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new OverrideExternalAuthProviderDecoratorEvent());
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "Microsoft.AspNetCore.Identity" };
        }
    }
}