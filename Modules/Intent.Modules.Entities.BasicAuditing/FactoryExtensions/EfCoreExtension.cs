using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Entities.BasicAuditing.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.BasicAuditing.Settings;
using Intent.Modules.Entities.BasicAuditing.Templates;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.BasicAuditing.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EfCoreExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.BasicAuditing.EfCoreExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallDbContext(application);
            UpdateEntities(application);
        }

        private static void InstallDbContext(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
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
            var entityStateClasses = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Interface));
            foreach (var entityTemplate in entityStateClasses)
            {
                // This needs to be an AfterBuild because DomainEntityTemplate automatically adds [IntentManaged(Mode.Fully, Body = Mode.Merge)] to
                // all methods in its own AfterBuild which we don't want for the ones we're adding here.
                entityTemplate.CSharpFile.AfterBuild(file =>
                {
                    if (file.Interfaces.Any())
                    {
                        UpdateEntityInterface(file.Interfaces.First(), entityTemplate);
                        var model = file.Interfaces.First().GetMetadata<ClassModel>("model");
                        if (entityTemplate.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, model, out var relatedTemplate))
                        {
                            UpdateEntityClass(relatedTemplate.CSharpFile.Classes.First(), relatedTemplate, false);
                        }
                    }
                    else if (file.Classes.Any())
                    {
                        UpdateEntityClass(file.Classes.First(), entityTemplate, true);
                    }
                }, 100);
            }
        }

        private static void UpdateEntityInterface(CSharpInterface @interface, ICSharpFileBuilderTemplate entityTemplate)
        {
            var model = @interface.GetMetadata<ClassModel>("model");
            if (!model.HasBasicAuditing()) { return; }

            var auditableInterfaceName = entityTemplate.GetAuditableInterfaceName();
            @interface.ImplementsInterfaces(auditableInterfaceName);
        }

        private static void UpdateEntityClass(CSharpClass @class, ICSharpFileBuilderTemplate entityTemplate, bool includeAuditInterface)
        {
            var model = @class.GetMetadata<ClassModel>("model");
            if (!model.HasBasicAuditing()) { return; }

            var auditableInterfaceName = entityTemplate.GetAuditableInterfaceName();
            if (includeAuditInterface)
            {
                @class.ImplementsInterface(auditableInterfaceName);
            }

            string userIdType = TemplateHelper.GetUserIdentifierType(entityTemplate.ExecutionContext);

            @class.AddMethod("void", "SetCreated", method =>
            {
                method.AddParameter(entityTemplate.UseType(userIdType), "createdBy");
                method.AddParameter("DateTimeOffset", "createdDate");
                method.IsExplicitImplementationFor(auditableInterfaceName);
                method.WithExpressionBody("(CreatedBy, CreatedDate) = (createdBy, createdDate)");
            });

            @class.AddMethod("void", "SetUpdated", method =>
            {
                method.AddParameter(entityTemplate.UseType(userIdType), "updatedBy");
                method.AddParameter("DateTimeOffset", "updatedDate");
                method.IsExplicitImplementationFor(auditableInterfaceName);
                method.WithExpressionBody("(UpdatedBy, UpdatedDate) = (updatedBy, updatedDate)");
            });
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
                    Property = new Func<string, {template.UseType("Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry")}>(entry.Property),
                    Auditable = ({auditableTypeName})entry.Entity
                }}"))
                        .WithoutSemicolon());
                    chain.AddChainStatement(new CSharpInvocationStatement("ToArray"))
                        .WithoutSemicolon();
                });

                method.AddIfStatement("!auditableEntries.Any()", @if => @if.AddStatement("return;"));

                string userIdentityProperty;
                switch (template.ExecutionContext.Settings.GetBasicAuditing().UserIdentityToAudit().AsEnum())
                {
                    case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserName:
                        userIdentityProperty = "UserName";
                        break;
                    case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserId:
                    default:
                        userIdentityProperty = "UserId";
                        break;
                }
                method.AddStatement(
                    $"var userIdentifier = _currentUserService.{userIdentityProperty} ?? throw new InvalidOperationException(\"{userIdentityProperty} is null\");",
                    s => s.SeparatedFromPrevious());
                method.AddStatement(
                    "var timestamp = DateTimeOffset.UtcNow;");

                method.AddForEachStatement("entry", "auditableEntries", forStmt =>
                {
                    forStmt.AddSwitchStatement("entry.State", switchStmt => switchStmt
                        .AddCase("EntityState.Added", block => block
                            .AddStatement("entry.Auditable.SetCreated(userIdentifier, timestamp);")
                            .WithBreak())
                        .AddCase("EntityState.Modified or EntityState.Deleted", block => block
                            .AddStatement("entry.Auditable.SetUpdated(userIdentifier, timestamp);")
                            .AddStatement("entry.Property(\"CreatedBy\").IsModified = false;")
                            .AddStatement("entry.Property(\"CreatedDate\").IsModified = false;")
                            .WithBreak())
                        .AddDefault(block => block
                            .AddStatement("throw new ArgumentOutOfRangeException();")));
                });
            });
        }
    }
}