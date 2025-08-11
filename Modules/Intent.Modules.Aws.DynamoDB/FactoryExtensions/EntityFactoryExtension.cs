using Intent.Engine;
using Intent.Modules.Aws.DynamoDB.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.DocumentDB.Shared;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.DynamoDB.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            EntityFactoryExtensionHelper.Execute(
                application: application,
                dbProviderApplies: DynamoDBProvider.FilterDbProvider,
                primaryKeyInitStrategy: new DynamoDbPrimaryKeyInitStrategy(),
                makeNonPersistentPropertiesVirtual: false);
        }
    }
}