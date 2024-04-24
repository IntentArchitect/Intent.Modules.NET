using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    public interface IRazorComponentTemplate : IRazorFileTemplate
    {
        IRazorComponentBuilderProvider ComponentBuilderProvider { get; }
        BindingManager BindingManager { get; }
        // TODO: Make interface:
        void AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null);
        CSharpClassMappingManager CreateMappingManager();
    }
}