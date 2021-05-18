using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class IdentityConfigurationDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual string ConfigureServices() => string.Empty;

        public virtual string Methods() => string.Empty;
    }
}