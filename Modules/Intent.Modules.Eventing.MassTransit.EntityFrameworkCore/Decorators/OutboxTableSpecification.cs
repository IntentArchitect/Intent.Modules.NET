using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class OutboxTableSpecification : DbContextDecoratorBase
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.Eventing.MassTransit.EntityFrameworkCore.OutboxTableSpecification";

        [IntentManaged(Mode.Fully)] private readonly DbContextTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public OutboxTableSpecification(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override void DecorateDbContext(CSharpClass @class)
        {
            if (!_application.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework() ||
                (!_application.Settings.GetDatabaseSettings().DatabaseProvider().IsSqlServer() &&
                 !_application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql()))
            {
                return;
            }
            _template.AddUsing("MassTransit");

            var onModelCreatingMethod = @class.Methods.Single(x => x.Name.Equals("OnModelCreating"));
            onModelCreatingMethod.AddStatement(""); // GCB - new line (not sure if this is the best way).
            onModelCreatingMethod.AddStatement("modelBuilder.AddInboxStateEntity();");
            onModelCreatingMethod.AddStatement("modelBuilder.AddOutboxMessageEntity();");
            onModelCreatingMethod.AddStatement("modelBuilder.AddOutboxStateEntity();");
        }

        //public override IEnumerable<string> GetOnModelCreatingStatements()
        //{
        //    if (!_application.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework() ||
        //        (!_application.Settings.GetDatabaseSettings().DatabaseProvider().IsSqlServer() &&
        //        !_application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql()))
        //    {
        //        yield break;
        //    }

        //    _template.AddUsing("MassTransit");

        //    yield return @$"";
        //    yield return @$"modelBuilder.AddInboxStateEntity();";
        //    yield return @$"modelBuilder.AddOutboxMessageEntity();";
        //    yield return @$"modelBuilder.AddOutboxStateEntity();";
        //}
    }
}