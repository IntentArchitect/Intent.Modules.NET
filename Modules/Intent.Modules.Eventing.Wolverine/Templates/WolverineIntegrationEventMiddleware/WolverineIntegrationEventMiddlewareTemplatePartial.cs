using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.UnitOfWork.Settings;
using Intent.Modules.Common.UnitOfWork.Shared;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Wolverine.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineIntegrationEventMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineIntegrationEventMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineIntegrationEventMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineIntegrationEventMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("WolverineIntegrationEventMiddleware", @class =>
                {
                    // R19.x (Outbox = None): a subscriber's handler otherwise has no unit-of-work
                    // safety net at all - no row written, no cascading event published, no exception.
                    // This middleware is the single consume-path seam for every integration message
                    // this application receives, covering what AutoApplyTransactions() + the
                    // ApplicationDbContext.SaveChangesAsync splice cover under Outbox = Durable.
                    // See CONTEXT.md.
                    // Not a plain TryGetTypeName(WolverineMessageBusTemplate.TemplateId, ...): the
                    // class always exists, but its DI registration is conditional (R3.9 - a
                    // subscribe-only application gets none). Injecting it unregistered compiles fine
                    // and throws "No service for type ... has been registered" on the first message
                    // the app actually receives. See WolverineMessageBusIsRegistered's own comment.
                    var hasBus = this.WolverineMessageBusIsRegistered();

                    // Inject the IMessageBus CONTRACT, not the concrete WolverineMessageBus class:
                    // DependencyInjection.cs only registers "services.AddScoped<IMessageBus,
                    // WolverineMessageBus>()" - the concrete type is never registered on its own, so
                    // asking the container for it throws "No service for type ... has been
                    // registered" the first time this application actually receives a message. Same
                    // interface-resolution pattern as MessageBusPublishBehaviourTemplatePartial.
                    string busType = null;
                    if (hasBus &&
                        !TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out busType) &&
                        !TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out busType))
                    {
                        throw new InvalidOperationException(
                            "Could not find Message Bus interface with template role 'TemplateRoles.Application.Eventing.MessageBusInterface' (or older legacy template with role 'TemplateRoles.Application.Eventing.EventBusInterface').");
                    }
                    var usesPersistenceUnitOfWork = this.SystemUsesPersistenceUnitOfWork();
                    var wrapInTransaction = usesPersistenceUnitOfWork
                        && ExecutionContext.GetSettings()?.GetUnitOfWorkSettings()?.UseAmbientTransactions() == true
                        && ProviderSupportsAmbientTransactions();

                    CSharpConstructor ctor = null;
                    if (hasBus || usesPersistenceUnitOfWork)
                    {
                        @class.AddConstructor(c =>
                        {
                            ctor = c;

                            if (hasBus)
                            {
                                ctor.AddParameter(busType, "messageBus", param => param.IntroduceReadonlyField());
                            }
                        });
                    }

                    if (wrapInTransaction)
                    {
                        // Concerns 2/6: Before() opens the ambient transaction; FinallyAsync is the
                        // only hook Wolverine guarantees on a throwing handler, so disposal (and
                        // therefore rollback) lives there, not in an AfterAsync try/finally. See
                        // CONTEXT.md "AFTER-NOT-RUN-ON-THROW".
                        @class.AddNestedClass("Scope", nested =>
                        {
                            nested.Sealed();
                            nested.AddProperty(UseType("System.Transactions.TransactionScope"), "Transaction");
                            nested.AddProperty("bool", "Committed");
                        });

                        @class.AddMethod("Scope", "Before", method =>
                        {
                            var transactionScopeType = UseType("System.Transactions.TransactionScope");
                            var transactionScopeOptionType = UseType("System.Transactions.TransactionScopeOption");
                            var transactionOptionsType = UseType("System.Transactions.TransactionOptions");
                            var isolationLevelType = UseType("System.Transactions.IsolationLevel");
                            var transactionScopeAsyncFlowOptionType = UseType("System.Transactions.TransactionScopeAsyncFlowOption");

                            method.AddObjectInitializerBlock("return new Scope", scope =>
                            {
                                scope.AddInitStatement("Transaction",
                                    $"new {transactionScopeType}({transactionScopeOptionType}.Required, new {transactionOptionsType} {{ IsolationLevel = {isolationLevelType}.ReadCommitted }}, {transactionScopeAsyncFlowOptionType}.Enabled)");
                                scope.WithSemicolon();
                            });
                        });
                    }

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "AfterAsync", method =>
                    {
                        method.Async();
                        if (wrapInTransaction)
                        {
                            method.AddParameter("Scope", "scope");
                        }

                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        if (usesPersistenceUnitOfWork)
                        {
                            // The shared unit-of-work component generates this call's entire body -
                            // the same eight-provider detection chain the MassTransit consumer, the
                            // MediatR UnitOfWorkBehaviour and the ServiceContract controllers use.
                            // WithTransactionScope(false): Before() has already opened the scope, so
                            // this must not wrap the SaveChangesAsync call(s) in its own.
                            method.ApplyUnitOfWorkImplementations(this, ctor, config => config
                                .UseCancellationToken("cancellationToken")
                                .WithTransactionScope(false)
                                .WithComments(false)
                                .UseConstructorInjection());
                        }

                        if (wrapInTransaction)
                        {
                            method.AddStatement("scope.Transaction.Complete();", s => s.SeparatedFromPrevious());
                            method.AddStatement("scope.Committed = true;");
                        }
                        else if (hasBus)
                        {
                            // No ambient transaction was opened, so there is nothing for FinallyAsync
                            // to dispose before flushing - the flush can happen right here.
                            method.AddStatement("await _messageBus.FlushAllAsync(cancellationToken);", s => s.SeparatedFromPrevious());
                        }
                    });

                    if (wrapInTransaction)
                    {
                        @class.AddMethod(UseType("System.Threading.Tasks.Task"), "FinallyAsync", method =>
                        {
                            method.Async();
                            method.AddParameter("Scope", "scope");
                            method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                            // Concern 6: disposing without Complete() rolls back. Concern 7: the flush
                            // happens AFTER dispose, so a broker publish can never occur inside the
                            // ambient scope (load-bearing for Azure Service Bus - see CONTEXT.md).
                            method.AddStatement("scope.Transaction.Dispose();");

                            if (hasBus)
                            {
                                method.AddIfStatement("scope.Committed", @if => @if.AddStatement("await _messageBus.FlushAllAsync(cancellationToken);"));
                            }
                        });
                    }
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

        /// <summary>
        /// Mirrors the shared unit-of-work component's own EF ambient-transaction exclusion (SQLite
        /// does not support System.Transactions.TransactionScope) without depending on that internal
        /// detail directly, and without adding an Intent.EntityFrameworkCore dependency just for its
        /// typed Database Provider accessor - read via the raw setting group/id instead, same as the
        /// shared component does internally.
        /// </summary>
        private bool ProviderSupportsAmbientTransactions()
        {
            var databaseSettingGroup = ExecutionContext.GetSettings().GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9"); // Database Settings
            var provider = databaseSettingGroup?.GetSetting("00bb780c-57bf-43c1-b952-303f11096be7")?.Value; // Database Provider
            return provider is null || provider != "sql-lite";
        }

        /// <summary>
        /// None-only: under Outbox = Durable, AutoApplyTransactions() plus the ApplicationDbContext
        /// splice already cover this application's message handlers - emitting this middleware too
        /// would double-wrap the transaction and double-flush. Also skipped when the application has
        /// neither a message bus nor a persistence unit of work to protect.
        /// </summary>
        public override bool CanRunTemplate()
        {
            if (!base.CanRunTemplate())
            {
                return false;
            }

            if (ExecutionContext.Settings.GetWolverineMessageBusSettings()?.TransactionalOutbox()?.IsDurable() == true)
            {
                return false;
            }

            return this.WolverineMessageBusIsRegistered() || this.SystemUsesPersistenceUnitOfWork();
        }
    }
}
