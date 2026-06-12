using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Blazor.Api;

public enum BlazorThemeStorageMode
{
    BrowserStorage,
    Cookie
}

public static class BlazorThemeCapabilities
{
    public const string CookieThemeStorageValue = "cookie";

    private const string AuthenticationModuleId = "Intent.Blazor.Authentication";

    public static BlazorThemeStorageMode GetThemeStorageMode(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.InstalledModules.Any(module => module.ModuleId == AuthenticationModuleId)
            ? BlazorThemeStorageMode.Cookie
            : BlazorThemeStorageMode.BrowserStorage;
    }

    public static bool RequiresSsrSafeThemeToggle(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.GetThemeStorageMode() == BlazorThemeStorageMode.Cookie;
    }
}
