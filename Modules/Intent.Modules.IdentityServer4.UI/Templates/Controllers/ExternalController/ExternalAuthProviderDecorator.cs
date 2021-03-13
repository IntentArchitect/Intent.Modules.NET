using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ExternalAuthProviderDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public abstract string GetAuthProviderVariableName();

        public abstract string GetAuthProviderType();

        public abstract string GetAuthProviderUserType();

        public abstract string GetAutoProvisionUserMethodCode();

        public abstract string GetUserLookupCodeExpression();
    }
}