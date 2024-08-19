using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program;

public interface IBlazorProgramTemplate : ICSharpFileBuilderTemplate
{
    IBlazorProgramFile ProgramFile { get; }
}