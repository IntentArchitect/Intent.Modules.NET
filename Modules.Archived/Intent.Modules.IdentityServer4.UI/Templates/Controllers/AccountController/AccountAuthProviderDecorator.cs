using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Templates.Controllers.AccountController
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class AccountAuthProviderDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public abstract string GetAuthProviderVariableName();

        public abstract string GetAuthProviderType();

        public virtual string GetPreAuthenticationCode()
        {
            return string.Empty;
        }

        public abstract string GetAuthenticationCheckCodeExpression();

        /// <summary>
        /// Your code needs to ultimately generate and populate 3 variables:
        /// * user_username
        /// * user_subjectId
        /// * user_name
        /// </summary>
        public abstract string GetUserMappingCode();
    }
}