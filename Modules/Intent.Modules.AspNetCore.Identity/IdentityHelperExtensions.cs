using System;
using System.Linq;
using System.Text;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Utils;

namespace Intent.Modules.AspNetCore.Identity;

public static class IdentityHelperExtensions
{
    public static string GetIdentityUserClass<T>(this CSharpTemplateBase<T> template)
    {
        var identityModel = template.ExecutionContext.MetadataManager.GetIdentityUserClass(template.ExecutionContext.GetApplicationConfig().Id);
        var identityUserClass = identityModel != null
            ? template.GetTypeName("Domain.Entity", identityModel)
            : template.TryGetTypeName("Domain.IdentityUser");

        return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
    }

    internal static ClassModel GetIdentityUserClass(this IMetadataManager metadataManager, string applicationId)
    {
        var identityModels = metadataManager.Domain(applicationId).GetClassModels()
            .Where(x => x.HasIdentityUser())
            .ToArray();
        if (identityModels.Length > 1)
        {
            var sb = new StringBuilder("More than one class has the \"Identity User\" stereotype applied to it:");
            foreach (var model in identityModels)
            {
                sb.Append($"{Environment.NewLine}- \"{model.Name}\" [{model.Id}]");
            }

            Logging.Log.Failure(sb.ToString());
            return null;
        }

        return identityModels.SingleOrDefault();
    }

    public static string GetIdentityRoleClass<T>(this CSharpTemplateBase<T> template)
    {
        return template.UseType("Microsoft.AspNetCore.Identity.IdentityRole");
    }
}