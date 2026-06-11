using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    /// <summary>
    /// When both the EF DbContext and MediatR UnitOfWorkBehaviour templates are present, this extension
    /// wires three cross-module changes so that the MediatR pipeline correctly co-exists with any
    /// infrastructure that externally manages an EF connection/transaction (e.g. NServiceBus
    /// ITransactionalSession, raw ADO.NET orchestrators, etc.):
    ///
    /// 1. Adds <c>HasDbTransaction()</c> to the <c>IUnitOfWork</c> interface so the pipeline behavior
    ///    can detect whether an externally-managed EF transaction is already active.
    ///
    /// 2. Implements <c>HasDbTransaction()</c> on <c>ApplicationDbContext</c> returning
    ///    <c>Database.CurrentTransaction != null</c>.
    ///
    /// 3. Modifies <c>UnitOfWorkBehaviour.Handle</c> to skip wrapping in <c>TransactionScope</c> when
    ///    <c>HasDbTransaction()</c> is true — prevents MSDTC escalation when an external party already
    ///    owns the connection.
    ///
    /// This is an EF concern, not a transport/messaging concern. Any module that externally enlists an
    /// EF connection benefits automatically once this extension fires.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Comments = Mode.Ignore)]
    public class UnitOfWorkExternalTransactionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.UnitOfWorkExternalTransactionExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 500;

        [IntentManaged(Mode.Ignore)]
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
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

                handleMethod.InsertStatement(0, """
                    if (_dataSource.HasDbTransaction())
                    {
                        // External EF transaction active — skip TransactionScope to avoid MSDTC escalation.
                        var result = await next(cancellationToken);
                        await _dataSource.SaveChangesAsync(cancellationToken);
                        return result;
                    }
                    """, s => s.SeparatedFromNext());
            }, 500);
        }
    }
}