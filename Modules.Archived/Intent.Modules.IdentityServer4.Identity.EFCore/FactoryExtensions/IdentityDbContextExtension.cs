using System.Linq;
using Intent.Engine;
using Intent.IdentityServer4.Identity.EFCore.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class IdentityDbContextExtension : FactoryExtensionBase
{
    public override string Id => "Intent.IdentityServer4.Identity.EFCore.IdentityDbContextExtension";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    /// <summary>
    /// This is an example override which would extend the
    /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
    /// See <see cref="FactoryExtensionBase"/> for all available overrides.
    /// </summary>
    /// <remarks>
    /// It is safe to update or delete this method.
    /// </remarks>
    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<DbContextTemplate>(DbContextTemplate.TemplateId);
        if (template is null)
        {
            return;
        }
        template.FulfillsRole("Infrastructure.Data.IdentityDbContext");

        // IdentityServer4.EntityFramework is no longer in production and
        // has a hard dependency on Automapper 10, only way to resolve compilation
        // issue with newer Automapper is to actually install it on this project
        template.AddNugetDependency(NugetPackages.Automapper);

        template.AddNugetDependency(NugetPackages.IdentityServer4EntityFramework);
        template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(template.OutputTarget.GetProject()));

        template.CSharpFile.OnBuild(file =>
        {
            var @class = file.Classes.First();
            @class.WithBaseType($"{template.UseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext")}<{template.GetIdentityUserClass()}>");
        });
    }
}

[IntentManaged(Mode.Ignore)]
public static class IdentityHelperExtensions
{
    public static string GetIdentityUserClass<T>(this CSharpTemplateBase<T> template)
    {
        var identityModel = template.ExecutionContext.MetadataManager
            .Domain(template.ExecutionContext.GetApplicationConfig().Id)
            .GetClassModels()
            .SingleOrDefault(x => x.HasIdentityUser());
        var identityUserClass = identityModel != null
            ? template.GetTypeName("Domain.Entity", identityModel)
            : (template.TryGetTypeName("Domain.IdentityUser", out var userId) ? userId : null);

        return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
    }

    public static string GetIdentityRoleClass<T>(this CSharpTemplateBase<T> template)
    {
        return template.UseType("Microsoft.AspNetCore.Identity.IdentityRole");
    }
}