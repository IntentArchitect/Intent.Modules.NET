using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
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

        [IntentManaged(Mode.Fully)]
        private readonly EntityTypeConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public DomainEventsConfigurationsDecorator(EntityTypeConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override string AfterAttributes()
        {
            //if (_template.Model.ParentClass != null && (!_template.Model.ParentClass.IsAbstract || !_template.ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()))
            if (_template.Model.ParentClass?.IsAggregateRoot() == true || !_template.Model.IsAggregateRoot())
            {
                return null;
            }
            return @"
            builder.Ignore(e => e.DomainEvents);
";
        }
    }
}