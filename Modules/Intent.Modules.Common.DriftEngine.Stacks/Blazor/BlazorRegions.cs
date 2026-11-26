using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.Blazor;

/// <summary>
/// <c>region.outlet</c>: a Region of a Razor component (a Layout's, Page's, Dialog's or Component's named placement point)
/// is a <c>&lt;SectionOutlet SectionName="..."/&gt;</c> of its name in the component's markup, which pages fill with
/// <c>&lt;SectionContent&gt;</c>. A layout's <c>@Body</c> is its one placement point the code doesn't name, so it is the
/// layout's Region no outlet names, when only one is (or only one of those places a Page, as the designer places pages in a
/// layout). It answers only for a Razor component, so another framework's reader keeps the claim.
/// </summary>
internal sealed class Regions : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["region.outlet"];

    public ClaimReading? Read(ClaimContext context)
    {
        if (context.Element.ParentElement is not { } parent || context.TypeOf(parent) is not { } type || !BlazorExtension.IsRazor(type))
        {
            return null;
        }

        var name = context.Element.Name;
        var outlets = BlazorMarkup.SectionOutlets(context.Code, type);
        if (outlets.FirstOrDefault(x => x.Name == name) is { Node: { } outlet })
        {
            return ClaimReading.Stated(name, outlet.At);
        }

        // A RenderFragment parameter is a named placement point a parent fills, as a section outlet is one a page fills.
        var fragments = BlazorMarkup.Fragments(context.Code, type);
        if (fragments.FirstOrDefault(x => x.Name == name) is { } fragment)
        {
            return ClaimReading.Stated(name, fragment.At);
        }

        var named = outlets.Select(x => (Name: x.Name, x.Node.At)).Concat(fragments.Select(x => (Name: (string?)x.Name, x.At))).ToList();

        // The parent's Regions, by the region.outlet claims on its children, whatever its designer calls them.
        var regions = parent.ChildElements.Where(x => x == context.Element || context.ClaimOn(x, "region.outlet") is not null).ToList();
        var unnamed = regions.Where(x => named.All(o => o.Name != x.Name)).ToList();
        if (BlazorMarkup.Body(context.Code, type) is { } body && Body(context, unnamed) == context.Element)
        {
            return ClaimReading.Stated(name, body);
        }

        if (outlets.Any(x => x.Name is null))
        {
            return ClaimReading.CantVerify("unreadable", "A <SectionOutlet>'s SectionName is computed.", outlets.First(x => x.Name is null).Node.At);
        }

        // A placement point no Region names is the one this Region was renamed from, when it is the only one.
        var spare = named.Where(x => regions.All(r => r.Name != x.Name)).ToList();
        var actual = spare.Count == 1 ? spare[0].Name! : "none";
        var has = (outlets.Count, fragments.Count) switch
        {
            (0, 0) => "has no `<SectionOutlet>` or `RenderFragment` parameter",
            (_, 0) => $"has the outlets {string.Join(", ", outlets.Select(x => $"`{x.Name}`"))}",
            (0, _) => $"has the `RenderFragment` parameters {string.Join(", ", fragments.Select(x => $"`{x.Name}`"))}",
            _ => $"has the outlets {string.Join(", ", outlets.Select(x => $"`{x.Name}`"))} and the `RenderFragment` parameters {string.Join(", ", fragments.Select(x => $"`{x.Name}`"))}"
        };
        return ClaimReading.Compared(actual, spare.Count == 1 ? spare[0].At : type.At, false,
            $"`{parent.Name}` region `{name}`: model `{name}`, code {(spare.Count == 1 ? $"`{actual}`" : "none")}. `{parent.Name}` {has}, and no `<SectionOutlet SectionName=\"{name}\"/>` or `[Parameter] RenderFragment {name}`. The model states it in the Region's name. Rename the Region, or the placement point.");
    }

    /// <summary>
    /// The Region a layout's <c>@Body</c> is: of the Regions no outlet names, the only one, else the only one placing a Page.
    /// </summary>
    private static IElement? Body(ClaimContext context, IReadOnlyList<IElement> unnamed) =>
        unnamed.Count == 1 ? unnamed[0]
        : unnamed.Where(x => context.Model.Links(x, "Composition").Any(l => l.Target.SpecializationType == "Page")).ToList() is [var only] ? only : null;
}

/// <summary>
/// <c>unmodelled.region</c>: a named placement point of a component the model describes that no Region of its element names
/// (no <c>region.outlet</c> claim on one of its children), the code-to-model side of <see cref="Regions"/>: a Razor component's
/// <c>&lt;SectionOutlet SectionName="..."/&gt;</c> and <c>[Parameter] RenderFragment</c>s, an MVC view's <c>RenderSection("...")</c>
/// (<see cref="Mvc.ViewRegions"/>) and a .NET MAUI Shell's named items (<see cref="Maui.ShellRegions"/>), whose outlets join this
/// rule rather than registering a second one under its id. A component no element describes is <c>unmodelled.type</c>'s, and a
/// layout's <c>@Body</c> or <c>RenderBody()</c>, which the code doesn't name, is never asked for. Nor is anything of a
/// component with only one placement point (its body counted): the designer adds Regions "only where an element has more than
/// one placement point".
/// </summary>
internal sealed class UnmodelledRegions : IOptInRule
{
    public string Id => "unmodelled.region";

    public IEnumerable<UnmodelledCode> Find(RuleContext context)
    {
        var mvc = Mvc.MvcSite.Of(context.Code);
        var outlets = context.Code.Types.Where(BlazorExtension.IsRazor)
            .Select(x => (Type: x, Outlets: BlazorMarkup.SectionOutlets(context.Code, x).Select(o => (o.Name, o.Node.At, Written: $"`<SectionOutlet SectionName=\"{o.Name}\"/>`"))
                .Concat(BlazorMarkup.Fragments(context.Code, x).Select(f => ((string?)f.Name, f.At, Written: $"`[Parameter] RenderFragment {f.Name}`"))).ToList(),
                Body: BlazorMarkup.Body(context.Code, x) is not null))
            .Concat(mvc?.Views.Values.Select(x => (x.Type, Outlets: x.Sections.Select(s => (s.Name, s.At, Written: $"`RenderSection(\"{s.Name}\")`")).ToList(), Body: x.Body is not null)) ?? [])
            .Concat(Maui.MauiSite.Of(context.Code).ShellItems.GroupBy(x => x.Shell).Select(x => (Type: x.Key, Outlets: x.Select(i => (i.Name, i.At,
                Written: i.Route is not null ? $"The Shell item `<{i.Tag} Route=\"{i.Route}\">`" : $"The Shell item `<{i.Tag}>` showing `<ShellContent Route=\"{i.Name}\">`")).ToList(), Body: false)));
        foreach (var (type, named, body) in outlets.Where(x => context.Scope.Checks(x.Type.At.File) && x.Outlets.Count + (x.Body ? 1 : 0) > 1))
        {
            if (context.ElementOf(type) is not { } located)
            {
                continue;
            }

            var (designer, element) = located;
            var regions = element.ChildElements.Select(x => context.ClaimOn(designer, x, "region.outlet")).OfType<Claim>().Select(x => x.Expected).ToHashSet(StringComparer.Ordinal);
            foreach (var (name, at, written) in named)
            {
                if (name is not null && !regions.Contains(name))
                {
                    yield return new UnmodelledCode(designer, at, $"{type.Name}.{name}",
                        $"{written} is a named placement point of `{type.Name}`, which the {designer} {element.SpecializationType} `{element.Name}` describes, but no Region of `{element.Name}` names it. Add the Region `{name}` to `{element.Name}`, or add an exception with a reason to the Root Folder's Drift Check.",
                        element.Id, type.Name);
                }
            }
        }
    }
}
