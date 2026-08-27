using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Wolverine.Common.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Wolverine.Common.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineHostRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Wolverine.Common.WolverineHostRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        // Contributions are recorded here rather than via the OnEmitOrPublished/EmitOrPublish event
        // bus. That bus only works when the *owning* template subscribes in its own constructor
        // (see DependencyInjectionTemplate subscribing OnEmitOrPublished<ContainerRegistrationRequest>
        // in its own constructor, and SerilogStartupConfigurationExtension calling EmitOrPublish
        // against that already-subscribed template instance). Here the "owning" template
        // (IProgramTemplate/"App.Program") belongs to a different module (Intent.Modules.AspNetCore),
        // so nothing subscribes OnEmitOrPublished<WolverineHostConfigurationRequest> against it and
        // no amount of factory-extension Order tuning would make that reliable.
        //
        // Instead, contributing modules call Contribute(...) directly - a plain, order-independent
        // registry keyed by the IProgramTemplate instance. Order-independence is guaranteed by
        // *when* it's consumed rather than when it's written: contributions are recorded during the
        // AfterTemplateRegistrations phase (from any factory extension's OnAfterTemplateRegistrations),
        // while consumption happens inside a CSharpFile.OnBuild callback, which only runs once the
        // whole application has moved into the later Build phase - i.e. strictly after every factory
        // extension's OnAfterTemplateRegistrations, across the entire application, has completed.
        private static readonly ConditionalWeakTable<IProgramTemplate, List<WolverineHostConfigurationRequest>> _contributions = new();

        /// <summary>
        /// Contribute a request to the single, shared <c>builder.Host.UseWolverine(opts => ...)</c>
        /// registration for the given ASP.NET host program template. Safe to call from any factory
        /// extension's <c>OnAfterTemplateRegistrations</c>, regardless of that extension's <c>Order</c>
        /// relative to <see cref="WolverineHostRegistrationExtension"/>.
        /// </summary>
        public static void Contribute(IProgramTemplate programTemplate, WolverineHostConfigurationRequest request)
        {
            if (programTemplate == null)
            {
                throw new ArgumentNullException(nameof(programTemplate));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _contributions.GetOrCreateValue(programTemplate).Add(request);
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // This module only ever targets the ASP.NET host role. Azure Functions is deliberately
            // excluded - it has its own program template shape and is out of scope for this module.
            foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>("App.Program"))
            {
                RegisterWolverineOnHost(programTemplate);
            }
        }

        private static void RegisterWolverineOnHost(IProgramTemplate programTemplate)
        {
            if (programTemplate == null)
            {
                return;
            }

            programTemplate.AddNugetDependency(NugetPackages.WolverineFx(programTemplate.OutputTarget));

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Wolverine");

                programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" },
                    (lambdaBlock, parameters) =>
                    {
                        // Deliberately never call lambdaBlock.Statements.Clear() here - doing so would
                        // silently discard whatever an earlier contributor (or this method, on a second
                        // pass) already added, which is exactly the "one module's contribution
                        // overwrites another's" bug this module exists to prevent.
                        var contributions = _contributions.TryGetValue(programTemplate, out var requests)
                            ? requests.OrderBy(x => x.Priority).ToList()
                            : new List<WolverineHostConfigurationRequest>();

                        foreach (var contribution in contributions)
                        {
                            contribution.ConfigureAction?.Invoke(lambdaBlock, parameters);
                        }

                        var discoveryAssemblies = contributions
                            .SelectMany(x => x.DiscoveryAssemblies)
                            .Distinct()
                            .ToList();

                        foreach (var assembly in discoveryAssemblies)
                        {
                            lambdaBlock.AddStatement($"opts.Discovery.IncludeAssembly({GetAssemblyExpression(assembly)});");
                        }
                    });
            });
        }

        /// <summary>
        /// Produces a compilable <c>typeof(SomeType).Assembly</c> expression that resolves, at
        /// application runtime, to the given assembly. There is no direct way to embed a
        /// <see cref="Assembly"/> reference as a C# literal, so a representative exported type from
        /// that assembly is used instead - the same idiom
        /// <c>Intent.Application.Wolverine</c>'s own <c>WolverineConfigurationTemplatePartial</c> uses
        /// for a single, statically-known type (<c>typeof({commandType}).Assembly</c>), generalized
        /// here to an arbitrary, dynamically-supplied assembly.
        /// </summary>
        private static string GetAssemblyExpression(Assembly assembly)
        {
            var representativeType = assembly.GetExportedTypes().FirstOrDefault(t => !t.IsGenericTypeDefinition);

            if (representativeType == null)
            {
                throw new InvalidOperationException(
                    $"Assembly '{assembly.GetName().Name}' has no exported, non-generic types that can be used to reference it via a typeof(...).Assembly expression.");
            }

            var typeName = representativeType.FullName!.Replace('+', '.');
            return $"typeof({typeName}).Assembly";
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // Your custom logic here.
        }
    }
}
