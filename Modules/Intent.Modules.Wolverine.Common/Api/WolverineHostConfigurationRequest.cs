using System.Collections.Generic;
using System.Reflection;
using Intent.Modules.Common.CSharp.AppStartup;

namespace Intent.Modules.Wolverine.Common.Api
{
    /// <summary>
    /// Request that a contribution be added to the single, shared
    /// <c>builder.Host.UseWolverine(opts => ...)</c> registration owned by
    /// <c>Intent.Wolverine.Common</c>.
    /// </summary>
    public class WolverineHostConfigurationRequest
    {
        private readonly List<Assembly> _discoveryAssemblies = new();

        /// <summary>
        /// Statements to add inside the <c>opts</c> lambda of the shared
        /// <c>UseWolverine(opts => ...)</c> registration.
        /// </summary>
        public IProgramFile.HostBuilderChainStatementConfiguration ConfigureAction { get; }

        /// <summary>
        /// Determines the order in which this contribution's statements are
        /// added to the shared registration, relative to other contributions.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Assemblies to be included for Wolverine's conventional handler
        /// discovery (<c>opts.Discovery.IncludeAssembly(...)</c>).
        /// </summary>
        public IEnumerable<Assembly> DiscoveryAssemblies => _discoveryAssemblies;

        private WolverineHostConfigurationRequest(IProgramFile.HostBuilderChainStatementConfiguration configureAction)
        {
            ConfigureAction = configureAction;
        }

        /// <summary>
        /// Creates a request that adds statements to the shared <c>UseWolverine(opts => ...)</c>
        /// registration using the provided callback.
        /// </summary>
        public static WolverineHostConfigurationRequest Configure(IProgramFile.HostBuilderChainStatementConfiguration configureAction)
        {
            return new WolverineHostConfigurationRequest(configureAction);
        }

        /// <summary>
        /// Supply a priority that will determine the order in which this contribution's
        /// statements get added, relative to other contributions.
        /// </summary>
        public WolverineHostConfigurationRequest WithPriority(int priority)
        {
            Priority = priority;
            return this;
        }

        /// <summary>
        /// Requests that the given assembly be included in Wolverine's conventional
        /// handler discovery.
        /// </summary>
        /// <remarks>
        /// KNOWN DEFECT - this overload is currently unusable, and has no callers. It takes a
        /// runtime-loaded <see cref="Assembly"/>, but the only assemblies loaded while the Software
        /// Factory runs are Intent module/SDK assemblies; the consuming application's own
        /// Application-layer assembly does not exist yet and is never loaded. Consequently
        /// <c>WolverineHostRegistrationExtension.GetAssemblyExpression</c> would emit
        /// <c>typeof(SomeIntentModuleType).Assembly</c>, which does not even compile in the consumer
        /// (it has no reference to Intent module assemblies). Expressing "the generated app's
        /// Application-layer assembly" needs a template-id/role plus model reference resolved
        /// through the contributing template, not a <see cref="Assembly"/>. Kept as-is for now
        /// purely to avoid a breaking change to this API surface; a contributor that needs assembly
        /// discovery should emit the statement from its own template via <see cref="Configure"/>
        /// (as <c>Intent.Application.Wolverine</c>'s <c>WolverineConfigurationTemplate</c> does).
        /// </remarks>
        public WolverineHostConfigurationRequest RequiringDiscoveryOf(Assembly assembly)
        {
            if (assembly is not null && !_discoveryAssemblies.Contains(assembly))
            {
                _discoveryAssemblies.Add(assembly);
            }

            return this;
        }
    }
}
