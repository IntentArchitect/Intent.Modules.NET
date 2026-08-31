using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates.CommandHandler;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineRegistrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Wolverine.WolverineRegistrationFactoryExtension";

        // Deliberate, not incidental: this decides the order ConfigureCqrs's call statement lands
        // inside Intent.Wolverine.Common's shared Configure method body - before
        // Intent.Eventing.Wolverine's ConfigureEventing (Order 20). This module no longer touches
        // Program.cs at all; see ContributeCqrsConfiguration below.
        [IntentManaged(Mode.Ignore)]
        public override int Order => 10;

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
            ContributeCqrsConfiguration(application);
        }

        /// <summary>
        /// Finds Intent.Wolverine.Common's WolverineConfiguration template and contributes this
        /// module's CQRS configuration to it: a private <c>ConfigureCqrs</c> method carrying the
        /// logic, plus one call statement into the shared <c>Configure</c> method body. Same
        /// find-template + OnBuild + AddMethod/AddStatement idiom Intent.Eventing.MassTransit's
        /// FinbuckleConfiguratorExtension already uses on MassTransitConfigurationTemplate.
        /// <para>
        /// This module takes no ProjectReference on Intent.Wolverine.Common (see that module's
        /// CONTEXT.md for why), so its TemplateId is a string literal here, not a compiled constant.
        /// </para>
        /// </summary>
        private static void ContributeCqrsConfiguration(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Wolverine.Common.WolverineConfiguration");
            if (template == null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var configureMethod = @class.FindMethod("Configure");

                var commandType = template.GetTypeName("Intent.Application.Wolverine.CommandInterface");
                var handlerPolicyType = template.GetTypeName("Intent.Application.Wolverine.ApplicationHandlerPolicy");

                @class.AddMethod("void", "ConfigureCqrs", method =>
                {
                    method.Private().Static();
                    method.AddParameter("WolverineOptions", "opts");

                    // Load-bearing, and NOT redundant with the per-type registrations below.
                    // Wolverine's conventional discovery only scans the ENTRY assembly, so this line
                    // is the only thing that brings the Application layer assembly into discovery
                    // scope at all. Sibling modules that generate convention-named handlers into that
                    // same assembly and register nothing of their own - notably
                    // Intent.Application.Wolverine.DomainEvents - depend on it. See CONTEXT.md.
                    method.AddStatement($"opts.Discovery.IncludeAssembly(typeof({commandType}).Assembly);");

                    // R18.3: this module's OWN CQRS handlers, registered explicitly and by type so
                    // each registration is attributable to the module that owns the handler rather
                    // than riding in on another module's blanket scan.
                    foreach (var handlerTypeName in GetOwnedHandlerTypeNames(template))
                    {
                        method.AddStatement($"opts.Discovery.IncludeType<{handlerTypeName}>();");
                    }

                    method.AddStatement($"{handlerPolicyType}.Apply(opts);");
                });

                configureMethod.AddStatement("ConfigureCqrs(opts);");
            });
        }

        /// <summary>
        /// The CQRS handler types this module generates: one per Command and one per Query in the
        /// Services designer. Deduplicated and ordered, so the emitted statement set is a pure
        /// function of the model and therefore identical across Software Factory runs.
        /// </summary>
        private static IEnumerable<string> GetOwnedHandlerTypeNames(ICSharpFileBuilderTemplate template)
        {
            var services = template.ExecutionContext.MetadataManager.Services(template.ExecutionContext.GetApplicationConfig().Id);
            var handlerTypeNames = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var command in services.GetCommandModels())
            {
                if (template.TryGetTypeName(CommandHandlerTemplate.TemplateId, command, out var handlerTypeName))
                {
                    handlerTypeNames.Add(handlerTypeName);
                }
            }

            foreach (var query in services.GetQueryModels())
            {
                if (template.TryGetTypeName(QueryHandlerTemplate.TemplateId, query, out var handlerTypeName))
                {
                    handlerTypeNames.Add(handlerTypeName);
                }
            }

            return handlerTypeNames;
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
