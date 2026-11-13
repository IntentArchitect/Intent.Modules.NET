using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.DriftEngine.Events;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Reporting;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Common.DriftEngine.FactoryExtensions
{
    /// <summary>
    /// Runs the drift engine after template execution (<see cref="DriftEngineReport"/>) and adds its findings to the drift
    /// report. At the run's start it subscribes to the modules' extensions (<see cref="Api.DriftEngineExtensionBase"/>),
    /// which the engine runs beside its stock ones. Merged rather than fully managed, so it keeps its base class.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class DriftEngineReporter : DriftReporterBase
    {
        public override string Id => "Intent.Common.DriftEngine.DriftEngineReporter";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>Each run's registered extensions, by the run's event dispatcher, which is created for each run.</summary>
        [IntentManaged(Mode.Ignore)]
        private readonly ConditionalWeakTable<IApplicationEventDispatcher, List<IDriftExtension>> _runs = new();

        [IntentManaged(Mode.Ignore)]
        protected override void OnStart(IApplication application)
        {
            var extensions = new List<IDriftExtension>();
            _runs.AddOrUpdate(application.EventDispatcher, extensions);
            application.EventDispatcher.Subscribe<DriftEngineExtensionRegistered>(DriftEngineExtensionRegistered.EventId, e => extensions.Add(e.Extension));
        }

        [IntentManaged(Mode.Ignore)]
        protected override void Report(IDriftContext context) =>
            DriftEngineReport.Report(context, _runs.TryGetValue(context.Application.EventDispatcher, out var extensions) ? extensions : new List<IDriftExtension>());
    }
}
