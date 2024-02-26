using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.AspNetCore.Logging.Serilog.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SerilogStartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.AspNetCore.Logging.Serilog.SerilogStartupConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        [IntentInitialGen]
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // Your custom logic here.
        }
    }
}