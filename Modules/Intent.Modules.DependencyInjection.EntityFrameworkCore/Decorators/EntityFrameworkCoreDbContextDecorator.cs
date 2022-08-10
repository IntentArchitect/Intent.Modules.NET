using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityFrameworkCoreDbContextDecorator(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
            {
                template.AddUsing("Microsoft.Extensions.Options");
            }
        }

        public override IEnumerable<string> GetPrivateFields()
        {
            if (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
            {
                yield break;
            }

            yield return $"private readonly IOptions<{_template.GetDbContextConfigurationName()}> _dbContextConfig;";
        }

        public override IEnumerable<string> GetConstructorParameters()
        {
            if (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
            {
                yield break;
            }

            yield return $"IOptions<{_template.GetDbContextConfigurationName()}> dbContextConfig";
        }

        public override IEnumerable<string> GetConstructorInitializations()
        {
            if (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
            {
                yield break;
            }

            yield return "_dbContextConfig = dbContextConfig;";
        }

        public override IEnumerable<string> GetOnModelCreatingStatements()
        {
            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    const string schemaNameExpression = "_dbContextConfig.Value?.DefaultSchemaName";
                    yield return $"modelBuilder.HasDefaultSchema({_template.GetDbContextConfigHelperName()}.EmptyToNull({schemaNameExpression}));";
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    const string containerNameExpression = "_dbContextConfig.Value?.DefaultContainerName";
                    yield return $"modelBuilder.HasDefaultContainer({_template.GetDbContextConfigHelperName()}.EmptyToNull({containerNameExpression}));";
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                default:
                    break;
            }
        }

        public override IEnumerable<string> GetTypeConfigurationParameters(EntityTypeConfigurationCreatedEvent @event)
        {
            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                yield break;
            }

            const string partitionKeyExpression = "_dbContextConfig.Value?.PartitionKey";
            yield return $"{_template.GetDbContextConfigHelperName()}.EmptyToNull({partitionKeyExpression})";
        }

        public override IEnumerable<string> GetMethods()
        {
            if (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
            {
                yield break;
            }

            yield return @"
        /// <summary>
        /// If configured to do so, a check is performed to see
        /// whether the database exist and if not will create it
        /// based on this container configuration.
        /// </summary>
        public void EnsureDbCreated()
        {
            if (_dbContextConfig.Value.EnsureDbCreated == true)
            {
                Database.EnsureCreated();
            }
        }
";
        }
    }
}