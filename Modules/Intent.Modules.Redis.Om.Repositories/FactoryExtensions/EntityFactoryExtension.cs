using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.DocumentDB.Shared;
using Intent.Modules.Redis.Om.Repositories.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Redis.Om.Repositories.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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
            EntityFactoryExtensionHelper.Execute(
                application: application,
                dbProviderApplies: RedisOmProvider.FilterDbProvider,
                primaryKeyInitStrategy: new RedisOmPrimaryKeyInitStrategy(),
                makeNonPersistentPropertiesVirtual: false);
        }

    }
}