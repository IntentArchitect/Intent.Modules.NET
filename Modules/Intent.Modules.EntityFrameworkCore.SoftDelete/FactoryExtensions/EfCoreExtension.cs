using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.SoftDelete.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.SoftDelete.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.SoftDelete.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class EfCoreExtension : FactoryExtensionBase
{
    public override string Id => "Intent.EntityFrameworkCore.SoftDelete.EfCoreExtension";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        InstallSoftDeleteOnEntities(application);
        InstallSoftDeleteOnDbContext(application);
        InstallSoftDeleteOnEntityTypeConfiguration(application);
    }

    private void InstallSoftDeleteOnEntityTypeConfiguration(IApplication application)
    {
        var entities = application
            .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                TemplateDependency.OnTemplate("Infrastructure.Data.EntityTypeConfiguration"))
            .Where(p => p.Model.HasSoftDeleteEntity() || (p.Model.ParentClass is not null && p.Model.ParentClass.HasSoftDeleteEntity()))
            .Cast<ICSharpFileBuilderTemplate>()
            .ToArray();
        foreach (var entity in entities)
        {
            entity.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                priClass.FindMethod("Configure")
                    ?.AddStatement("builder.HasQueryFilter(t => t.IsDeleted == false);");
            });
        }
    }

    private void InstallSoftDeleteOnDbContext(IApplication application)
    {
        var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));

        dbContext?.CSharpFile.AfterBuild(file =>
        {
            var priClass = file.Classes.First();

            AddSetSoftDeletePropertiesMethod(dbContext, priClass);

            var asyncSaveChanges =
                priClass.Methods.Where(p => p.Name == "SaveChangesAsync").MaxBy(o => o.Parameters.Count);
            var normalSaveChanges = priClass.Methods.Where(p => p.Name == "SaveChanges").MaxBy(o => o.Parameters.Count);

            asyncSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                ?.InsertAbove("SetSoftDeleteProperties();");

            normalSaveChanges?.FindStatement(s => s.HasMetadata("save-changes"))
                ?.InsertAbove("SetSoftDeleteProperties();");
        }, 100);
    }

    private static void AddSetSoftDeletePropertiesMethod(
        ICSharpFileBuilderTemplate template,
        CSharpClass priClass)
    {
        priClass.AddMethod("void", "SetSoftDeleteProperties", method =>
        {
            method.Private();
            method.AddMethodChainStatement("var entities = ChangeTracker", stmt => stmt
                .AddChainStatement("Entries()")
                .AddChainStatement(
                    $"Where(t => t.Entity is {template.GetSoftDeleteInterfaceName()} && t.State == EntityState.Deleted)")
                .AddChainStatement("ToArray()"));

            method.AddIfStatement("!entities.Any()", stmt =>
            {
                stmt.SeparatedFromPrevious();
                stmt.AddStatement("return;");
            });

            method.AddForEachStatement("entry", "entities", stmt =>
            {
                stmt.SeparatedFromPrevious();
                stmt.AddStatement($"var entity = ({template.GetSoftDeleteInterfaceName()})entry.Entity;");
                stmt.AddStatement("entity.IsDeleted = true;");
                stmt.AddStatement("entry.State = EntityState.Modified;");
            });
        });
    }

    private void InstallSoftDeleteOnEntities(IApplication application)
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
                priClass.ImplementsInterface(entity.GetSoftDeleteInterfaceName());
            });
        }
    }
}