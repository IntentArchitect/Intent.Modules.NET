#nullable enable
using Intent.Configuration;
using Intent.Engine;

namespace Intent.Modules.Application.Identity.Settings
{
    /// <summary>
    /// Local shim for the "ASP.NET Core Settings" group, which is OWNED by the
    /// <c>Intent.AspNetCore</c> module. This module contributes its "Enable Authentication" setting into
    /// that group via a settings group extension (see the <c>moduleSettingsExtensions</c> element of the
    /// imodspec), so that the authentication and authorization toggles appear together for the user.
    /// <para>
    /// Deliberately declared here rather than taken as a dependency on <c>Intent.AspNetCore</c> — the
    /// coupling is by group id only. <see cref="GetASPNETCoreSettings"/> therefore returns <c>null</c> when
    /// that module is not installed, and every call site must treat absence as "not configured".
    /// </para>
    /// </summary>
    public class ASPNETCoreSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ASPNETCoreSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }
    }

    internal static class ASPNETCoreSettingsProvider
    {
        private const string ASPNETCoreSettingsGroupId = "9677c813-b334-4b0d-be2c-3b149f1f6ae8";
        private const string EnableAuthenticationSettingId = "25718607-fca5-47ae-adce-3231e0c6e755";

        /// <summary>
        /// Whether <c>UseAuthentication</c> should be registered on the application builder. Defaults to
        /// <c>true</c> in both of the cases where no explicit choice exists: the setting has never been
        /// persisted (an application upgrading from a module version predating it), or the
        /// <c>Intent.AspNetCore</c> module which owns the group is not installed at all. Only an explicit
        /// <c>false</c> disables it.
        /// <para>
        /// Deliberately not the generated <c>EnableAuthentication()</c> accessor, which is of the form
        /// <c>bool.TryParse(...) &amp;&amp; result</c> and so cannot distinguish "unset" from "false" — it would
        /// silently drop a statement the user never opted out of.
        /// </para>
        /// </summary>
        public static bool EnableAuthenticationOrDefault(this ASPNETCoreSettings? settings) =>
            !bool.TryParse(settings?.GetSetting(EnableAuthenticationSettingId)?.Value, out var result) || result;

        /// <summary>
        /// Returns the ASP.NET Core settings group, or <c>null</c> when the <c>Intent.AspNetCore</c> module
        /// which owns it is not installed.
        /// </summary>
        public static ASPNETCoreSettings? GetASPNETCoreSettings(this IApplicationSettingsProvider settings)
        {
            var group = settings.GetGroup(ASPNETCoreSettingsGroupId);

            return group != null ? new ASPNETCoreSettings(group) : null;
        }
    }
}
