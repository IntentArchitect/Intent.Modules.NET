using System.Collections.Generic;
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
using static Intent.Entities.BasicAuditing.Api.AuditRoles;

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

        private static bool AnyAuditFieldsEnabled(IApplication application)
        {
            var auditSettings = application.Settings.GetBasicAuditing();
            return auditSettings.HasCreatedByField() ||
                auditSettings.HasCreatedDateField() ||
                auditSettings.HasUpdatedByField() ||
                auditSettings.HasUpdatedDateField();
        }

        private static void InstallDbContext(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
            if (dbContext == null)
            {
                return;
            }

            if (!AnyAuditFieldsEnabled(application))
            {
                return;
            }

            dbContext.CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var ctor = priClass.Constructors.First();
                ctor.AddParameter(dbContext.GetCurrentUserServiceInterfaceName(), "currentUserService",
                    param => param.IntroduceReadonlyField());
            });

            dbContext.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Linq");

                var priClass = file.Classes.First();

                AddSetAuditableFieldsMethod(dbContext, priClass);

                var asyncSaveChanges = dbContext.GetSaveChangesAsyncMethod();
                var normalSaveChanges = dbContext.GetSaveChangesMethod();

                asyncSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                    ?.InsertAbove("await SetAuditableFieldsAsync();");

                normalSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                    ?.InsertAbove("SetAuditableFieldsAsync().GetAwaiter().GetResult();");
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
            var auditSettings = entityTemplate.ExecutionContext.Settings.GetBasicAuditing();

            AddAuditMethod(
                @class, entityTemplate, auditableInterfaceName, userIdType,
                "SetCreated", auditSettings.HasCreatedByField(), auditSettings.HasCreatedDateField(),
                model.GetAuditField(CreatedBy), model.GetAuditField(CreatedDate),
                "createdBy", "createdDate");

            AddAuditMethod(
                @class, entityTemplate, auditableInterfaceName, userIdType,
                "SetUpdated", auditSettings.HasUpdatedByField(), auditSettings.HasUpdatedDateField(),
                model.GetAuditField(UpdatedBy), model.GetAuditField(UpdatedDate),
                "updatedBy", "updatedDate");
        }

        private static void AddAuditMethod(
            CSharpClass @class, ICSharpFileBuilderTemplate entityTemplate, string auditableInterfaceName, string userIdType,
            string methodName, bool hasFirstField, bool hasSecondField,
            AttributeModel firstAttribute, AttributeModel secondAttribute,
            string firstParamName, string secondParamName)
        {
            if (!hasFirstField && !hasSecondField)
            {
                return;
            }
            if ((hasFirstField && firstAttribute == null) || (hasSecondField && secondAttribute == null))
            {
                return;
            }

            @class.AddMethod("void", methodName, method =>
            {
                if (hasFirstField)
                {
                    method.AddParameter(entityTemplate.UseType(userIdType), firstParamName);
                }
                if (hasSecondField)
                {
                    method.AddParameter("DateTimeOffset", secondParamName);
                }
                method.IsExplicitImplementationFor(auditableInterfaceName);

                var targets = new List<string>();
                var values = new List<string>();
                if (hasFirstField)
                {
                    targets.Add(firstAttribute.Name);
                    values.Add(firstParamName);
                }
                if (hasSecondField)
                {
                    targets.Add(secondAttribute.Name);
                    values.Add(secondParamName);
                }

                method.WithExpressionBody(targets.Count == 1
                    ? $"{targets[0]} = {values[0]}"
                    : $"({string.Join(", ", targets)}) = ({string.Join(", ", values)})");
            });
        }

        private static void AddSetAuditableFieldsMethod(
            ICSharpFileBuilderTemplate template,
            CSharpClass priClass)
        {
            var auditableTypeName = template.GetAuditableInterfaceName();
            var auditSettings = template.ExecutionContext.Settings.GetBasicAuditing();
            var hasCreatedBy = auditSettings.HasCreatedByField();
            var hasCreatedDate = auditSettings.HasCreatedDateField();
            var hasUpdatedBy = auditSettings.HasUpdatedByField();
            var hasUpdatedDate = auditSettings.HasUpdatedDateField();
            var relevantEntityStates = hasCreatedBy || hasCreatedDate
                ? "EntityState.Added or EntityState.Deleted or EntityState.Modified"
                : "EntityState.Deleted or EntityState.Modified";

            priClass.AddMethod(template.UseType("System.Threading.Tasks.Task"), "SetAuditableFieldsAsync", method =>
            {
                method.Async();
                method.Private();
                method.AddMethodChainStatement("var auditableEntries = ChangeTracker.Entries()", chain =>
                {
                    chain.AddChainStatement(new CSharpInvocationStatement("Where").AddArgument(new CSharpLambdaBlock("entry")
                        .WithExpressionBody(@$"entry.State is {relevantEntityStates} &&
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
                        userIdentityProperty = "Name";
                        break;
                    case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserId:
                        default:
                        userIdentityProperty = "Id";
                        break;
                }
                method.AddStatement(
                    $"var userIdentifier = (await _currentUserService.GetAsync())?.{userIdentityProperty} ?? throw new InvalidOperationException(\"{userIdentityProperty} is null\");",
                    s => s.SeparatedFromPrevious());
                method.AddStatement(
                    "var timestamp = DateTimeOffset.UtcNow;");

                method.AddForEachStatement("entry", "auditableEntries", forStmt =>
                {
                    forStmt.AddSwitchStatement("entry.State", switchStmt =>
                    {
                        if (hasCreatedBy || hasCreatedDate)
                        {
                            switchStmt.AddCase("EntityState.Added", block =>
                            {
                                var args = string.Join(", ", new[] { hasCreatedBy ? "userIdentifier" : null, hasCreatedDate ? "timestamp" : null }.Where(a => a != null));
                                block.AddStatement($"entry.Auditable.SetCreated({args});");
                                block.WithBreak();
                            });
                        }
                        if (hasCreatedBy || hasCreatedDate || hasUpdatedBy || hasUpdatedDate)
                        {
                            switchStmt.AddCase("EntityState.Modified or EntityState.Deleted", block =>
                            {
                                if (hasUpdatedBy || hasUpdatedDate)
                                {
                                    var args = string.Join(", ", new[] { hasUpdatedBy ? "userIdentifier" : null, hasUpdatedDate ? "timestamp" : null }.Where(a => a != null));
                                    block.AddStatement($"entry.Auditable.SetUpdated({args});");
                                }
                                if (hasCreatedBy)
                                {
                                    block.AddStatement($"entry.Property(\"{ResolveFieldName(auditSettings.CreatedByFieldName(), CreatedBy)}\").IsModified = false;");
                                }
                                if (hasCreatedDate)
                                {
                                    block.AddStatement($"entry.Property(\"{ResolveFieldName(auditSettings.CreatedDateFieldName(), CreatedDate)}\").IsModified = false;");
                                }
                                block.WithBreak();
                            });
                        }
                        switchStmt.AddDefault(block => block
                            .AddStatement("throw new ArgumentOutOfRangeException();"));
                    });
                });
            });
        }

        private static string ResolveFieldName(string configuredName, string defaultName)
        {
            return string.IsNullOrWhiteSpace(configuredName) ? defaultName : configuredName;
        }
    }
}
