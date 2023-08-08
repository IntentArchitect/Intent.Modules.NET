using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.EntityFrameworkCore.Shared;

public static class DbContextHelpers
{
    /// <summary>
    /// Gets the <c>Task&lt;int&gt; SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)</c>
    /// method from the provided <paramref name="template"/>'s Class, if one does not exist, it will be added first.
    /// </summary>
    public static CSharpClassMethod GetSaveChangesAsyncMethod(this ICSharpFileBuilderTemplate template)
    {
        var @class = template.CSharpFile.Classes.First();

        var saveMethod = @class.Methods
            .OrderByDescending(x => x.Parameters.Count)
            .SingleOrDefault(x => x.Name == "SaveChangesAsync");

        if (saveMethod == null)
        {
            @class.InsertMethod(0, $"{template.UseType("System.Threading.Tasks.Task")}<int>", "SaveChangesAsync", method =>
            {
                saveMethod = method;
                method.Override().Async()
                    .AddParameter("bool", "acceptAllChangesOnSuccess")
                    .AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"))
                    .AddStatement("return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);", stmt => stmt.AddMetadata("save-changes", true));
            });
        }

        return saveMethod;
    }

    /// <summary>
    /// Gets the <c>int SaveChanges(bool acceptAllChangesOnSuccess)</c>
    /// method from the provided <paramref name="template"/>'s Class, if one does not exist, it will be added first.
    /// </summary>
    public static CSharpClassMethod GetSaveChangesMethod(this ICSharpFileBuilderTemplate template)
    {
        var @class = template.CSharpFile.Classes.First();

        var saveMethod = @class.Methods
            .OrderByDescending(x => x.Parameters.Count)
            .SingleOrDefault(x => x.Name == "SaveChanges");

        if (saveMethod == null)
        {
            @class.InsertMethod(0, "int", "SaveChanges", method =>
            {
                saveMethod = method;
                method.Override()
                    .AddParameter("bool", "acceptAllChangesOnSuccess")
                    .AddStatement("return base.SaveChanges(acceptAllChangesOnSuccess);", stmt => stmt.AddMetadata("save-changes", true));
            });
        }

        return saveMethod;
    }
}