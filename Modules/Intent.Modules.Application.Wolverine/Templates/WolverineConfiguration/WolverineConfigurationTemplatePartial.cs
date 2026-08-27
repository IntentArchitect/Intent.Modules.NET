using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates.CommandHandler;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.WolverineConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.WolverineConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.WolverineFx(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Wolverine")
                .AddClass("WolverineConfiguration", @class =>
                {
                    var commandType = GetTypeName("Intent.Application.Wolverine.CommandInterface");
                    var handlerPolicyType = GetTypeName("Intent.Application.Wolverine.ApplicationHandlerPolicy");

                    @class.Static();

                    @class.AddMethod("void", "Configure", method =>
                    {
                        method.Static();
                        method.AddParameter("WolverineOptions", "opts");

                        // Load-bearing, and NOT redundant with the per-type registrations below.
                        // Wolverine's conventional discovery only scans the ENTRY assembly, so this
                        // line is the only thing that brings the Application layer assembly into
                        // discovery scope at all. Sibling modules that generate convention-named
                        // handlers into that same assembly and register nothing of their own -
                        // notably Intent.Application.Wolverine.DomainEvents - depend on it, and
                        // would be silently stranded if it were removed. See CONTEXT.md.
                        method.AddStatement($"opts.Discovery.IncludeAssembly(typeof({commandType}).Assembly);");

                        // R18.3: this module's OWN CQRS handlers, registered explicitly and by type
                        // so each registration is attributable to the module that owns the handler
                        // rather than riding in on another module's blanket scan.
                        foreach (var handlerTypeName in GetOwnedHandlerTypeNames())
                        {
                            method.AddStatement($"opts.Discovery.IncludeType<{handlerTypeName}>();");
                        }

                        method.AddStatement($"{handlerPolicyType}.Apply(opts);");
                    });
                });
        }

        /// <summary>
        /// The CQRS handler types this module generates: one per Command and one per Query in the
        /// Services designer. Deduplicated and ordered, so the emitted statement set is a pure
        /// function of the model and therefore identical across Software Factory runs.
        /// <para>
        /// Registering a type here that conventional discovery ALSO finds (via the
        /// <c>IncludeAssembly</c> above) does not double-register: verified against WolverineFx
        /// 5.39.5 that the message's handler chain still holds exactly one <c>HandlerCall</c> for
        /// that type/method, and the handler pipeline generates and executes cleanly. Wolverine
        /// de-duplicates by handler type plus method. See CONTEXT.md.
        /// </para>
        /// </summary>
        private IEnumerable<string> GetOwnedHandlerTypeNames()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);
            var handlerTypeNames = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var command in services.GetCommandModels())
            {
                if (TryGetTypeName(CommandHandlerTemplate.TemplateId, command, out var handlerTypeName))
                {
                    handlerTypeNames.Add(handlerTypeName);
                }
            }

            foreach (var query in services.GetQueryModels())
            {
                if (TryGetTypeName(QueryHandlerTemplate.TemplateId, query, out var handlerTypeName))
                {
                    handlerTypeNames.Add(handlerTypeName);
                }
            }

            return handlerTypeNames;
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}