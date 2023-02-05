using System;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    [IntentManaged(Mode.Merge)]
    public abstract class AppSettingsDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public abstract void UpdateSettings(AppSettingsEditor appSettings);
    }
}