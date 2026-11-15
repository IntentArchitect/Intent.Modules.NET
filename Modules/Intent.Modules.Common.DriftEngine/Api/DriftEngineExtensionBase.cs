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
    /// The base of a module's drift extension: a factory extension that adds its packs, languages, code readers, claim readers,
    /// rules and comparers to the run's drift engine. Override what the module needs; each part is optional. Every part of
    /// <see cref="IDriftExtension"/> is here to override: one that weren't would stay the interface's empty default, whatever
    /// the module wrote.
    /// </summary>
    public abstract class DriftEngineExtensionBase : FactoryExtensionBase, IDriftExtension
    {
        public virtual IReadOnlyDictionary<string, string> Designers => new Dictionary<string, string>();

        public virtual IEnumerable<KindDefinition> Kinds => Enumerable.Empty<KindDefinition>();

        public virtual IEnumerable<IClaimSource> ClaimSources => Enumerable.Empty<IClaimSource>();

        public virtual IEnumerable<Pack> Packs => Enumerable.Empty<Pack>();

        /// <summary>
        /// The languages it reads. One with the same name as a stock language replaces it, as a later extension's always
        /// does; its profile comes from a pack, as a stock language's does.
        /// </summary>
        public virtual IEnumerable<ILanguage> Languages => Enumerable.Empty<ILanguage>();

        public virtual IEnumerable<ICodeReader> Readers => Enumerable.Empty<ICodeReader>();

        public virtual IEnumerable<IClaimReader> ClaimReaders => Enumerable.Empty<IClaimReader>();

        public virtual IEnumerable<IMemberRefiner> Refiners => Enumerable.Empty<IMemberRefiner>();

        public virtual IEnumerable<IOptInRule> OptInRules => Enumerable.Empty<IOptInRule>();

        public virtual IEnumerable<IValueComparer> Comparers => Enumerable.Empty<IValueComparer>();

        public virtual IEnumerable<IDriftRule> Rules => Enumerable.Empty<IDriftRule>();

        public virtual IEnumerable<ModelFeature> Reads => Enumerable.Empty<ModelFeature>();

        public virtual IEnumerable<ModelFeature> NonClaims => Enumerable.Empty<ModelFeature>();

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
