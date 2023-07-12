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
            InstallInterfaceOnEntities(application);
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

                var priClass = file.Classes.First();

                AddSetAuditableFieldsMethod(dbContext, priClass);

                var asyncSaveChanges = priClass.Methods.Where(p => p.Name == "SaveChangesAsync").MaxBy(o => o.Parameters.Count);
                var normalSaveChanges = priClass.Methods.Where(p => p.Name == "SaveChanges").MaxBy(o => o.Parameters.Count);

                asyncSaveChanges?.Statements.Insert(0, "SetAuditableFields();");

                normalSaveChanges?.Statements.Insert(0, "SetAuditableFields();");
            }, 100);
        }

        private static void InstallInterfaceOnEntities(IApplication application)
        {
            var entityStateClasses = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Interface));
            foreach (var entity in entityStateClasses)
            {
                entity.CSharpFile.OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var model = priClass.GetMetadata<ClassModel>("model");
                    if (!model.HasBasicAudit()) { return; }
                    priClass.ImplementsInterface(entity.GetAuditableInterfaceName());
                });
            }
        }

        private static void AddSetAuditableFieldsMethod(
            ICSharpFileBuilderTemplate template,
            CSharpClass priClass)
        {
            priClass.AddMethod("void", "SetAuditableFields", method =>
            {
                method.Private();
                method.AddStatements(@"
            var userName = _currentUserService.UserName;
            var timestamp = DateTimeOffset.UtcNow;
            var entries = ChangeTracker.Entries().ToArray();");
                method.AddForEachStatement("entry", "entries", forStmt =>
                {
                    forStmt.AddIfStatement($"entry.Entity is not {template.GetAuditableInterfaceName()} auditable",
                        ifStmt => ifStmt.AddStatement("continue;"));

                    forStmt.AddSwitchStatement("entry.State", switchStmt => switchStmt
                        .AddCase("EntityState.Modified or EntityState.Deleted", block => block
                            .AddStatement("auditable.UpdatedBy = userName;")
                            .AddStatement("auditable.UpdatedDate = timestamp;")
                            .WithBreak())
                        .AddCase("EntityState.Added", block => block
                            .AddStatement("auditable.CreatedBy = userName;")
                            .AddStatement("auditable.CreatedDate = timestamp;")
                            .WithBreak())
                        .AddCase("EntityState.Detached")
                        .AddCase("EntityState.Unchanged", block => block.WithBreak())
                        .AddDefault(block => block
                            .AddStatement("throw new ArgumentOutOfRangeException();")));
                });
            });
        }
    }
}