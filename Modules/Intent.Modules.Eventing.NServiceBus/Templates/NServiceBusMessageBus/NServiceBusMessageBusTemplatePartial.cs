using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NServiceBusMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.NServiceBus.NServiceBusMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);

            var outboxPattern = ExecutionContext.Settings.GetNServiceBusSettings().OutboxPattern();
            var hasOutbox = outboxPattern.IsSqlPersistence();

            AddNugetDependency(NugetPackages.NServiceBus(OutputTarget));
            if (hasOutbox)
            {
                AddNugetDependency(NugetPackages.NServiceBusPersistenceSql(OutputTarget));
                AddNugetDependency(NugetPackages.NServiceBusPersistenceSqlTransactionalSession(OutputTarget));
                AddNugetDependency(NugetPackages.MicrosoftDataSqlClient(OutputTarget));
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("NServiceBus");

            if (hasOutbox)
            {
                CSharpFile
                    .AddUsing("System.Transactions")
                    .AddUsing("Microsoft.EntityFrameworkCore")
                    .AddUsing("NServiceBus.Persistence.Sql")
                    .AddUsing("NServiceBus.TransactionalSession");
            }

            CSharpFile.AddClass("NServiceBusMessageBus", @class =>
            {
                @class.ImplementsInterface(this.GetBusInterfaceName());

                // Events (Publish) and commands (Send) are buffered separately because NSB routes them
                // differently: events go via Publish (pub/sub), commands go via Send (point-to-point routing
                // configured by RouteToEndpoint in NServiceBusConfiguration).
                @class.AddField("List<object>", "_publishBuffer", field => field
                    .PrivateReadOnly()
                    .WithAssignment(new CSharpStatement("new()")));
                @class.AddField("List<object>", "_sendBuffer", field => field
                    .PrivateReadOnly()
                    .WithAssignment(new CSharpStatement("new()")));

                @class.AddConstructor(ctor =>
                {
                    if (hasOutbox)
                    {
                        ctor.AddParameter("ITransactionalSession", "transactionalSession", p => p.IntroduceReadonlyField());
                        // DbContext is resolved by the EF template role
                        ctor.AddParameter(this.GetTypeName(TemplateRoles.Infrastructure.Data.DbContext), "dbContext", p => p.IntroduceReadonlyField());
                    }
                    else
                    {
                        ctor.AddParameter("IMessageSession", "messageSession", p => p.IntroduceReadonlyField());
                    }
                });

                // The ActiveContext property is the core of the push pattern (mirrors MassTransit's ConsumeContext).
                // Infrastructure sets this before business logic runs — NServiceBusMessageHandler<T> sets it to
                // IMessageHandlerContext; HTTP middleware sets it to ITransactionalSession.
                // Application code never touches it.
                @class.AddProperty("object?", "ActiveContext");

                @class.AddMethod("void", "Publish", method =>
                {
                    method.AddGenericParameter("TMessage", out var tMessage)
                        .AddGenericTypeConstraint(tMessage, c => c.AddType("class"))
                        .AddParameter(tMessage, "message");
                    method.AddStatement("_publishBuffer.Add(message);");
                });

                @class.AddMethod("void", "Send", method =>
                {
                    method.AddGenericParameter("TMessage", out var tMessage)
                        .AddGenericTypeConstraint(tMessage, c => c.AddType("class"))
                        .AddParameter(tMessage, "message");
                    method.AddStatement("_sendBuffer.Add(message);");
                });

                @class.AddMethod("Task", "FlushAllAsync", method =>
                {
                    method.Async();
                    method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));

                    method.AddIfStatement("_publishBuffer.Count == 0 && _sendBuffer.Count == 0", b =>
                        b.AddStatement("return;"));

                    // Priority 1: inside an inbound NSB handler — route through handler's outbox transaction.
                    // IMessageHandlerContext.Publish/Send do NOT have CancellationToken overloads; use
                    // PublishOptions/SendOptions to pass cancellation if needed in future.
                    method.AddIfStatement("ActiveContext is IMessageHandlerContext handlerContext", b =>
                    {
                        b.AddForEachStatement("message", "_publishBuffer", fe =>
                            fe.AddStatement("await handlerContext.Publish(message, new PublishOptions());"));
                        b.AddForEachStatement("message", "_sendBuffer", fe =>
                            fe.AddStatement("await handlerContext.Send(message, new SendOptions());"));
                        b.AddStatement("_publishBuffer.Clear();");
                        b.AddStatement("_sendBuffer.Clear();");
                        b.AddStatement("return;");
                    });

                    if (hasOutbox)
                    {
                        // Priority 2: HTTP/MediatR path — open ITransactionalSession and join EF atomically.
                        // Suppress any ambient TransactionScope (e.g. from UnitOfWorkBehaviour) so that
                        // NSB's SqlConnection does not auto-enlist and trigger MSDTC escalation.
                        // EF joins NSB's own connection+transaction below, ensuring atomicity.
                        method.AddUsingBlock(
                            "new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)",
                            usingBlock =>
                            {
                                usingBlock.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
                                usingBlock.AddStatement("await _transactionalSession.Open(new SqlPersistenceOpenSessionOptions(), cancellationToken);");
                                usingBlock.AddStatement("var sqlSession = _transactionalSession.SynchronizedStorageSession.SqlPersistenceSession();");
                                usingBlock.AddStatement("_dbContext.Database.SetDbConnection(sqlSession.Connection);");
                                usingBlock.AddStatement("await _dbContext.Database.UseTransactionAsync((System.Data.Common.DbTransaction)sqlSession.Transaction, cancellationToken);");
                                usingBlock.AddTryBlock(tryBlock =>
                                {
                                    tryBlock.AddForEachStatement("message", "_publishBuffer", fe =>
                                        fe.AddStatement("await _transactionalSession.Publish(message, cancellationToken);"));
                                    tryBlock.AddForEachStatement("message", "_sendBuffer", fe =>
                                        fe.AddStatement("await _transactionalSession.Send(message, cancellationToken);"));
                                    tryBlock.AddStatement("await _dbContext.SaveChangesAsync(cancellationToken);", s => s.SeparatedFromPrevious());
                                    tryBlock.AddStatement("await _transactionalSession.Commit(cancellationToken);");
                                    // Only clear after successful commit — messages would be lost on crash if cleared earlier
                                    tryBlock.AddStatement("_publishBuffer.Clear();", s => s.SeparatedFromPrevious());
                                    tryBlock.AddStatement("_sendBuffer.Clear();");
                                })
                                .AddFinallyBlock(finallyBlock =>
                                {
                                    finallyBlock.AddStatement("_dbContext.Database.SetDbConnection(null);");
                                });
                            });
                    }
                    else
                    {
                        // Priority 3: best-effort dispatch via global IMessageSession
                        method.AddForEachStatement("message", "_publishBuffer", fe =>
                            fe.AddStatement("await _messageSession.Publish(message, cancellationToken);"));
                        method.AddForEachStatement("message", "_sendBuffer", fe =>
                            fe.AddStatement("await _messageSession.Send(message, cancellationToken);"));
                        method.AddStatement("_publishBuffer.Clear();", s => s.SeparatedFromPrevious());
                        method.AddStatement("_sendBuffer.Clear();");
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