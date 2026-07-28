using System.Linq;
using Intent.Modelers.Domain.Api;

namespace Intent.Entities.BasicAuditing.Api
{
    internal static class AuditRoles
    {
        public const string CreatedBy = "CreatedBy";
        public const string CreatedDate = "CreatedDate";
        public const string UpdatedBy = "UpdatedBy";
        public const string UpdatedDate = "UpdatedDate";
    }

    public static class AuditFieldExtensions
    {
        // Resolves the attribute Basic Auditing assigned a role to, regardless of its current name -
        // a class may have been synced under a since-renamed setting, so the name alone can't be trusted.
        public static AttributeModel GetAuditField(this ClassModel model, string role)
        {
            return model.Attributes.FirstOrDefault(a =>
                a.InternalElement.Metadata.TryGetValue("basic-auditing-role", out var elementRole) && elementRole == role);
        }
    }
}
