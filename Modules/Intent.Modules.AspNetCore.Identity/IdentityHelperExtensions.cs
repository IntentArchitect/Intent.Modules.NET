using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.AspNetCore.Identity;

public static class IdentityHelperExtensions
{
    public static string GetIdentityUserClass<T>(this CSharpTemplateBase<T> template)
    {
        var identityModel = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels()
            .SingleOrDefault(x => x.HasIdentityUser());
        var identityUserClass = identityModel != null
            ? template.GetTypeName("Domain.Entity", identityModel)
            : template.TryGetTypeName("Domain.IdentityUser");

        return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
    }

    public static string GetIdentityRoleClass<T>(this CSharpTemplateBase<T> template)
    {
        return template.UseType("Microsoft.AspNetCore.Identity.IdentityRole");
    }
}