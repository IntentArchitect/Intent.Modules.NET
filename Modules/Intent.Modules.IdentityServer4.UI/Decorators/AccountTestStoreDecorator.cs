using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.UI.Events;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.AccountController;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AccountTestStoreDecorator : AccountAuthProviderDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.AccountTestStoreDecorator";

        private readonly AccountControllerTemplate _template;
        private readonly IApplication _application;
        private bool _overridden;

        [IntentManaged(Mode.Merge)]
        public AccountTestStoreDecorator(AccountControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            application.EventDispatcher.Subscribe<OverrideAccountAuthProviderDecoratorEvent>(evt =>
            {
                _overridden = true;
            });
        }

        public override string GetAuthProviderVariableName()
        {
            if (_overridden) { return string.Empty; }
            return "users";
        }

        public override string GetAuthProviderType()
        {
            if (_overridden) { return string.Empty; }
            return "TestUserStore";
        }

        public override string GetPreAuthenticationCode()
        {
            if (_overridden) { return string.Empty; }
            return "// validate username/password against in-memory store";
        }

        public override string GetAuthenticationCheckCodeExpression()
        {
            if (_overridden) { return string.Empty; }
            return "_users.ValidateCredentials(model.Username, model.Password)";
        }

        public override string GetUserMappingCode()
        {
            if (_overridden) { return string.Empty; }
            return @"var user = _users.FindByUsername(model.Username);
                    var user_username = user.Username;
                    var user_subjectId = user.SubjectId;
                    var user_name = user.Username;";
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (_overridden) { return new string[0]; }
            return new[] { "IdentityServer4.Test" };
        }
    }
}