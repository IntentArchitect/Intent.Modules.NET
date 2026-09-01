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
using Intent.Modules.Eventing.Wolverine.Settings;
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

                    // R12.2: only present when Finbuckle multi-tenancy is installed -
                    // WolverineTenantStrategyTemplate gates itself on the same check, so this simply
                    // mirrors whatever that template decided. Resolved HERE rather than in the
                    // constructor: the SDK derives this template's own output path from its first
                    // added class, so resolving a foreign type before AddClass throws
                    // NullReferenceException for any application generating this file for the first
                    // time. This bus no longer depends on a generated strategy class at all - the
                    // tenant identifier is read directly off IMultiTenantContextAccessor and carried
                    // on DeliveryOptions.TenantId, Wolverine's own envelope-native tenant field.
                    var finbuckleInstalled = GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration",
                        new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null;

                    if (finbuckleInstalled)
                    {
                        // Only needed for DeliveryOptions (BuildDeliveryOptions, below) - an app
                        // with no Finbuckle installed never references it, so this stays out of the
                        // unconditional using list rather than risk shadowing this file's OWN
                        // ContractsMessageBus/WolverineBus aliases the way an unconditional
                        // `using Wolverine;` shadowed the sibling contract's IMessageBus in the
                        // CS0104 this module's Point 1 fix removed.
                        CSharpFile.AddUsing("Wolverine");
                        CSharpFile.AddUsing("Finbuckle.MultiTenant.Abstractions");
                    }

                    // PublishAsync/SendAsync return ValueTask, not Task.
                    @class.AddField($"List<Func<{WolverineBusAlias}, ValueTask>>", "_pendingActions", field => field
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(WolverineBusAlias, "bus", param => param.IntroduceReadonlyField());
                        if (finbuckleInstalled)
                        {
                            ctor.AddParameter("IMultiTenantContextAccessor", "multiTenantContextAccessor", param => param.IntroduceReadonlyField());
                        }
                    });

                    var deliveryOptionsArg = finbuckleInstalled
                        ? ", BuildDeliveryOptions()"
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

                    if (finbuckleInstalled)
                    {
                        // A resolved tenant is optional here, not required: a message may
                        // legitimately be published without one (e.g. a background/system process
                        // with no ambient tenant). Only set it when a tenant is actually present -
                        // never throw (R12.1/R12.3).
                        @class.AddMethod("DeliveryOptions?", "BuildDeliveryOptions", method =>
                        {
                            method.Private();
                            method.AddStatement("var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;");
                            method.AddStatement("return tenantIdentifier is null ? null : new DeliveryOptions { TenantId = tenantIdentifier };", s => s.SeparatedFromPrevious());
                        });
                    }

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

                        // R6.3: brokers must never be called under an ambient TransactionScope.
                        // Verified against a real Azure Service Bus namespace: a send inside a
                        // TransactionScope throws "The only supported IsolationLevel is Serializable",
                        // and UnitOfWorkMiddleware/UnitOfWorkBehaviour both open theirs at
                        // ReadCommitted - so any flush reached while one is open is a hard failure,
                        // with or without a database enlisted. (Serializable is no escape either:
                        // the send succeeds and the commit then throws TransactionInDoubtException.)
                        // Today the dispatch-layer flush seam is registered OUTSIDE the unit of work
                        // in both stacks, so this never triggers - but that is implicit ordering, and
                        // this makes the guarantee explicit. Matches Intent.Eventing.AzureServiceBus's
                        // AzureServiceBusMessageBus and Intent.Eventing.NServiceBus's bus.
                        //
                        // Deliberately NOT emitted for the Durable outbox: there the DbContext splice
                        // (WolverineMessageBusInteropExtension) calls this from inside SaveChangesAsync,
                        // where Wolverine enrols outgoing envelopes on the DbContext's own connection so
                        // they commit atomically with the entity changes. Suppressing the ambient
                        // transaction there risks decoupling the envelope write from that commit, which
                        // is the one guarantee the outbox exists to provide. No broker call happens on
                        // that path anyway - the durability agent dispatches later, out of band.
                        var isDurableOutbox = ExecutionContext.Settings
                            .GetWolverineMessageBusSettings()?.TransactionalOutbox()?.IsDurable() == true;
                        var suppressAmbientTransaction = !isDurableOutbox;

                        if (suppressAmbientTransaction)
                        {
                            CSharpFile.AddUsing("System.Transactions");
                            method.AddStatement(
                                "using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);",
                                s => s.SeparatedFromPrevious());
                        }

                        method.AddForEachStatement("action", "toFlush", loop =>
                        {
                            loop.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
                            loop.AddStatement("cancellationToken.ThrowIfCancellationRequested();");
                            loop.AddStatement("await action(_bus);");
                        });

                        if (suppressAmbientTransaction)
                        {
                            method.AddStatement("scope.Complete();", s => s.SeparatedFromPrevious());
                        }
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
