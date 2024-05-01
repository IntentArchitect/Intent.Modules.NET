using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazor.Api
{
    public interface IRazorComponentTemplate : IRazorFileTemplate
    {
        IRazorComponentBuilderProvider ComponentBuilderProvider { get; }
        BindingManager BindingManager { get; }
        CSharpClassMappingManager CreateMappingManager();
    }
}