using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api
{
    public interface IRazorFileTemplate : ICSharpTemplate
    {
        RazorFile RazorFile { get; }
    }
}