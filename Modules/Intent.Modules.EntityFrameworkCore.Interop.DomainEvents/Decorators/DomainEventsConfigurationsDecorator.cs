using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainEventsConfigurationsDecorator : EntityTypeConfigurationDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsConfigurationsDecorator";

        private readonly EntityTypeConfigurationTemplate _template;

        public DomainEventsConfigurationsDecorator(EntityTypeConfigurationTemplate template)
        {
            _template = template;
        }

        public override string AfterAttributes()
        {
            if (_template.Model.ParentClass != null && (!_template.Model.ParentClass.IsAbstract || !_template.ExecutionContext.Settings.GetEntityFrameworkCoreSettings().InheritanceStrategy().IsTablePerConcreteType()))
            {
                return null;
            }
            return @"
            builder.Ignore(e => e.DomainEvents);
";
        }
    }
}