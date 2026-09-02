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

        // Both the Intent Eventing Contracts interface this class implements and Wolverine's own
        // are named IMessageBus; alias both so the generated code is unambiguous on sight.
        private const string ContractsBusAlias = "ContractsMessageBus";
        private const string WolverineBusAlias = "WolverineBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // Lets a broker module that ADDS a member to IMessageBus supply a default implementation
            // for this bus, so it still compiles. See CONTEXT.md, D6.
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);

            var contractsBusInterfaceFullyQualifiedName = GetFullyQualifiedTypeName(this.GetBusInterfaceTemplateId());

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing($"{ContractsBusAlias} = {contractsBusInterfaceFullyQualifiedName}")
                .AddClass("WolverineMessageBus", @class =>
                {
                    // Must be the literal fully-qualified name, NOT UseType's return value: C#
                    // resolves an alias target without considering sibling usings. See CONTEXT.md.
                    CSharpFile.AddUsing($"{WolverineBusAlias} = Wolverine.IMessageBus");

                    @class.ImplementsInterface(ContractsBusAlias);

                    // R12.2. Resolved inside AddClass, not the constructor: resolving a foreign
                    // type before the first class exists throws NullReferenceException. See CONTEXT.md.
                    var finbuckleInstalled = GetTemplate<object>("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration",
                        new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false }) != null;

                    if (finbuckleInstalled)
                    {
                        // Kept inside this branch: an unconditional `using Wolverine;` shadows this
                        // file's own IMessageBus aliases (CS0104). See CONTEXT.md.
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
                        // A tenant is optional - never throw when absent (R12.1/R12.3).
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

                        // Azure Service Bus rejects a send made under an ambient TransactionScope
                        // that is not Serializable. Scoped to ASB + non-durable outbox, and inert for
                        // what this module generates today (Wolverine dispatches via a background
                        // sender, not inline) - kept as insurance. Rationale + measurements: CONTEXT.md.
                        var wolverineSettings = ExecutionContext.Settings.GetWolverineMessageBusSettings();
                        var isAzureServiceBus = wolverineSettings?.Transport()?.AsEnum()
                            == WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus;
                        var isDurableOutbox = wolverineSettings?.TransactionalOutbox()?.IsDurable() == true;
                        var suppressAmbientTransaction = isAzureServiceBus && !isDurableOutbox;

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
