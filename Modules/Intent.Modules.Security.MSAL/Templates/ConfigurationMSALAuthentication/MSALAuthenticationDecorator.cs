using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Templates.ConfigurationMSALAuthentication
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class MSALAuthenticationDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public abstract string GetConfigurationCode();

    }
}