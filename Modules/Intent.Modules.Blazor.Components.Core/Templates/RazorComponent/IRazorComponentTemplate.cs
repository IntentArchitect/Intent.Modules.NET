using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    public interface IRazorComponentTemplate
    {
        IRazorComponentBuilderResolver ComponentBuilderResolver { get; }
        // TODO: Make interface:
        RazorFile BlazorFile { get; }
    }
}