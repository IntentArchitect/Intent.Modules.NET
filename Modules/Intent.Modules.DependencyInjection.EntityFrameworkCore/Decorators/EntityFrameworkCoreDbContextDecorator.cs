using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreDbContextDecorator : DbContextDecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.DependencyInjection.EntityFrameworkCore.EntityFrameworkCoreDbContextDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly DbContextTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public EntityFrameworkCoreDbContextDecorator(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> GetPrivateFields()
        {
            yield return $"private readonly IOptions<{_template.GetDbContextConfigurationName()}> _dbContextConfig;";
        }

        public override IEnumerable<string> GetConstructorParameters()
        {
            yield return $"IOptions<{_template.GetDbContextConfigurationName()}> dbContextConfig";
        }

        public override IEnumerable<string> GetConstructorInitializations()
        {
            yield return "_dbContextConfig = dbContextConfig;";
        }

        public override IEnumerable<string> GetOnModelCreatingStatements()
        {
            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.PostgreSQL:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SQLServer:
                    yield return "modelBuilder.HasDefaultSchema(_dbContextConfig.Value?.DefaultSchemaName);";
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.CosmosDB:
                    yield return "modelBuilder.HasDefaultContainer(_dbContextConfig.Value?.DefaultContainerName);";
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                default:
                    break;
            }
        }
    }
}