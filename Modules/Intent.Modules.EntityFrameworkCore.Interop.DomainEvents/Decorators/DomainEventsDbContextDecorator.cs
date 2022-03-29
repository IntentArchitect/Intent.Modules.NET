using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DomainEventsDbContextDecorator : DbContextDecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsDbContextDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly DbContextTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public DomainEventsDbContextDecorator(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> GetPrivateFields()
        {
            yield return $"private readonly {_template.GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId)} _domainEventService;";
        }

        public override IEnumerable<string> GetConstructorParameters()
        {
            yield return $"{_template.GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId)} domainEventService";
        }

        public override IEnumerable<string> GetConstructorInitializations()
        {
            yield return $"_domainEventService = domainEventService;";
        }

        public override string BeforeCallToSaveChangesAsync()
        {
            return @"await DispatchEvents();";
        }

        public override IEnumerable<string> GetMethods()
        {
            yield return $@"
        private async Task DispatchEvents()
        {{
            while (true)
            {{
                var domainEventEntity = ChangeTracker
                    .Entries<{_template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId)}>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }}
        }}";
        }
    }
}