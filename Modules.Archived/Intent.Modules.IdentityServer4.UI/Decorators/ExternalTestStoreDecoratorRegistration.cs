using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [Description(ExternalTestStoreDecorator.DecoratorId)]
    public class ExternalTestStoreDecoratorRegistration : DecoratorRegistration<ExternalControllerTemplate, ExternalAuthProviderDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override ExternalAuthProviderDecorator CreateDecoratorInstance(ExternalControllerTemplate template, IApplication application)
        {
            return new ExternalTestStoreDecorator(template, application);
        }

        public override string DecoratorId => ExternalTestStoreDecorator.DecoratorId;
    }
}