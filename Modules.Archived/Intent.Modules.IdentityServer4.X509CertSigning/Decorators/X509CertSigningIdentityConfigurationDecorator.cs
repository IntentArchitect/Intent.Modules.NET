using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class X509CertSigningIdentityConfigurationDecorator : IdentityConfigurationDecorator, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.X509CertSigning.X509CertSigningIdentityConfigurationDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly IdentityServerConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public X509CertSigningIdentityConfigurationDecorator(IdentityServerConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override string ConfigureServices()
        {
            return "idServerBuilder.AddSigningCredential(CertificateRepo.GetUsingOptions(configuration));";
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new CertificateSpecifiedEvent());
        }
    }
}