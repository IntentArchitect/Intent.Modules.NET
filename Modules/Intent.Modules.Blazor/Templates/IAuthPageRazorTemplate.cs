using Intent.Modelers.UI.Api;

namespace Intent.Modules.Blazor.Templates;

/// <summary>
/// Exposes what an authentication-page default-content factory extension needs generically from
/// either a Client (<c>RazorComponentTemplate</c>) or Server (<c>RazorServerComponentTemplate</c>)
/// Razor page template.
/// </summary>
public interface IAuthPageRazorTemplate
{
    ComponentModel Model { get; }

    string? DefaultContentOverride { get; set; }
}
