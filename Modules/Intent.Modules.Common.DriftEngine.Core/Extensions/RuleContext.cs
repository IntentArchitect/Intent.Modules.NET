using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>What a rule sees: the designers' elements, the claims and their outcomes, the in-scope code and the scope.</summary>
public sealed class RuleContext
{
    private readonly DriftEngine _engine;

    internal RuleContext(DriftEngine engine, CodeGraph code, CodeScope scope, IReadOnlyList<Claim> claims)
    {
        _engine = engine;
        Code = code;
        Scope = scope;
        Claims = claims;
    }

    public CodeGraph Code { get; }

    public CodeScope Scope { get; }

    /// <summary>Every claim of the run, evaluated.</summary>
    public IReadOnlyList<Claim> Claims { get; }

    /// <summary>A designer's elements at any depth, by the name packs give it; none when the application hasn't the designer.</summary>
    public IReadOnlyList<IElement> Elements(string designer) => _engine.Elements(designer);

    /// <summary>A claim of a designer on an element.</summary>
    public Claim? ClaimOn(string designer, IElement element, string kind) => _engine.ClaimOn(designer, element, kind);

    /// <summary>The type an element's <c>type.exists</c> claim located, or null when none did.</summary>
    public TypeNode? TypeOf(string designer, IElement element) => _engine.TypeOf(designer, element);

    /// <summary>The member an element's <c>member.exists</c> claim located, or null when none did.</summary>
    public MemberNode? MemberOf(string designer, IElement element) => _engine.MemberOf(designer, element);

    /// <summary>What an element's existence claim of <paramref name="kind"/> located; null when nothing did.</summary>
    public object? LocatedOf(string designer, IElement element, string kind) => _engine.Locate(designer, element, kind);
}

/// <summary>Which files the Drift Scope reads. Paths are relative to the output folder, with <c>/</c>.</summary>
public sealed class CodeScope
{
    private readonly IReadOnlyDictionary<string, bool> _files;

    internal CodeScope(IEnumerable<(string Path, bool In)> files)
    {
        _files = files.ToDictionary(x => x.Path, x => x.In, StringComparer.Ordinal);
        Files = _files.Keys.OrderBy(x => x, StringComparer.Ordinal).ToList();
    }

    /// <summary>Every file under the output folder, in or out of scope, in path order.</summary>
    public IReadOnlyList<string> Files { get; }

    /// <summary>The file is in scope, so it is read.</summary>
    public bool Includes(string path) => _files.GetValueOrDefault(path);
}
