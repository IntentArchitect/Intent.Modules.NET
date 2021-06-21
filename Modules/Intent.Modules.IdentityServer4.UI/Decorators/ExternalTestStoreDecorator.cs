using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.UI.Events;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ExternalTestStoreDecorator : ExternalAuthProviderDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.ExternalTestStoreDecorator";

        private readonly ExternalControllerTemplate _template;
        private readonly IApplication _application;
        private bool _overridden;

        [IntentManaged(Mode.Merge)]
        public ExternalTestStoreDecorator(ExternalControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            application.EventDispatcher.Subscribe<OverrideExternalAuthProviderDecoratorEvent>(evt =>
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

        public override string GetAuthProviderUserType()
        {
            if (_overridden) { return string.Empty; }
            return "TestUser";
        }

        public override string GetAutoProvisionUserMethodCode()
        {
            if (_overridden) { return string.Empty; }
            return @"private Task<TestUser> AutoProvisionUser(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var user = _users.AutoProvisionUser(provider, providerUserId, claims.ToList());
            return Task.FromResult(user);
        }";
        }

        public override string GetUserMappingCode()
        {
            if (_overridden) { return string.Empty; }
            return @"var user_username = user.Username;
                    var user_subjectId = user.SubjectId;";
        }

        public override string GetUserLookupCodeExpression()
        {
            if (_overridden) { return string.Empty; }
            return "_users.FindByExternalProvider(provider, providerUserId)";
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (_overridden) { return new string[0]; }
            return new[] { "IdentityServer4.Test" };
        }
    }
}