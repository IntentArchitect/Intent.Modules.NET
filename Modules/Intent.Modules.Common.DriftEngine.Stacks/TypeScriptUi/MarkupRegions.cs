using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

/// <summary>
/// <c>region.outlet</c> for a component whose markup writes its placement points: a Region of a Layout, Page, Dialog or
/// Component is the placement point of its name in the component's markup (Angular's <c>&lt;ng-content select&gt;</c> and named
/// <c>&lt;router-outlet&gt;</c>, Vue's named <c>&lt;slot&gt;</c> and <c>&lt;RouterView&gt;</c>), and the one the markup doesn't
/// name (a bare <c>&lt;ng-content&gt;</c>, <c>&lt;router-outlet&gt;</c> or <c>&lt;slot&gt;</c>) is the Region no placement point
/// names, when only one is, else the only one of those placing a Page, as a Blazor layout's <c>@Body</c> is. Names are compared
/// as words (<c>CardTitle</c> is <c>card-title</c>). A framework's reader says which components are its own and what their
/// placement points are.
/// </summary>
internal abstract class MarkupRegions : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["region.outlet"];

    /// <summary>A placement point: its name, null where the markup names none, or <see cref="UiCode.Hole"/> where it is computed.</summary>
    protected sealed record Outlet(string? Name, Span At);

    /// <summary>The component is this reader's framework's.</summary>
    protected abstract bool Answers(TypeNode type);

    /// <summary>The component's placement points, in the order its markup writes them.</summary>
    protected abstract IReadOnlyList<Outlet> Outlets(CodeGraph code, TypeNode type);

    /// <summary>How the framework writes a placement point of a name, for the message: <c>&lt;slot name="{0}"&gt;</c>.</summary>
    protected abstract string Written { get; }

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element.ParentElement is not { } parent || context.TypeOf(parent) is not { } type || !Answers(type))
        {
            return null;
        }

        var name = context.Element.Name;
        var outlets = Outlets(context.Code, type);
        bool Names(Outlet outlet, string region) => outlet.Name is { } named && named != UiCode.Hole.ToString() && UiCode.Words(named) == UiCode.Words(region);
        if (outlets.FirstOrDefault(x => Names(x, name)) is { } outlet)
        {
            return ClaimReading.Stated(name, outlet.At);
        }

        // The parent's Regions, by the region.outlet claims on its children, whatever its designer calls them.
        var regions = parent.ChildElements.Where(x => x == context.Element || context.ClaimOn(x, "region.outlet") is not null).ToList();
        var unnamed = regions.Where(x => !outlets.Any(o => Names(o, x.Name))).ToList();
        var spare = outlets.Where(x => x.Name is not null && regions.All(r => !Names(x, r.Name))).ToList();
        if (outlets.FirstOrDefault(x => x.Name is null) is { } bare)
        {
            var body = Unnamed(context, unnamed);
            if (body == context.Element)
            {
                return ClaimReading.Stated(name, bare.At);
            }

            // Several Regions name no placement point, as when the code renames or removes a named one beside the bare one:
            // which of them the bare one is, nothing says.
            if (body is null && unnamed.Count > 1)
            {
                var others = spare.Count == 0 ? "" : $" and {string.Join(", ", spare.Select(x => $"`{x.Name}`").Distinct())}, which no Region names,";
                return ClaimReading.CantVerify("ambiguous",
                    $"`{parent.Name}` has a bare placement point{others} and its Regions {string.Join(", ", unnamed.Select(x => $"`{x.Name}`"))} name none of its placement points, so which of them is the bare one can't be told. Rename the Region or placement point that changed.",
                    bare.At, spare.Select(x => x.At).Prepend(bare.At).ToList());
            }
        }

        if (outlets.FirstOrDefault(x => x.Name == UiCode.Hole.ToString()) is { } computed)
        {
            return ClaimReading.CantVerify("unreadable", "A placement point's name is computed.", computed.At);
        }

        // A placement point no Region names is the one this Region was renamed from, when it is the only one.
        var actual = spare.Count == 1 ? spare[0].Name! : "none";
        var has = outlets.Count == 0
            ? "has no placement point"
            : $"has the placement points {string.Join(", ", outlets.Select(x => x.Name is null ? "a bare one" : $"`{x.Name}`").Distinct())}";
        return ClaimReading.Compared(actual, spare.Count == 1 ? spare[0].At : type.At, false,
            $"`{parent.Name}` region `{name}`: model `{name}`, code {(spare.Count == 1 ? $"`{actual}`" : "none")}. `{parent.Name}` {has}, and none named `{name}` ({string.Format(Written, UiCode.Kebab(name))}). The model states it in the Region's name. Rename the Region, or the placement point.");
    }

    /// <summary>The Region the bare placement point is: of the Regions none names, the only one, else the only one placing a Page.</summary>
    private static IElement? Unnamed(ClaimContext context, IReadOnlyList<IElement> unnamed) =>
        unnamed.Count == 1 ? unnamed[0]
        : unnamed.Where(x => context.Model.Links(x, "Composition").Any(l => l.Target.SpecializationType == "Page")).ToList() is [var only] ? only : null;
}
