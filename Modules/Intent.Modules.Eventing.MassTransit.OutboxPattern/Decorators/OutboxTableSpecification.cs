using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.Eventing.MassTransit.OutboxPattern.Settings;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.OutboxPattern.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class OutboxTableSpecification : DbContextDecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Eventing.MassTransit.OutboxPattern.OutboxTableSpecification";

        [IntentManaged(Mode.Fully)]
        private readonly DbContextTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public OutboxTableSpecification(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> GetOnModelCreatingStatements()
        {
            if (_application.Settings.GetEventingSettings().OutboxPersistence().IsInMemory() ||
                (!_application.Settings.GetDatabaseSettings().DatabaseProvider().IsSqlServer() &&
                 !_application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql()))
            {
                yield break;
            }

            _template.AddUsing("MassTransit");

            yield return @$"";
            yield return @$"modelBuilder.AddInboxStateEntity();";
            yield return @$"modelBuilder.AddOutboxMessageEntity();";
            yield return @$"modelBuilder.AddOutboxStateEntity();";
        }
    }
}