using Intent.Blazor.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.Dialog;
using Intent.Modules.Blazor.Templates.Templates.Client.DialogCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.Page;
using Intent.Modules.Blazor.Templates.Templates.Client.PageCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponent;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerDialog;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerDialogCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerPage;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerPageCodeBehind;

namespace Intent.Modules.Blazor.Api;

/// <summary>
/// Resolves the Razor/CodeBehind template that actually generates a given <see cref="ComponentModel"/>,
/// now that plain Component, Page and Dialog each generate through their own template
/// (<see cref="RazorComponentTemplate"/>/<see cref="PageTemplate"/>/<see cref="DialogTemplate"/> and their
/// Server-rendered/CodeBehind equivalents), so consumers no longer need to hardcode a single fixed pair
/// of ids.
/// </summary>
public static class ComponentTemplateIds
{
    public static readonly string[] AllClientRazorTemplateIds =
    [
        RazorComponentTemplate.TemplateId,
        PageTemplate.TemplateId,
        DialogTemplate.TemplateId
    ];

    public static readonly string[] AllClientCodeBehindTemplateIds =
    [
        RazorComponentCodeBehindTemplate.TemplateId,
        PageCodeBehindTemplate.TemplateId,
        DialogCodeBehindTemplate.TemplateId
    ];

    public static readonly string[] AllServerRazorTemplateIds =
    [
        RazorServerComponentTemplate.TemplateId,
        RazorServerPageTemplate.TemplateId,
        RazorServerDialogTemplate.TemplateId
    ];

    public static readonly string[] AllServerCodeBehindTemplateIds =
    [
        RazorServerComponentCodeBehindTemplate.TemplateId,
        RazorServerPageCodeBehindTemplate.TemplateId,
        RazorServerDialogCodeBehindTemplate.TemplateId
    ];

    /// <summary>
    /// The id of the Razor template that generates <paramref name="model"/>'s <c>.razor</c> file,
    /// honoring its Render-On-Server/Page/Dialog stereotypes.
    /// </summary>
    public static string GetRazorTemplateId(this ComponentModel model)
    {
        if (model.HasRenderOnServer())
        {
            if (model.HasPage()) return RazorServerPageTemplate.TemplateId;
            if (model.HasDialog()) return RazorServerDialogTemplate.TemplateId;
            return RazorServerComponentTemplate.TemplateId;
        }

        if (model.HasPage()) return PageTemplate.TemplateId;
        if (model.HasDialog()) return DialogTemplate.TemplateId;
        return RazorComponentTemplate.TemplateId;
    }

    /// <summary>
    /// The id of the C# template that generates <paramref name="model"/>'s <c>.razor.cs</c> code-behind
    /// file, honoring its Render-On-Server/Page/Dialog stereotypes.
    /// </summary>
    public static string GetCodeBehindTemplateId(this ComponentModel model)
    {
        if (model.HasRenderOnServer())
        {
            if (model.HasPage()) return RazorServerPageCodeBehindTemplate.TemplateId;
            if (model.HasDialog()) return RazorServerDialogCodeBehindTemplate.TemplateId;
            return RazorServerComponentCodeBehindTemplate.TemplateId;
        }

        if (model.HasPage()) return PageCodeBehindTemplate.TemplateId;
        if (model.HasDialog()) return DialogCodeBehindTemplate.TemplateId;
        return RazorComponentCodeBehindTemplate.TemplateId;
    }
}
