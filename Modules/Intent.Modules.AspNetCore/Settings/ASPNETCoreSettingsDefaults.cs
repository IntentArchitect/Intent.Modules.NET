#nullable enable
using Intent.Configuration;

namespace Intent.Modules.AspNetCore.Settings
{
    /// <summary>
    /// Readers for switch settings which must default to <c>true</c> when they have never been persisted.
    /// <para>
    /// The generated accessors on <see cref="ASPNETCoreSettings"/> are of the form
    /// <c>bool.TryParse(GetSetting(id)?.Value.ToPascalCase(), out var result) &amp;&amp; result</c>, which returns
    /// <c>false</c> for an <em>absent</em> setting — indistinguishable from an explicit "false". For a setting
    /// that gates code which was previously always generated, that is a silent regression: an application
    /// upgrading from a module version predating the setting would lose the generated statement without ever
    /// having opted out. These readers treat absence as "enabled", so only an explicit <c>false</c> disables.
    /// </para>
    /// </summary>
    internal static class ASPNETCoreSettingsDefaults
    {
        private const string EnableAuthorizationSettingId = "d96fffca-b2da-4b71-9c8c-93fd0d00520b";

        /// <summary>
        /// Whether <c>UseAuthorization</c> should be registered on the application builder. Defaults to
        /// <c>true</c> when the setting has not been persisted, or when <paramref name="settings"/> is
        /// <c>null</c>.
        /// </summary>
        public static bool EnableAuthorizationOrDefault(this ASPNETCoreSettings? settings) =>
            settings.IsEnabledUnlessExplicitlyFalse(EnableAuthorizationSettingId);

        private static bool IsEnabledUnlessExplicitlyFalse(this IGroupSettings? settings, string settingId) =>
            !bool.TryParse(settings?.GetSetting(settingId)?.Value, out var result) || result;
    }
}
