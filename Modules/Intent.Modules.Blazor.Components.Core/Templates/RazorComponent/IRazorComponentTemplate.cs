using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    public interface IRazorComponentTemplate : ICSharpTemplate
    {
        IRazorComponentBuilderProvider ComponentBuilderResolver { get; }
        BindingManager BindingManager { get; }
        // TODO: Make interface:
        RazorFile BlazorFile { get; }
        void AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null);
        CSharpClassMappingManager CreateMappingManager();
    }
}