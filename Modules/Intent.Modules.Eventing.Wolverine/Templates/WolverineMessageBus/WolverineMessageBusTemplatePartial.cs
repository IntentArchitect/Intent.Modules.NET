using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantHeaderStrategy;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineMessageBus";

        // Two different interfaces are both called IMessageBus - the Intent Eventing Contracts one
        // this class implements, and Wolverine's own that it delegates to. They are aliased at the
        // using site rather than qualified inline, so which is which is unambiguous on sight to a
        // developer reading the generated code.
        private const string ContractsBusAlias = "ContractsMessageBus";
        private const string WolverineBusAlias = "WolverineBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // Every other eventing provider (MassTransit, NServiceBus, Kafka, Solace, AzureServiceBus,
            // AzureEventGrid, AzureQueueStorage, Dapr, Aws.Sqs) fulfils this role. It is how a broker
            // module that ADDS a member to the IMessageBus interface - MassTransit's addressed Send
            // overload, MassTransit.Scheduling's scheduling members - finds every provider's bus and
            // gives it a default implementation, so all of them still compile. Without it this bus is
            // invisible to those extensions and has to hand-roll each added member itself, which is
            // how the addressed Send overload came to be written here ignoring its address.
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);

            var contractsBusInterfaceFullyQualifiedName = GetFullyQualifiedTypeName(this.GetBusInterfaceTemplateId());

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing($"{ContractsBusAlias} = {contractsBusInterfaceFullyQualifiedName}")
                .AddClass("WolverineMessageBus", @class =>
                {
                    // Deliberately not using UseType's return value here: it shortens the
                    // reference to whatever is valid given the file's OTHER using directives
                    // (e.g. "IMessageBus" once "using Wolverine;" is present), but C# resolves a
                    // using-alias's right-hand side WITHOUT considering sibling using directives -
                    // only enclosing namespaces and fully-qualified names count there. A shortened
                    // name that compiles fine as an ordinary reference can fail to resolve as an
                    // alias target, so the alias always spells out the literal fully-qualified name.
                    CSharpFile.AddUsing($"{WolverineBusAlias} = Wolverine.IMessageBus");

                    @class.ImplementsInterface(ContractsBusAlias);

                    // R12.2: only present when Finbuckle multi-tenancy is installed - WolverineTenantHeaderStrategyTemplate
                    // gates itself on the same check, so this simply mirrors whatever that template decided.
                    // Resolved HERE rather than in the constructor: GetTypeName resolves a foreign type,
                    // and the SDK derives this template's own output path from its first added class, so
                    // resolving before AddClass throws NullReferenceException for any application
                    // generating this file for the first time.
                    // Gate on the SAME condition WolverineTenantHeaderStrategyTemplate.CanRunTemplate()
                    // uses, not on that template's mere existence: its instance is registered
                    // unconditionally and only declines to produce a file, so an existence check
                    // reports "present" for an application that never gets the class - leaving this
                    // bus referencing a type nobody generated (CS0246).
                    var tenantHeaderStrategyTypeName = GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration",
                        new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null
                        ? GetTypeName(WolverineTenantHeaderStrategyTemplate.TemplateId)
                        : null;

                    // PublishAsync/SendAsync return ValueTask, not Task.
                    @class.AddField($"List<Func<{WolverineBusAlias}, ValueTask>>", "_pendingActions", field => field
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(WolverineBusAlias, "bus", param => param.IntroduceReadonlyField());
                        if (tenantHeaderStrategyTypeName != null)
                        {
                            ctor.AddParameter(tenantHeaderStrategyTypeName, "tenantHeaderStrategy", param => param.IntroduceReadonlyField());
                        }
                    });

                    var deliveryOptionsArg = tenantHeaderStrategyTypeName != null
                        ? ", _tenantHeaderStrategy.BuildDeliveryOptions()"
                        : string.Empty;

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage)
                            .AddGenericTypeConstraint(tMessage, c => c.AddType("class"))
                            .AddParameter(tMessage, "message");
                        method.AddStatement($"_pendingActions.Add(bus => bus.PublishAsync(message{deliveryOptionsArg}));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage)
                            .AddGenericTypeConstraint(tMessage, c => c.AddType("class"))
                            .AddParameter(tMessage, "message");
                        method.AddStatement($"_pendingActions.Add(bus => bus.SendAsync(message{deliveryOptionsArg}));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddIfStatement("_pendingActions.Count == 0", stmt => stmt.AddStatement("return;"));

                        // Copy-then-clear so a flush failure partway through doesn't leave
                        // already-flushed actions still queued.
                        method.AddStatement(
                            $"""
                            var toFlush = new List<Func<{WolverineBusAlias}, ValueTask>>(_pendingActions);
                            _pendingActions.Clear();
                            """,
                            s => s.SeparatedFromPrevious());

                        method.AddForEachStatement("action", "toFlush", loop =>
                        {
                            loop.AddStatement("cancellationToken.ThrowIfCancellationRequested();");
                            loop.AddStatement("await action(_bus);");
                        });
                    });
                });
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
