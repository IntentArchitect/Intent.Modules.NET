using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OutboxTableSpecificationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.EntityFrameworkCore.OutboxTableSpecificationExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!IsTransactionalOutboxPatternAndDatabaseProviderSelected(application))
            {
                return;
            }

            var dbContextTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext);
            if (dbContextTemplate is null)
            {
                return;
            }

            dbContextTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var onModelCreatingMethod = @class.Methods.Single(x => x.Name.Equals("OnModelCreating"));
                file.AddUsing("MassTransit");
                onModelCreatingMethod.AddStatement("modelBuilder.AddInboxStateEntity();", stmt => stmt.SeparatedFromPrevious());
                onModelCreatingMethod.AddStatement("modelBuilder.AddOutboxMessageEntity();");
                onModelCreatingMethod.AddStatement("modelBuilder.AddOutboxStateEntity();");
            });
        }

        private static bool IsTransactionalOutboxPatternAndDatabaseProviderSelected(IApplication application)
        {
            return application.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework() &&
                   (application.Settings.GetDatabaseSettings().DatabaseProvider().IsSqlServer() ||
                    application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql() ||
                    application.Settings.GetDatabaseSettings().DatabaseProvider().IsMySql());
        }
    }
}