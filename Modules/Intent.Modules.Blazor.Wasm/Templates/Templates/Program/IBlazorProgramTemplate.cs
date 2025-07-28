using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Wasm.Templates.Templates.Program;

public interface IBlazorProgramTemplate : ICSharpFileBuilderTemplate
{
    IBlazorProgramFile ProgramFile { get; }
}