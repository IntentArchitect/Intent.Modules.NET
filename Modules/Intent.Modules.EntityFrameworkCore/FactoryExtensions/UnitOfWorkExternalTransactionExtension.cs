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
    /// When the EF DbContext and either dispatch stack's unit-of-work template (MediatR's
    /// UnitOfWorkBehaviour, or Wolverine's UnitOfWorkMiddleware) are present, this extension wires
    /// cross-module changes so that dispatch correctly co-exists with any infrastructure that
    /// externally manages an EF connection/transaction (e.g. NServiceBus ITransactionalSession, raw
    /// ADO.NET orchestrators, etc.):
    ///
    /// 1. Adds <c>HasDbTransaction()</c> to the <c>IUnitOfWork</c> interface so the pipeline behavior
    ///    can detect whether an externally-managed EF transaction is already active.
    ///
    /// 2. Implements <c>HasDbTransaction()</c> on <c>ApplicationDbContext</c> returning
    ///    <c>Database.CurrentTransaction != null</c>.
    ///
    /// 3. Modifies <c>UnitOfWorkBehaviour.Handle</c> (MediatR) and <c>UnitOfWorkMiddleware.Before</c>
    ///    (Wolverine) to skip wrapping in <c>TransactionScope</c> when <c>HasDbTransaction()</c> is
    ///    true — prevents MSDTC escalation when an external party already owns the connection. Both
    ///    are held to the same behavioural bar: run the handler, save, return, with no scope.
    ///    This is only injected when the EF unit-of-work field (<c>_dataSource</c>)
    ///    is actually present on the generated behaviour class, since that field is what the guard
    ///    dereferences.
    ///
    /// This is an EF concern, not a transport/messaging/dispatch concern. Any module that externally
    /// enlists an EF connection benefits automatically once this extension fires.
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
            AddHasDbTransactionToDbContextInterface(application);
            AddHasDbTransactionToDbContext(application);
            ModifyUnitOfWorkBehaviour(application);
            ModifyUnitOfWorkMiddleware(application);
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

        private static void AddHasDbTransactionToDbContextInterface(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateRoles.Application.Common.DbContextInterface);
            if (template == null) return;

            template.CSharpFile.AfterBuild(file =>
            {
                var iface = file.Interfaces.FirstOrDefault();
                if (iface != null && iface.Methods.All(m => m.Name != "HasDbTransaction"))
                    iface.AddMethod("bool", "HasDbTransaction", m => { });
            }, 500);
        }

        private static void AddHasDbTransactionToDbContext(IApplication application)
        {
            // Primary DbContexts fulfill DbContext role; secondary ones fulfill ConnectionStringDbContext.
            // Only patch those that implement IUnitOfWork.
            var roles = new[]
            {
                TemplateRoles.Infrastructure.Data.DbContext,
                TemplateRoles.Infrastructure.Data.ConnectionStringDbContext
            };

            foreach (var template in roles.SelectMany(application.FindTemplateInstances<ICSharpFileBuilderTemplate>))
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    if (@class.FindMethod("HasDbTransaction") != null) return;

                    @class.AddMethod("bool", "HasDbTransaction", m =>
                        m.WithExpressionBody("Database.CurrentTransaction != null"));
                }, 500);
            }
        }

        private static void ModifyUnitOfWorkBehaviour(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                "Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour");
            if (template == null) return;

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.FirstOrDefault();
                if (@class == null) return;

                var handleMethod = @class.FindMethod("Handle");
                if (handleMethod == null) return;

                // Only inject when the EF unit-of-work chain actually created a field on THIS class.
                // Deriving this from template roles is not reliable: the field is only created for the
                // primary DbContext, while the roles also match secondary (connection string) DbContexts,
                // and the ServiceProvider resolution strategy emits a local variable and no field at all.
                var unitOfWorkField = @class.Fields.FirstOrDefault(x => x.Name == "_dataSource");
                if (unitOfWorkField == null) return;

                // Idempotency guard — skip if already modified
                if (handleMethod.Statements.Any(s => s.ToString().Contains("HasDbTransaction"))) return;

                // Match the next() call style already in the method (older MediatR omits cancellationToken)
                var nextInvocation = handleMethod.Statements.Any(s => s.ToString().Contains("next(cancellationToken)"))
                    ? "next(cancellationToken)"
                    : "next()";

                handleMethod.InsertStatement(0,
                    $"if ({unitOfWorkField.Name}.HasDbTransaction())\r\n" +
                    "{\r\n" +
                    "    // External EF transaction active — skip TransactionScope to avoid MSDTC escalation.\r\n" +
                    $"    var result = await {nextInvocation};\r\n" +
                    $"    await {unitOfWorkField.Name}.SaveChangesAsync(cancellationToken);\r\n" +
                    "    return result;\r\n" +
                    "}",
                    s => s.SeparatedFromNext());
            }, 500);
        }

        /// <summary>
        /// Sibling of <see cref="ModifyUnitOfWorkBehaviour"/> for the Wolverine dispatch stack.
        /// Wolverine's <c>UnitOfWorkMiddleware.Before</c> always opens a <c>TransactionScope</c> —
        /// unlike MediatR's <c>UnitOfWorkBehaviour</c>, it has no <c>HasDbTransaction()</c> guard, so
        /// an external EF transaction owner collides with it exactly the way it used to collide with
        /// the unguarded MediatR behaviour. <c>AfterAsync</c> already calls <c>SaveChangesAsync</c>
        /// unconditionally and null-guards the scope (<c>tx?.Complete()</c> / <c>tx?.Dispose()</c>), so
        /// <c>Before</c> returning <c>null</c> here produces the same three effects, in the same
        /// order, as MediatR's guarded path (run the handler, save, return — no scope).
        /// </summary>
        private static void ModifyUnitOfWorkMiddleware(IApplication application)
        {
            // Only inject when an EF DbContext is present — other unit-of-work backends (e.g. Dapr)
            // don't have a data source to call HasDbTransaction() on.
            var hasDbContext = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext).Any()
                || application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext).Any();
            if (!hasDbContext) return;

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                "Intent.Application.Wolverine.UnitOfWorkMiddleware");
            if (template == null) return;

            template.CSharpFile.AfterBuild(file =>
            {
                var beforeMethod = file.Classes.First().FindMethod("Before");
                if (beforeMethod == null) return;

                // Idempotency guard — skip if already modified
                if (beforeMethod.Statements.Any(s => s.ToString().Contains("HasDbTransaction"))) return;

                beforeMethod.InsertStatement(0,
                    "if (dataSource.HasDbTransaction())\r\n" +
                    "{\r\n" +
                    "    // External EF transaction active — skip TransactionScope to avoid MSDTC escalation.\r\n" +
                    "    return null;\r\n" +
                    "}",
                    s => s.SeparatedFromNext());
            }, 500);
        }
    }
}
