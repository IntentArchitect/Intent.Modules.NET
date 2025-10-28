using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.DiffAudit.Settings;
using Intent.Modules.EntityFrameworkCore.DiffAudit.Templates;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DiffAudit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EfCoreExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.DiffAudit.EfCoreExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
            dbContext?.CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var ctor = priClass.Constructors.First();
                if (!ctor.Parameters.Any(p => p.Type == dbContext.GetCurrentUserServiceInterfaceName() && p.Name == "currentUserService"))
                {
                    ctor.AddParameter(dbContext.GetCurrentUserServiceInterfaceName(), "currentUserService",
                        param => param.IntroduceReadonlyField());
                }
            });

            dbContext?.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Linq");
                file.AddUsing("System.Collections.Generic");

                var priClass = file.Classes.First();

                var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary);
                var auditTableName = string.Empty;

                foreach (var template in templates)
                {
                    if (!template.TryGetModel<ClassModel>(out var templateModel) ||
                    !templateModel.HasStereotype("92e433aa-4858-4129-849f-ef0f9f0bf9e6")) // Audit Table StereoType
                    {
                        continue;
                    }
                    else
                    {
                        auditTableName = templateModel.Name;
                    }
                }

                if (!string.IsNullOrEmpty(auditTableName))
                {
                    AddChangeAuditingMethod(dbContext, priClass, auditTableName);

                    var asyncSaveChanges = dbContext.GetSaveChangesAsyncMethod();
                    var normalSaveChanges = dbContext.GetSaveChangesMethod();

                    asyncSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                        ?.InsertAbove("LogDiffAudit();");

                    normalSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                        ?.InsertAbove("LogDiffAudit();");
                }
                else
                {
                    if (templates.Any())
                    {
                        if (templates.First().TryGetModel<ClassModel>(out var templateModel))
                        {
                            throw new ElementException(templateModel.InternalElement, "No Audit Log Entity found. Either uninstall `Intent.EntityFramework.DiffAudit` or add a new entity and apply the `Audit Log` stereotype.");
                        }
                    }
                    else
                    {
                        throw new System.Exception("No Audit Log Entity found. Either uninstall `Intent.EntityFramework.DiffAudit` or add a new entity and apply the `Audit Log` stereotype.");
                    }
                }

            }, 101);
        }

        private static void AddChangeAuditingMethod(ICSharpFileBuilderTemplate template, CSharpClass dbContextClass, string logEntityName)
        {
            var changeAuditingTypeName = template.GetDiffAuditInterfaceName();

            dbContextClass.AddMethod("void", "LogDiffAudit", method =>
            {
                method.Private();
                method.AddMethodChainStatement("var diffAuditEntries = ChangeTracker.Entries()", chain =>
                {
                    chain.AddChainStatement(new CSharpInvocationStatement("Where").AddArgument(new CSharpLambdaBlock("entry")
                        .WithExpressionBody(@$"entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified &&
                                entry.Entity is {changeAuditingTypeName}"))
                        .WithoutSemicolon());
                    chain.AddChainStatement(new CSharpInvocationStatement("Select").AddArgument(new CSharpLambdaBlock("entry")
                        .WithExpressionBody(@$"new
                        {{
                            entry.State,
                            entry.Entity,
                            entry.Properties
                        }}"))
                        .WithoutSemicolon());
                    chain.AddChainStatement(new CSharpInvocationStatement("ToArray"))
                        .WithoutSemicolon();
                });

                method.AddIfStatement("!diffAuditEntries.Any()", @if => @if.AddStatement("return;"));

                method.AddStatement($"var auditEntries = new List<{logEntityName}>();", s => s.SeparatedFromPrevious());

                string userIdentityProperty;
                switch (template.ExecutionContext.Settings.GetDiffAudit().UserIdentityToAudit().AsEnum())
                {
                    case Settings.DiffAudit.UserIdentityToAuditOptionsEnum.UserName:
                        userIdentityProperty = "UserName";
                        break;
                    case Settings.DiffAudit.UserIdentityToAuditOptionsEnum.UserId:
                    default:
                        userIdentityProperty = "UserId";
                        break;
                }

                method.AddStatement(
                    $"var userIdentifier = _currentUserService.{userIdentityProperty} ?? throw new InvalidOperationException(\"{userIdentityProperty} is null\");",
                    s => s.SeparatedFromPrevious());
                method.AddStatement(
                    "var timestamp = DateTimeOffset.UtcNow;");

                method.AddForEachStatement("entry", "diffAuditEntries", forStmt =>
                {
                    forStmt.AddStatement("var entityName = entry.Entity.GetType().Name;");
                    forStmt.AddStatement("var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());");

                    forStmt.AddSwitchStatement("entry.State", switchStmt => switchStmt
                        .AddCase("EntityState.Added", block => block
                        .AddStatement($"auditEntries.Add(new {logEntityName}" + @"
                                {
                                    TableName = entityName,
                                    Key = primaryKey?.CurrentValue?.ToString(),
                                    ColumnName = ""EntityCreated"",
                                    OldValue = null,
                                    NewValue = null,
                                    ChangedBy = userIdentifier,
                                    ChangedDate = timestamp,
                                });")
                            .AddForEachStatement("prop", "entry.Properties", forEachStatement =>
                            {
                                forEachStatement.AddStatement($"auditEntries.Add(new {logEntityName}" + @"
                                {
                                    TableName = entityName,
                                    Key = primaryKey?.CurrentValue?.ToString(),
                                    ColumnName = prop.Metadata.Name,
                                    OldValue = null,
                                    NewValue = prop.CurrentValue?.ToString(),
                                    ChangedBy = userIdentifier,
                                    ChangedDate = timestamp,
                                });");
                            })
                            .WithBreak())
                        .AddCase("EntityState.Deleted", block => block
                            .AddStatement($"auditEntries.Add(new {logEntityName}" + @"
                                {
                                    TableName = entityName,
                                    Key = primaryKey?.CurrentValue?.ToString(),
                                    ColumnName = ""EntityDeleted"",
                                    OldValue = null,
                                    NewValue = null,
                                    ChangedBy = userIdentifier,
                                    ChangedDate = timestamp,
                                });")
                            .WithBreak())
                        .AddCase("EntityState.Modified", block => block
                            .AddForEachStatement("prop", "entry.Properties", forEachStatement =>
                            {
                                forEachStatement.AddIfStatement("!Equals(prop.OriginalValue, prop.CurrentValue)", ifStatement =>
                                {
                                    ifStatement.AddStatement($"auditEntries.Add(new {logEntityName}" + @"
                                    {
                                        TableName = entityName,
                                        Key = primaryKey?.CurrentValue?.ToString(),
                                        ColumnName = prop.Metadata.Name,
                                        OldValue = prop.OriginalValue?.ToString(),
                                        NewValue = prop.CurrentValue?.ToString(),
                                        ChangedBy = userIdentifier,
                                        ChangedDate = timestamp,
                                    });");
                                });
                            })
                            .WithBreak())
                        .AddDefault(block => block
                            .AddStatement("throw new ArgumentOutOfRangeException();")));
                });



                method.AddStatement($"{GetDbSetName(template, logEntityName)}.AddRange(auditEntries);");
            });
        }

        public static string GetDbSetName(ICSharpFileBuilderTemplate template, string logEntityName)
        {
            if (template.ExecutionContext.Settings.GetSetting("ac0a788e-d8b3-4eea-b56d-538608f1ded9", "6010e890-6e2d-4812-9969-ffbdb8f93d87")?.Value == "same-as-entity")
            {
                return logEntityName.ToPascalCase();
            }

            return logEntityName.Pluralize();
        }
    }
}