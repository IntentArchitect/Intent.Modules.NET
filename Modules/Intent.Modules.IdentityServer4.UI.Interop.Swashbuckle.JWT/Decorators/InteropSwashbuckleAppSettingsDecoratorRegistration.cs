using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Interop.Swashbuckle.JWT.Decorators
{
    [Description(InteropSwashbuckleAppSettingsDecorator.DecoratorId)]
    public class InteropSwashbuckleAppSettingsDecoratorRegistration : DecoratorRegistration<AppSettingsTemplate, AppSettingsDecorator>
    {
        public override AppSettingsDecorator CreateDecoratorInstance(AppSettingsTemplate template, IApplication application)
        {
            return new InteropSwashbuckleAppSettingsDecorator(template, application);
        }

        public override string DecoratorId => InteropSwashbuckleAppSettingsDecorator.DecoratorId;
    }
}