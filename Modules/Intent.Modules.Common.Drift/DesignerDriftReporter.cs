using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.DriftReporting.Api;

namespace Intent.Modules.Common.Drift
{
    /// <summary>
    /// A drift reporter that contributes a designer to the drift check. It reports the designer's drift with
    /// <see cref="DriftCheckRunner"/>, and at the start of the run records the designer, so the drift module's rule and skills
    /// name every designer the run checks. The record is this module's own: Intent.Common.DriftReporting knows nothing of
    /// designers.
    /// </summary>
    public abstract class DesignerDriftReporter : DriftReporterBase
    {
        /// <summary>Each run's designers, by the run's event dispatcher, which is created for each run.</summary>
        private static readonly ConditionalWeakTable<IApplicationEventDispatcher, List<DriftDesigner>> Runs = new();

        public abstract DriftDesigner Designer { get; }

        protected sealed override void OnStart(IApplication application)
        {
            var designers = Runs.GetValue(application.EventDispatcher, _ => new List<DriftDesigner>());
            lock (designers)
            {
                designers.Add(Designer);
            }
        }

        protected sealed override void Report(IDriftContext context) => DriftCheckRunner.Report(context, Designer);

        /// <summary>
        /// Every designer the run's reporters contribute, in order, one per designer. Reporters record theirs at the start of
        /// the run, before any template is registered, so this is safe from a template's constructor onwards.
        /// </summary>
        internal static IReadOnlyList<DriftDesigner> Designers(ISoftwareFactoryExecutionContext context)
        {
            if (!Runs.TryGetValue(context.EventDispatcher, out var designers))
            {
                return new List<DriftDesigner>();
            }

            lock (designers)
            {
                return designers.DistinctBy(x => x.Name).OrderBy(x => x.Order).ToList();
            }
        }
    }
}
