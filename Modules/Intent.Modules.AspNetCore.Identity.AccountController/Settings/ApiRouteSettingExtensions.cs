using System.Linq;
using Intent.Engine;

namespace Intent.Modules.AspNetCore.Identity.AccountController.Settings
{
    /// <summary>
    /// Reads the "Default API Route Prefix" from the "API Settings" group owned by
    /// Intent.AspNetCore.Controllers. That module is deliberately NOT a dependency of this one —
    /// installing the auth endpoints must not force controllers onto an application that has no
    /// Services — so the group is looked up by id and its absence is handled by a fallback.
    /// </summary>
    internal static class ApiRouteSettingExtensions
    {
        private const string ApiSettingsGroupId = "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb";
        private const string DefaultApiRoutePrefixId = "07c796c5-ff1c-4fb4-b9a4-bc9000439fec";

        /// <summary>
        /// The prefix used when the API Settings group is not present. Matches both the setting's
        /// own default value and the route this module hardcoded prior to 4.2.0.
        /// </summary>
        public const string FallbackPrefix = "api";

        /// <summary>
        /// Resolves the configured API route prefix with no leading or trailing separator, mirroring
        /// the designer-time <c>getDefaultRoutePrefix</c> helper that ordinary controllers go through.
        /// Returns an empty string when the user has deliberately blanked the setting.
        /// </summary>
        public static string GetApiRoutePrefix(this IApplicationSettingsProvider settings)
        {
            var group = settings.GetGroup(ApiSettingsGroupId);
            if (group == null)
            {
                return FallbackPrefix;
            }

            return (group.GetSetting(DefaultApiRoutePrefixId)?.Value ?? string.Empty).Trim().Trim('/');
        }

        /// <summary>
        /// Joins route segments with a single separator, dropping any that are blank. Dropping rather
        /// than joining blindly is what keeps a blanked prefix from producing an empty leading segment.
        /// </summary>
        public static string CombineRoute(params string[] segments)
        {
            return string.Join("/", segments.Where(segment => !string.IsNullOrWhiteSpace(segment)));
        }
    }
}
