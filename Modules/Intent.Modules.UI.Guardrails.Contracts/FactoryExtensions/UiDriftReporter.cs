using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Plugins;
using Intent.Modules.UI.Guardrails.Contracts.Drift;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.UI.Guardrails.Contracts.FactoryExtensions
{
    /// <summary>
    /// Contributes the User Interface designer to the drift check (<see cref="UiDrift"/>): the shared check runs after template
    /// execution and adds its findings to the drift report's UI Area. Merged rather than fully managed, so it keeps its
    /// base class.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class UiDriftReporter : DesignerDriftReporter
    {
        public override string Id => "Intent.UI.Guardrails.Contracts.UiDriftReporter";

        [IntentManaged(Mode.Ignore)]
        public override int Order => UiDrift.Designer.Order;

        [IntentManaged(Mode.Ignore)]
        public override DriftDesigner Designer => UiDrift.Designer;
    }
}
