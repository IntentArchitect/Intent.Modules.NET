using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program;

public interface IBlazorProgramFile
{
    IBlazorProgramFile ConfigureMainStatementsBlock(Action<IHasCSharpStatements> configure);
    IBlazorProgramFile AddMethod(string returnType, string name, Action<IBlazorProgramFileMethod>? configure = null, int priority = 0);
    IBlazorProgramFileMethod? FindMethod(string name) => TryFindMethod(name, out var method) ? method : null;
    bool TryFindMethod(string name, out IBlazorProgramFileMethod method);
    IReadOnlyList<IBlazorProgramFileMethod> Methods { get; }
}