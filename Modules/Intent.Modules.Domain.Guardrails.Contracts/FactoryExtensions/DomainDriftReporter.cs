using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.FactoryExtensions
{
    /// <summary>
    /// Contributes the Domain designer to the drift check (<see cref="DomainDrift"/>): the shared check runs after template
    /// execution and adds its findings to the drift report's Domain Area. Merged rather than fully managed, so it keeps its
    /// base class.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class DomainDriftReporter : DesignerDriftReporter
    {
        public override string Id => "Intent.Domain.Guardrails.Contracts.DomainDriftReporter";

        [IntentManaged(Mode.Ignore)]
        public override int Order => DomainDrift.Designer.Order;

        [IntentManaged(Mode.Ignore)]
        public override DriftDesigner Designer => DomainDrift.Designer;
    }
}
