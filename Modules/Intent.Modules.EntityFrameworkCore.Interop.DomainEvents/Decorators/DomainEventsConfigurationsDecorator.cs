using Intent.Modules.EntityFrameworkCore.Templates.Configurations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DomainEventsConfigurationsDecorator : ConfigurationsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsConfigurationsDecorator";

        private readonly ConfigurationsTemplate _template;

        public DomainEventsConfigurationsDecorator(ConfigurationsTemplate template)
        {
            _template = template;
        }

        public override string AfterAttributes()
        {
            return @"
            builder.Ignore(e => e.DomainEvents);
";
        }
    }
}