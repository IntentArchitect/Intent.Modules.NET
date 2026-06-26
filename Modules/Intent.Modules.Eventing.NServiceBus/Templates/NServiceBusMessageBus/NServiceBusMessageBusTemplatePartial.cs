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

            var nsbSettings = ExecutionContext.Settings.GetNServiceBusSettings();
            var persistence = nsbSettings.Persistence();
            var hasOutbox = nsbSettings.EnableOutbox() && (persistence.IsSqlPersistence() || persistence.IsNhibernate());

            AddNugetDependency(NugetPackages.NServiceBus(OutputTarget));
            if (hasOutbox)
            {
                if (persistence.IsSqlPersistence())
                {
                    AddNugetDependency(NugetPackages.NServiceBusPersistenceSql(OutputTarget));
                    AddNugetDependency(NugetPackages.NServiceBusPersistenceSqlTransactionalSession(OutputTarget));
                    AddNugetDependency(NugetPackages.MicrosoftDataSqlClient(OutputTarget));
                }
                else if (persistence.IsNhibernate())
                {
                    AddNugetDependency(NugetPackages.NServiceBusNHibernate(OutputTarget));
                    AddNugetDependency(NugetPackages.NServiceBusNHibernateTransactionalSession(OutputTarget));
                }
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Transactions")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("NServiceBus");

            if (hasOutbox)
            {
                if (persistence.IsSqlPersistence())
                    CSharpFile.AddUsing("NServiceBus.Persistence.Sql");
                CSharpFile.AddUsing("NServiceBus.TransactionalSession");
            }

            CSharpFile.AddClass("NServiceBusMessageBus", @class =>
            {
                @class.ImplementsInterface(this.GetBusInterfaceName());

                @class.AddField("List<object>", "_publishBuffer", field => field
                    .PrivateReadOnly()
                    .WithAssignment(new CSharpStatement("new()")));
                @class.AddField("List<object>", "_sendBuffer", field => field
                    .PrivateReadOnly()
                    .WithAssignment(new CSharpStatement("new()")));

                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter("IServiceProvider", "serviceProvider", p => p.IntroduceReadonlyField());
                });

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

                    // IMessageHandlerContext.Publish/Send do not have CancellationToken overloads;
                    // pass PublishOptions/SendOptions instead.
                    method.AddIfStatement("ActiveContext is IMessageHandlerContext handlerContext", b =>
                    {
                        b.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
                        b.AddStatement("await DispatchAsync(m => handlerContext.Publish(m, new PublishOptions()), m => handlerContext.Send(m, new SendOptions()));");
                        b.AddStatement("return;");
                    });

                    // Both non-handler paths suppress any ambient TransactionScope to prevent
                    // ASB/RabbitMQ/SQL clients from attempting DTC enlistment.
                    method.AddUsingBlock(
                        "new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled)",
                        usingBlock =>
                        {
                            usingBlock.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;

                            if (hasOutbox)
                            {
                                usingBlock.AddStatement("var transactionalSession = _serviceProvider.GetRequiredService<ITransactionalSession>();");
                                var openOptions = persistence.IsSqlPersistence()
                                    ? "new SqlPersistenceOpenSessionOptions()"
                                    : "new NHibernateOpenSessionOptions()";
                                usingBlock.AddStatement($"await transactionalSession.Open({openOptions}, cancellationToken);", s => s.SeparatedFromPrevious());
                                usingBlock.AddStatement("await DispatchAsync(m => transactionalSession.Publish(m, cancellationToken), m => transactionalSession.Send(m, cancellationToken));");
                                usingBlock.AddStatement("await transactionalSession.Commit(cancellationToken);", s => s.SeparatedFromPrevious());
                            }
                            else
                            {
                                usingBlock.AddStatement("var messageSession = _serviceProvider.GetRequiredService<IMessageSession>();");
                                usingBlock.AddStatement("await DispatchAsync(m => messageSession.Publish(m, cancellationToken), m => messageSession.Send(m, cancellationToken));");
                            }
                        });
                });

                @class.AddMethod("Task", "DispatchAsync", method =>
                {
                    method.Private().Async();
                    method.AddParameter("Func<object, Task>", "publishFn");
                    method.AddParameter("Func<object, Task>", "sendFn");
                    method.AddForEachStatement("message", "_publishBuffer", fe =>
                        fe.AddStatement("await publishFn(message);"));
                    method.AddForEachStatement("message", "_sendBuffer", fe =>
                        fe.AddStatement("await sendFn(message);"));
                    method.AddStatement("_publishBuffer.Clear();", s => s.SeparatedFromPrevious());
                    method.AddStatement("_sendBuffer.Clear();");
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