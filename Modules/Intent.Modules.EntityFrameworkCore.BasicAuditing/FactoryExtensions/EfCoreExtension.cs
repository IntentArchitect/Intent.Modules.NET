using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.BasicAuditing.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.BasicAuditing.Templates;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.BasicAuditing.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EfCoreExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.BasicAuditing.EfCoreExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallDbContext(application);
            UpdateEntities(application);
        }

        private static void InstallDbContext(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Infrastructure.Data.DbContext));
            dbContext?.CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var ctor = priClass.Constructors.First();
                ctor.AddParameter(dbContext.GetCurrentUserServiceInterfaceName(), "currentUserService",
                    param => param.IntroduceReadonlyField());
            });

            dbContext?.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Linq");

                var priClass = file.Classes.First();

                AddSetAuditableFieldsMethod(dbContext, priClass);

                var asyncSaveChanges = dbContext.GetSaveChangesAsyncMethod();
                var normalSaveChanges = dbContext.GetSaveChangesMethod();

                asyncSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                    ?.InsertAbove("SetAuditableFields();");

                normalSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                    ?.InsertAbove("SetAuditableFields();");
            }, 100);
        }

        private static void UpdateEntities(IApplication application)
        {
            var entityStateClasses = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Interface));
            foreach (var entity in entityStateClasses)
            {
                // This needs to be an AfterBuild because DomainEntityTemplate automatically adds [IntentManaged(Mode.Fully, Body = Mode.Merge)] to
                // all methods in its own AfterBuild which we don't want for the ones we're adding here.
                entity.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");
                    if (!model.HasBasicAuditing()) { return; }

                    var auditableInterfaceName = entity.GetAuditableInterfaceName();
                    @class.ImplementsInterface(auditableInterfaceName);

                    @class.AddMethod("void", "SetCreated", method =>
                    {
                        method.AddParameter("string", "createdBy");
                        method.AddParameter("DateTimeOffset", "createdDate");
                        method.IsExplicitImplementationFor(auditableInterfaceName);
                        method.WithExpressionBody("(CreatedBy, CreatedDate) = (createdBy, createdDate)");
                    });

                    @class.AddMethod("void", "SetUpdated", method =>
                    {
                        method.AddParameter("string", "updatedBy");
                        method.AddParameter("DateTimeOffset", "updatedDate");
                        method.IsExplicitImplementationFor(auditableInterfaceName);
                        method.WithExpressionBody("(UpdatedBy, UpdatedDate) = (updatedBy, updatedDate)");
                    });
                }, 100);
            }
        }

        private static void AddSetAuditableFieldsMethod(
            ICSharpFileBuilderTemplate template,
            CSharpClass priClass)
        {
            var auditableTypeName = template.GetAuditableInterfaceName();

            priClass.AddMethod("void", "SetAuditableFields", method =>
            {
                method.Private();
                method.AddMethodChainStatement("var auditableEntries = ChangeTracker.Entries()", chain =>
                {
                    chain.AddChainStatement(new CSharpInvocationStatement("Where").AddArgument(new CSharpLambdaBlock("entry")
                        .WithExpressionBody(@$"entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified &&
                                entry.Entity is {auditableTypeName}"))
                        .WithoutSemicolon());
                    chain.AddChainStatement(new CSharpInvocationStatement("Select").AddArgument(new CSharpLambdaBlock("entry")
                        .WithExpressionBody(@$"new
                {{
                    entry.State,
                    Auditable = ({auditableTypeName})entry.Entity
                }}"))
                        .WithoutSemicolon());
                    chain.AddChainStatement(new CSharpInvocationStatement("ToArray"))
                        .WithoutSemicolon();
                });

                method.AddIfStatement("!auditableEntries.Any()", @if => @if.AddStatement("return;"));

                method.AddStatement(
                    "var userName = _currentUserService.UserId ?? throw new InvalidOperationException(\"UserId is null\");",
                    s => s.SeparatedFromPrevious());
                method.AddStatement(
                    "var timestamp = DateTimeOffset.UtcNow;");

                method.AddForEachStatement("entry", "auditableEntries", forStmt =>
                {
                    forStmt.AddSwitchStatement("entry.State", switchStmt => switchStmt
                        .AddCase("EntityState.Added", block => block
                            .AddStatement("entry.Auditable.SetCreated(userName, timestamp);")
                            .WithBreak())
                        .AddCase("EntityState.Modified or EntityState.Deleted", block => block
                            .AddStatement("entry.Auditable.SetUpdated(userName, timestamp);")
                            .WithBreak())
                        .AddDefault(block => block
                            .AddStatement("throw new ArgumentOutOfRangeException();")));
                });
            });
        }
    }
}