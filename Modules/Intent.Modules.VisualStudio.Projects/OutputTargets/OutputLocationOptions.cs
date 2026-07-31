using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.OutputTargets
{
    /// <summary>
    /// Wraps the Root Folder's "Relative Location" so the app's absolute output root
    /// (<see cref="IApplication.OutputRootDirectory"/>) can be shifted uniformly for consumers that
    /// need an absolute base (<see cref="RootDirectory"/>) and consumers that report a value relative
    /// to it (<see cref="Combine"/>). An unset/blank Relative Location makes both a no-op, so every
    /// caller resolves to exactly the value it did before this type existed.
    /// </summary>
    internal sealed class OutputLocationOptions
    {
        /// <summary>
        /// A no-op shift for consumers (e.g. <c>ProjectConfig</c>) that have no reachable
        /// <see cref="IApplication"/> to resolve a real instance from. Only <see cref="Combine"/> is
        /// safe to call on this instance - <see cref="RootDirectory"/> would throw, since there is no
        /// real output root behind it.
        /// </summary>
        public static readonly OutputLocationOptions None = new(outputRootDirectory: null, relativeLocation: "");

        private readonly string _outputRootDirectory;

        public OutputLocationOptions(string outputRootDirectory, string relativeLocation)
        {
            _outputRootDirectory = outputRootDirectory;
            RelativeLocation = relativeLocation ?? string.Empty;
        }

        public string RelativeLocation { get; }

        public string RootDirectory => string.IsNullOrEmpty(RelativeLocation)
            ? _outputRootDirectory
            : Path.GetFullPath(Path.Combine(_outputRootDirectory, RelativeLocation));

        public string Combine(string relativeLocation)
        {
            return string.IsNullOrEmpty(RelativeLocation)
                ? relativeLocation
                : Path.Combine(RelativeLocation, relativeLocation);
        }

        /// <summary>
        /// The value an output target should report for its own relative location: the explicit
        /// <paramref name="rawRelativeLocation"/> when set (never shifted - an explicit location is
        /// already meaningful on its own terms), otherwise <paramref name="fallbackName"/> combined
        /// with the shift (mirrors the historical Name-fallback, made shift-aware).
        /// </summary>
        public string GetEffectiveRelativeLocation(string rawRelativeLocation, string fallbackName)
        {
            return string.IsNullOrWhiteSpace(rawRelativeLocation)
                ? Combine(fallbackName)
                : rawRelativeLocation;
        }
    }

    internal static class ApplicationOutputLocationExtensions
    {
        internal static OutputLocationOptions GetOutputLocationOptions(this IApplication application, IMetadataManager metadataManager)
        {
            // Zero root folders (module not installed against this app) or several (no single
            // canonical shift) both defensively resolve to the first non-blank value found, or "" (no-op).
            var relativeLocation = metadataManager.CodebaseStructure(application)
                .GetRootFolderModels()
                .Select(rootFolder => rootFolder.GetRootFolderOptions()?.RelativeLocation())
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? "";

            return new OutputLocationOptions(application.OutputRootDirectory, relativeLocation);
        }
    }
}
