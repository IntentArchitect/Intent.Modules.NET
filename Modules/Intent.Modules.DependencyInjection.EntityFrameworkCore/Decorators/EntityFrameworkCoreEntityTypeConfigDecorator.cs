using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreEntityTypeConfigDecorator : EntityTypeConfigurationDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.DependencyInjection.EntityFrameworkCore.EntityFrameworkCoreEntityTypeConfigDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly EntityTypeConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public EntityFrameworkCoreEntityTypeConfigDecorator(EntityTypeConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> GetClassMembers()
        {
            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                yield break;
            }

            var useExplicitNullSymbol = _template.Project.GetProject().NullableEnabled;
            yield return $"private readonly string{(useExplicitNullSymbol ? "?" : "")} _partitionKey;";
        }

        public override IEnumerable<string> GetConstructorParameters()
        {
            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                yield break;
            }

            var useExplicitNullSymbol = _template.Project.GetProject().NullableEnabled;
            yield return $"string{(useExplicitNullSymbol ? "?" : "")} partitionKey";
        }

        public override IEnumerable<string> GetConstructorBodyStatements()
        {
            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                yield break;
            }

            yield return "_partitionKey = partitionKey;";
        }

        public override IEnumerable<string> AfterAttributeStatements()
        {
            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                yield break;
            }

            yield return $"builder.HasPartitionKey(_partitionKey);";
        }
    }
}