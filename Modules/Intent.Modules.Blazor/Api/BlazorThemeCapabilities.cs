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
        // ASP.NET Core Identity account pages must use static SSR so sign-in/sign-out can issue auth cookies.
        // Cookie-backed theme storage lets those SSR pages render the selected theme before Blazor is interactive.
        return executionContext.InstalledModules.Any(module => module.ModuleId == AuthenticationModuleId)
            ? BlazorThemeStorageMode.Cookie
            : BlazorThemeStorageMode.BrowserStorage;
    }

    public static bool RequiresSsrSafeThemeToggle(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.GetThemeStorageMode() == BlazorThemeStorageMode.Cookie;
    }
}
