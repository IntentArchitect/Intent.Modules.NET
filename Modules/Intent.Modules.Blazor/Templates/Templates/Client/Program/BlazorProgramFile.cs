using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program;

internal class BlazorProgramFile : IBlazorProgramFile
{
    private readonly ProgramTemplate _template;
    private readonly bool _usesTopLevelStatements;

    public BlazorProgramFile(ProgramTemplate template, bool usesTopLevelStatements)
    {
        _template = template;
        _usesTopLevelStatements = usesTopLevelStatements;
    }

    public IBlazorProgramFile ConfigureMainStatementsBlock(Action<IHasCSharpStatements> configure)
    {
        var targetBlock = _usesTopLevelStatements
            ? (IHasCSharpStatements)CSharpFile.TopLevelStatements
            : CSharpFile.Classes.First().Methods.First(x => x.Name == "Main");

        configure(targetBlock);

        return this;
    }

    public IBlazorProgramFile AddMethod(string returnType, string name, Action<IBlazorProgramFileMethod>? configure = null, int priority = 0)
    {
        if (_usesTopLevelStatements)
        {
            CSharpFile.TopLevelStatements.AddLocalMethod(returnType, name, method =>
            {
                configure?.Invoke(Create(method));
            });
        }
        else
        {
            CSharpFile.Classes.First().AddMethod(returnType: returnType, name: name, method =>
            {
                configure?.Invoke(Create(method));
            });
        }

        return this;
    }

    public bool TryFindMethod(string name, out IBlazorProgramFileMethod method)
    {
        method = Methods.FirstOrDefault(x => x.Name == name)!;
        return method != null;
    }

    public IReadOnlyList<IBlazorProgramFileMethod> Methods => _usesTopLevelStatements
        ? CSharpFile.Classes.First().Methods.Select(Create).ToArray()
        : CSharpFile.TopLevelStatements.Statements.OfType<CSharpLocalMethod>().Select(Create).ToArray();

    private CSharpFile CSharpFile => _template.CSharpFile;

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private static IBlazorProgramFileMethod Create<TCSharpMethod>(ICSharpMethod<TCSharpMethod> method)
        where TCSharpMethod : ICSharpMethod<TCSharpMethod>
    {
        return new CSharpMethodWrapper<TCSharpMethod>(method);
    }
#pragma warning restore CA1859
}
