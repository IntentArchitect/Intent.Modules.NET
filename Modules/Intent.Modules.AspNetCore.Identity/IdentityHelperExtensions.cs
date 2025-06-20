using System;
using System.Collections.Generic;
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
    private static string GetName(this List<ClassModel> classModels, string entityName)
    {
        if(classModels.Any(c => c.Name == entityName))
        {
            return classModels.First(c => c.Name == entityName).ChildClasses.First().Name;
        }

        return $"{entityName}<string>";
    }

    public static string GetIdentityUserClass<T>(this CSharpTemplateBase<T> template)
    {
        var associations = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

        var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

        var identityModels = models.Select(p => p.ParentElement.AsClassModel()).Where(m => m.Name == "IdentityUserRole" || m.Name == "IdentityRole" ||
            m.Name == "IdentityUser" || m.Name == "IdentityRoleClaim" || m.Name == "IdentityUserToken" || m.Name == "IdentityUserClaim" ||
            m.Name == "IdentityUserLogin").ToList();

        if (identityModels.Count > 0)
        {
            //IdentityDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
            return $"{identityModels.GetName("IdentityUser")},{identityModels.GetName("IdentityRole")}, string, {identityModels.GetName("IdentityUserClaim")}," +
                $"{identityModels.GetName("IdentityUserRole")},{identityModels.GetName("IdentityUserLogin")},{identityModels.GetName("IdentityRoleClaim")}," +
                $"{identityModels.GetName("IdentityUserToken")}";
        }
        else
        {
            var identityModel = template.ExecutionContext.MetadataManager.GetIdentityUserClass(template.ExecutionContext.GetApplicationConfig().Id);
            var identityUserClass = identityModel != null
                ? template.GetTypeName("Domain.Entity", identityModel)
                : template.TryGetTypeName("Domain.IdentityUser");

            return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
        }
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