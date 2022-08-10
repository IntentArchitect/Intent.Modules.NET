using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.CosmosDb.Api;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.CosmosDb.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreEntityTypeConfigDecorator : EntityTypeConfigurationDecorator
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.EntityFrameworkCore.CosmosDb.EntityFrameworkCoreEntityTypeConfigDecorator";

        [IntentManaged(Mode.Fully)] private readonly EntityTypeConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public EntityFrameworkCoreEntityTypeConfigDecorator(EntityTypeConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> AfterAttributeStatements()
        {
            if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                yield break;
            }

            foreach (var key in _template.Model.Attributes.Where(p => p.HasPartitionKey()))
            {
                yield return $"builder.HasPartitionKey(x => x.{key.Name.ToPascalCase()});";
            }
        }
    }
}