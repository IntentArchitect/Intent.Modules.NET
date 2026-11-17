using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Services.Guardrails.Contracts.Drift;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.FactoryExtensions
{
    /// <summary>
    /// Contributes the Services designer to the drift check (<see cref="ServicesDrift"/>): the shared check runs after template
    /// execution and adds its findings to the drift report's Services Area. Merged rather than fully managed, so it keeps its
    /// base class.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class ServicesDriftReporter : DesignerDriftReporter
    {
        public override string Id => "Intent.Services.Guardrails.Contracts.ServicesDriftReporter";

        [IntentManaged(Mode.Ignore)]
        public override int Order => ServicesDrift.Designer.Order;

        [IntentManaged(Mode.Ignore)]
        public override DriftDesigner Designer => ServicesDrift.Designer;
    }
}
