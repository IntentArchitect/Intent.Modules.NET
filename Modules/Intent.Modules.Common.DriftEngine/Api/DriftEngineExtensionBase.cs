using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.DriftEngine.Events;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.DriftEngine.Api
{
    /// <summary>
    /// The base of a module's drift extension: a factory extension that adds its packs, code readers, claim readers and
    /// comparers to the run's drift engine. Override what the module needs; each part is optional.
    /// </summary>
    public abstract class DriftEngineExtensionBase : FactoryExtensionBase, IDriftExtension
    {
        public virtual IReadOnlyDictionary<string, string> Designers => new Dictionary<string, string>();

        public virtual IEnumerable<KindDefinition> Kinds => Enumerable.Empty<KindDefinition>();

        public virtual IEnumerable<IClaimSource> ClaimSources => Enumerable.Empty<IClaimSource>();

        public virtual IEnumerable<Pack> Packs => Enumerable.Empty<Pack>();

        public virtual IEnumerable<ICodeReader> Readers => Enumerable.Empty<ICodeReader>();

        public virtual IEnumerable<IClaimReader> ClaimReaders => Enumerable.Empty<IClaimReader>();

        public virtual IEnumerable<IMemberRefiner> Refiners => Enumerable.Empty<IMemberRefiner>();

    public virtual IEnumerable<IOptInRule> OptInRules => Enumerable.Empty<IOptInRule>();

        public virtual IEnumerable<IValueComparer> Comparers => Enumerable.Empty<IValueComparer>();

        public virtual IEnumerable<IDriftRule> Rules => Enumerable.Empty<IDriftRule>();

        /// <summary>
        /// The application being run, for a claim source or reader that reads the model the module's own way, as its own
        /// generated designer API or model reader does. Set before the engine runs.
        /// </summary>
        protected IApplication Application { get; private set; } = null!;

        /// <summary>
        /// Publishes the extension on the run's event dispatcher. The engine subscribes at the run's start, before any
        /// extension's registration step, so the order of the extensions doesn't matter.
        /// </summary>
        protected sealed override void OnBeforeTemplateRegistrations(IApplication application)
        {
            Application = application;
            application.EventDispatcher.Publish(DriftEngineExtensionRegistered.EventId, new DriftEngineExtensionRegistered(this));
        }
    }
}
