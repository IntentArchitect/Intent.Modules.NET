using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.UI.Events;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.AccountController;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Interop.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AccountSignInManagerDecorator : AccountAuthProviderDecorator, IDecoratorExecutionHooks, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.Interop.Identity.EFCore.AccountSignInManagerDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AccountControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Fully)]
        public AccountSignInManagerDecorator(AccountControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override string GetAuthProviderVariableName()
        {
            return "signInManager";
        }

        public override string GetAuthProviderType()
        {
            return "SignInManager<IdentityUser>";
        }

        public override string GetPreAuthenticationCode()
        {
            return "var user = await _signInManager.UserManager.FindByNameAsync(model.Username);";
        }

        public override string GetAuthenticationCheckCodeExpression()
        {
            return "user != null && (await _signInManager.CheckPasswordSignInAsync(user, model.Password, true)) == Microsoft.AspNetCore.Identity.SignInResult.Success";
        }

        public override string GetUserMappingCode()
        {
            return @"var user_username = user.UserName;
                    var user_subjectId = user.Id;
                    var user_name = user.UserName;";
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new OverrideAccountAuthProviderDecoratorEvent());
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "Microsoft.AspNetCore.Identity" };
        }
    }
}