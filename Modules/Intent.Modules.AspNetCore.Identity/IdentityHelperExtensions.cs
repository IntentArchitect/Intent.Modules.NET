using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

namespace Intent.Modules.AspNetCore.Identity;

public static class IdentityHelperExtensions
{
    private static string GetName<T>(this List<ClassModel> classModels, string entityName, CSharpTemplateBase<T> template, bool includeGeneric = true)
    {
        if (classModels.Any(c => c.Name == entityName))
        {
            var @class = classModels.First(c => c.Name == entityName).ChildClasses.First();
            template.GetTypeName("Domain.Entity", @class);

            var entityTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", @class);

            //entityTemplate.CSharpFile.Classes.First().Metadata.TryAdd("primary-keys");

            entityTemplate.AddNugetDependency(NugetPackages.MicrosoftExtensionsIdentityStores(entityTemplate.OutputTarget));
            entityTemplate.CSharpFile.AfterBuild(c =>
            {
                c.AddUsing("Microsoft.AspNetCore.Identity");
            });
            return @class.Name;
        }

        if (includeGeneric)
        {
            return $"{entityName}<string>";
        }
        return $"{entityName}";
    }

    public static bool IdentityClassHasRefreshToken(this IMetadataManager metadataManager, string applicationId)
    {
        var identityModels = metadataManager.Domain(applicationId).GetClassModels()
            .Where(x => x.ParentClass?.Name == "IdentityUser")
            .ToArray();

        if (identityModels.Length == 0)
        {
            identityModels = [.. metadataManager.Domain(applicationId).GetClassModels().Where(x => x.Name == "IdentityUser")];
        }

        if (identityModels.Length == 0)
        {
            return false;
        }

        var model = identityModels.First();

        return model.Attributes.Any(a => a.Name == "RefreshToken");
    }

    public static string GetIdentityUserClass<T>(this CSharpTemplateBase<T> template)
    {
        var associations = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

        var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

        var identityModels = template.ExecutionContext.MetadataManager.GetIdentityClassModels(template.ExecutionContext.GetApplicationConfig().Id);

        if (identityModels.Count > 0)
        {
            return $"{identityModels.GetName("IdentityUser", template, false)}";
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

    public static List<ClassModel> GetIdentityClassModels(this IMetadataManager metadataManager, string applicationId)
    {
        var associations = metadataManager.Domain(applicationId).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

        var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

        return models.Select(p => p.ParentElement.AsClassModel())
            .Where(m => m is not null && (m.Name == "IdentityUserRole" || m.Name == "IdentityRole" ||
                                           m.Name == "IdentityUser" || m.Name == "IdentityRoleClaim" || m.Name == "IdentityUserToken" || m.Name == "IdentityUserClaim" ||
                                           m.Name == "IdentityUserLogin")).ToList();
    }

    public static string GetIdentityDbContextGenericParameters<T>(this CSharpTemplateBase<T> template)
    {
        var identityModels = template.ExecutionContext.MetadataManager.GetIdentityClassModels(template.ExecutionContext.GetApplicationConfig().Id);

        if (identityModels.Count > 0)
        {
            //IdentityDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
            return $"{identityModels.GetName("IdentityUser", template)},{identityModels.GetName("IdentityRole", template)}, string, {identityModels.GetName("IdentityUserClaim", template)}," +
                $"{identityModels.GetName("IdentityUserRole", template)},{identityModels.GetName("IdentityUserLogin", template)},{identityModels.GetName("IdentityRoleClaim", template)}," +
                $"{identityModels.GetName("IdentityUserToken", template)}";
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

    public static bool HasRefreshToken(this ClassModel model)
    {
        return false;
    }

    public static string GetIdentityRoleClass<T>(this CSharpTemplateBase<T> template)
    {
        var identityModels = template.ExecutionContext.MetadataManager.GetIdentityClassModels(template.ExecutionContext.GetApplicationConfig().Id);

        if (identityModels.Count > 0)
        {
            return $"{identityModels.GetName("IdentityRole", template, true)}";
        }
        else
        {
            return template.UseType("Microsoft.AspNetCore.Identity.IdentityRole");
        }
    }
}