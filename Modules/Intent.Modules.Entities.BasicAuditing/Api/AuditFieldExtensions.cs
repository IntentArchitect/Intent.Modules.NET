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
        // Falls back to a name match against the configured field name for entities audited under a
        // pre-1.0.11 module version, whose attributes were never tagged with a role.
        public static AttributeModel GetAuditField(this ClassModel model, string role, string configuredName = null)
        {
            var taggedMatch = model.Attributes.FirstOrDefault(a =>
                a.InternalElement.Metadata.TryGetValue("basic-auditing-role", out var elementRole) && elementRole == role);
            if (taggedMatch != null)
            {
                return taggedMatch;
            }

            var fallbackName = string.IsNullOrWhiteSpace(configuredName) ? role : configuredName;
            return model.Attributes.FirstOrDefault(a => a.Name == fallbackName);
        }
    }
}
