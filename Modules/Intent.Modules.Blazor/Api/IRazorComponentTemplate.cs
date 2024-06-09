using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Razor;

namespace Intent.Modules.Blazor.Api;

public interface IRazorComponentTemplate : IRazorFileTemplate
{
    IRazorComponentBuilderProvider ComponentBuilderProvider { get; }
    BindingManager BindingManager { get; }
    CSharpClassMappingManager CreateMappingManager();
    IBuildsCSharpMembers GetCodeBlock();
}