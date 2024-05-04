using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MigrationsReadMeExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.DesignTimeDbContextFactory.MigrationsReadMeExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<DbMigrationsReadMeCreatedEvent>(SubscribeDbMigrationsReadMeCreatedEvent);
        }

        private static void SubscribeDbMigrationsReadMeCreatedEvent(DbMigrationsReadMeCreatedEvent @event)
        {
            @event.Template.IncludeDbContextArguments = true;
            @event.Template.IncludeStartupProjectArguments = false;
            @event.Template.ExtraArguments.Add("{ConnectionStringName}");
            @event.Template.ExtraComments = @"A separate ""appsettings.json"" file is used in this project for managing connection strings.";
        }
    }
}