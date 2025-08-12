using System;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Entities.SoftDelete.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.SoftDelete.Templates;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.SoftDelete.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EfCoreExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.SoftDelete.EfCoreExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.FindTemplateInstances<IIntentTemplate>(TemplateRoles.Infrastructure.Data.DbContext).Any())
            {
                return;
            }

            InstallSoftDeleteOnEntities(application);
            // TODO: Delete all code related to the below once the new pattern passes tests.
            //InstallSoftDeleteOnDbContext(application); // No longer necessary since we're now using the SoftDeleteInterceptor
            InstallSoftDeleteInterceptor(application);
            InstallSoftDeleteOnEntityTypeConfiguration(application);
        }

        private static void InstallSoftDeleteInterceptor(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template == null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod("AddInfrastructure");
                var addDbContext = method?.FindStatement(s => s.Text.StartsWith("services.AddDbContext")) as IHasCSharpStatements;
                if (addDbContext == null)
                {
                    return;
                }
                addDbContext.AddStatement($"options.AddInterceptors(sp.GetService<{template.GetSoftDeleteEFCoreInterceptorName()}>()!);");
                ((CSharpStatement)addDbContext).InsertAbove($"services.AddSingleton<{template.GetSoftDeleteEFCoreInterceptorName()}>();");
            }, 10);
        }

        private static void InstallSoftDeleteOnEntities(IApplication application)
        {
            var entities = application
                .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                    TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary))
                .Where(p => p.Model.HasSoftDeleteEntity())
                .Cast<ICSharpFileBuilderTemplate>()
                .ToArray();
            foreach (var entity in entities)
            {
                entity.CSharpFile.OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var softDeleteInterfaceName = entity.GetSoftDeleteInterfaceName();
                    if (priClass.Interfaces.All(x => x != softDeleteInterfaceName))
                    {
                        priClass.ImplementsInterface(softDeleteInterfaceName);
                    }

                    priClass.AddMethod("void", "ISoftDelete.SetDeleted", method =>
                    {
                        method.WithoutAccessModifier();
                        method.AddParameter("bool", "isDeleted");
                        method.AddStatement("IsDeleted = isDeleted;");
                    });
                });
            }
        }

        private static void InstallSoftDeleteOnDbContext(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));

            dbContext?.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();

                AddSetSoftDeletePropertiesMethod(dbContext, priClass);

                var asyncSaveChanges = dbContext.GetSaveChangesAsyncMethod();
                var normalSaveChanges = dbContext.GetSaveChangesMethod();

                asyncSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                    ?.InsertAbove("SetSoftDeleteProperties();");

                normalSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                    ?.InsertAbove("SetSoftDeleteProperties();");
            }, 100);
        }

        private static void InstallSoftDeleteOnEntityTypeConfiguration(IApplication application)
        {
            var entities = application
                .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                    TemplateDependency.OnTemplate("Infrastructure.Data.EntityTypeConfiguration"))
                .Where(p => p.Model.HasSoftDeleteEntity() || (p.Model.ParentClass is not null && p.Model.ParentClass.HasSoftDeleteEntity()))
                .Cast<ICSharpFileBuilderTemplate>()
                .ToArray();

            foreach (var entity in entities)
            {
                // if there is no parent, then add the filter query to the class
                // as it means the soft delete stereotype had to have been added to the class
                var addQuery = (entity as IIntentTemplate<ClassModel>).Model.ParentClass is null;

                if (!addQuery)
                {
                    // try find the configuration for the parent
                    var parentConfiguration = application.FindTemplateInstance("Infrastructure.Data.EntityTypeConfiguration", (entity as IIntentTemplate<ClassModel>).Model.ParentClass);

                    // only if there is no parent configuration for the parent, then add to the child
                    if (parentConfiguration is null)
                    {
                        addQuery = true;
                    }
                }

                if (addQuery)
                {
                    entity.CSharpFile.AfterBuild(file =>
                    {
                        var priClass = file.Classes.First();
                        priClass.FindMethod("Configure")
                            ?.AddStatement("builder.HasQueryFilter(t => t.IsDeleted == false);");
                    });
                }
            }
        }

        private static void AddSetSoftDeletePropertiesMethod(
            ICSharpFileBuilderTemplate template,
            CSharpClass priClass)
        {
            template.AddUsing("Microsoft.EntityFrameworkCore.ChangeTracking");
            priClass.AddMethod("void", "SetSoftDeleteProperties", method =>
            {
                method.Private();
                method.AddMethodChainStatement("var entities = ChangeTracker", stmt => stmt
                    .AddChainStatement($"Entries<{template.GetSoftDeleteInterfaceName()}>()")
                    .AddChainStatement("Where(t => t.State == EntityState.Deleted)")
                    .AddChainStatement("ToArray()"));

                method.AddIfStatement("entities.Length == 0", stmt =>
                {
                    stmt.SeparatedFromPrevious();
                    stmt.AddStatement("return;");
                });

                method.AddForEachStatement("entry", "entities", stmt =>
                {
                    stmt.SeparatedFromPrevious();
                    stmt.AddStatement("var entity = entry.Entity;");
                    stmt.AddStatement("entity.SetDeleted(true);");
                    stmt.AddStatement("entry.State = EntityState.Modified;");
                    stmt.AddStatement("UpdateOwnedEntriesRecursive(entry);");
                });

                method.AddStatement("return;", s => s.SeparatedFromPrevious());

                method.AddLocalMethod("void", "UpdateOwnedEntriesRecursive", l =>
                {
                    l.AddParameter("EntityEntry", "entry");

                    l.AddStatement(new CSharpAssignmentStatement(
                        new CSharpVariableDeclaration("ownedReferencedEntries"),
                        new CSharpStatement("entry.References")
                            .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.TargetEntry != null")).OnNewLine())
                            .AddInvocation("Select", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.TargetEntry!")).OnNewLine())
                            .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.State == EntityState.Deleted && x.Metadata.IsOwned()")).OnNewLine())
                    ));

                    l.AddForEachStatement("ownedEntry", "ownedReferencedEntries",
                        @for =>
                        {
                            @for.AddStatement("ownedEntry.State = EntityState.Unchanged;");
                            @for.AddStatement("UpdateOwnedEntriesRecursive(ownedEntry);");
                        });

                    l.AddStatement(new CSharpAssignmentStatement(
                        new CSharpVariableDeclaration("ownedCollectionEntries"),
                        new CSharpStatement("entry.Collections")
                            .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.IsLoaded && x.CurrentValue != null")).OnNewLine())
                            .AddInvocation("SelectMany", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.CurrentValue!.Cast<object>().Select(Entry)")).OnNewLine())
                            .AddInvocation("Where", i => i.AddArgument(new CSharpLambdaBlock("x"), a => a.WithExpressionBody("x.State == EntityState.Deleted && x.Metadata.IsOwned()")).OnNewLine())
                    ), s => s.SeparatedFromPrevious());

                    l.AddForEachStatement("ownedEntry", "ownedCollectionEntries",
                        @for =>
                        {
                            @for.AddStatement("ownedEntry.State = EntityState.Unchanged;");
                            @for.AddStatement("UpdateOwnedEntriesRecursive(ownedEntry);");
                        });
                });
            });
        }
    }
}