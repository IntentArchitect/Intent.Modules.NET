using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Events
{
    /// <summary>A module's extension registering with the run's drift engine (<see cref="Api.DriftEngineExtensionBase"/>).</summary>
    internal sealed class DriftEngineExtensionRegistered
    {
        public const string EventId = "Intent.Common.DriftEngine.ExtensionRegistered";

        public DriftEngineExtensionRegistered(IDriftExtension extension)
        {
            Extension = extension;
        }

        public IDriftExtension Extension { get; }
    }
}
