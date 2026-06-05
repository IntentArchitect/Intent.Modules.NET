using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.FactoryExtensions
{
    /// <summary>
    /// When OutboxPattern = EntityFramework, this extension wires three cross-module changes required
    /// for NServiceBus transactional session co-existence with the MediatR UnitOfWork pipeline:
    ///
    /// 1. Adds HasDbTransaction() to the IUnitOfWork interface so UnitOfWorkBehaviour can detect
    ///    an externally-managed EF transaction (e.g. NSB inbound handler path).
    ///
    /// 2. Implements HasDbTransaction() on ApplicationDbContext (Database.CurrentTransaction != null).
    ///
    /// 3. Modifies UnitOfWorkBehaviour.Handle to skip TransactionScope when HasDbTransaction()
    ///    is true — prevents MSDTC escalation when an external transaction already owns the connection.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NServiceBusUnitOfWorkConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.NServiceBus.NServiceBusUnitOfWorkConfiguratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 500;

        [IntentManaged(Mode.Ignore)]
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetNServiceBusSettings().OutboxPattern().IsEntityFramework())
                return;

            AddHasDbTransactionToInterface(application);
            AddHasDbTransactionToDbContext(application);
            ModifyUnitOfWorkBehaviour(application);
        }

        private static void AddHasDbTransactionToInterface(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                "Intent.Entities.Repositories.Api.UnitOfWorkInterface");
            if (template == null) return;

            template.CSharpFile.AfterBuild(file =>
            {
                // IUnitOfWork is generated as an interface
                var iface = file.Interfaces.FirstOrDefault();
                if (iface != null)
                {
                    if (iface.Methods.All(m => m.Name != "HasDbTransaction"))
                        iface.AddMethod("bool", "HasDbTransaction", m => { });
                    return;
                }

                // Fallback: some older SDK versions expose the interface via Classes
                var @class = file.Classes.FirstOrDefault();
                if (@class != null && @class.FindMethod("HasDbTransaction") == null)
                    @class.AddMethod("bool", "HasDbTransaction", m => { });
            }, 500);
        }

        private static void AddHasDbTransactionToDbContext(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateRoles.Infrastructure.Data.DbContext);
            if (template == null) return;

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                if (@class.FindMethod("HasDbTransaction") != null) return;

                @class.AddMethod("bool", "HasDbTransaction", m =>
                    m.WithExpressionBody("Database.CurrentTransaction != null"));
            }, 500);
        }

        private static void ModifyUnitOfWorkBehaviour(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                "Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour");
            if (template == null) return;

            template.CSharpFile.AfterBuild(file =>
            {
                var handleMethod = file.Classes.First().FindMethod("Handle");
                if (handleMethod == null) return;

                // Idempotency guard — skip if already modified
                if (handleMethod.Statements.Any(s => s.ToString().Contains("HasDbTransaction"))) return;

                handleMethod.Statements.Clear();

                handleMethod.AddIfStatement("_dataSource.HasDbTransaction()", b =>
                {
                    b.AddStatement("// External transaction active (e.g. NServiceBus ITransactionalSession joined EF).");
                    b.AddStatement("// Skip TransactionScope to avoid MSDTC escalation.");
                    b.AddStatement("var result = await next(cancellationToken);");
                    b.AddStatement("await _dataSource.SaveChangesAsync(cancellationToken);");
                    b.AddStatement("return result;");
                });

                handleMethod.AddStatement("""
                    using (var transaction = new TransactionScope(
                        TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                        TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var response = await next(cancellationToken);
                        await _dataSource.SaveChangesAsync(cancellationToken);
                        transaction.Complete();
                        return response;
                    }
                    """, s => s.SeparatedFromPrevious());
            }, 500);
        }
    }
}
