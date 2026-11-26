using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Interactions;
using Intent.Modules.Common.DriftEngine.Stacks.Markup;
using Intent.Modules.Common.DriftEngine.Stacks.Messaging;
using Intent.Modules.Common.DriftEngine.Stacks.Proxies;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;
using Intent.Modules.Common.DriftEngine.Stacks.Ui;

namespace Intent.Modules.Common.DriftEngine.Stacks.Blazor;

/// <summary>
/// Reads the User Interface designer's claims (the <c>intent.ui</c> pack's) from Blazor's Razor components: a page's
/// <c>@page</c> routes and <c>&lt;PageTitle&gt;</c>, whether a component is a page or a layout, its <c>[Parameter]</c>
/// properties and <c>EventCallback</c>s, its section outlets (Regions), and what its markup and code render, navigate to,
/// open, inject, post forms to and load scripts for. It answers only for a component its <c>.razor</c> file declares, so a
/// Vue front end's claims are left to its house pack, and for what a plain C# class uses and the proxy operations it calls
/// (a Store's, a View Model's), which a Blazor application's state and services are.
/// </summary>
public sealed class BlazorExtension : IDriftExtension
{
    public string Id => "intent.blazor";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Components(), new Members(), new Links(), new Regions()];

    public IEnumerable<ICodeReader> Readers { get; } = [new ComponentMembers()];

    public IEnumerable<IOptInRule> OptInRules { get; } = [new UnmodelledRegions()];

    /// <summary>
    /// What a component's properties are to the designer, so <c>unmodelled.member</c> asks only for the ones it can state:
    /// an injected service (<c>@inject</c>, <c>[Inject]</c>) is its <c>Uses</c> of what it injects (<see cref="CodeGraph.StatedByLink"/>),
    /// and a property that is no parameter, such as one bound from the posted form (<c>[SupplyParameterFromForm]</c>), is
    /// written in code (<see cref="CodeGraph.Unmodellable"/>): the designer states a component's Properties only for its
    /// parameters, and a Page's only for its route and query parameters.
    /// </summary>
    private sealed class ComponentMembers : ICodeReader
    {
        public void Read(CodeGraph graph)
        {
            // A RenderFragment parameter is a placement point a parent fills: a Region states it, never a Property.
            graph.Unstatable.Add((_, type, member) => IsRazor(type) && BlazorMarkup.IsFragment(member)
                ? "it is a placement point a parent fills with content, which a Region states"
                : null);

            foreach (var member in graph.Types.Where(IsRazor).SelectMany(x => x.Members).Where(x => x.Kind is "property" or "field"))
            {
                if (member.Attributes.Any(a => a.Name == "Inject"))
                {
                    graph.StatedByLink[member] = "Uses";
                }
                else if (!IsParameter(member))
                {
                    graph.Unmodellable[member] = IsFormBound(member) ? "Blazor binds it from the posted form" : "it is no parameter, so a parent can't set it";
                }
            }
        }
    }

    /// <summary>A component a <c>.razor</c> file declares, with its code-behind.</summary>
    internal static bool IsRazor(TypeNode? type) => type is { Kind: "component" } && type.Files.Any(x => x.EndsWith(".razor", StringComparison.OrdinalIgnoreCase));

    /// <summary>A component's <c>@page</c> routes, each the <c>[Route]</c> it compiles to.</summary>
    internal static IReadOnlyList<AttributeNode> Routes(TypeNode type) => type.Attributes.Where(x => x.Name == "Route" && x.Arguments.Count > 0).ToList();

    /// <summary>A property a parent sets: <c>[Parameter]</c>, <c>[SupplyParameterFromQuery]</c> or <c>[CascadingParameter]</c>.</summary>
    internal static bool IsParameter(MemberNode member) => member.Attributes.Any(x => x.Name is "Parameter" or "SupplyParameterFromQuery" or "CascadingParameter");

    /// <summary>A property Blazor binds from the posted form: <c>[SupplyParameterFromForm]</c>.</summary>
    internal static bool IsFormBound(MemberNode member) => member.Attributes.Any(x => x.Name == "SupplyParameterFromForm");

    internal static bool IsCallback(TypeRefNode? type) => type?.Name is "EventCallback";

    /// <summary>
    /// The element whose code a link starts in, and the code: the element itself, or the nearest element holding it that a
    /// <c>type.exists</c> claim located (a component operation's, a region's or a Display Component's component), whatever
    /// the designer calls them.
    /// </summary>
    internal static (IElement Element, TypeNode Type)? CodeOf(ClaimContext context, IElement? element)
    {
        for (var current = element; current is not null; current = current.ParentElement)
        {
            if (context.TypeOf(current) is { } type)
            {
                return (current, type);
            }
        }

        return null;
    }

    /// <summary>A binding the claim's vocabulary gives.</summary>
    internal static bool Binds(ClaimContext context, string name) => context.Claim.Bindings?.ContainsKey(name) == true;
}

/// <summary>What a component is, a page's routes and title, and its <c>[Authorize]</c>.</summary>
internal sealed class Components : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["component.kind", "page.route", "page.title", "security", "security.roles", "security.policy"];

    public ClaimReading? Read(ClaimContext context)
    {
        var type = context.TypeOf(context.Element);
        if (!BlazorExtension.IsRazor(type))
        {
            return null;
        }

        var routes = BlazorExtension.Routes(type!);
        switch (context.Kind)
        {
            case "component.kind":
            {
                // A Dialog is a component the code shows as one; nothing in its own code says so.
                if (context.Expected == "Dialog")
                {
                    return ClaimReading.CantVerify("noRule", "A Razor component's own code doesn't say whether it is a dialog.", type!.At);
                }

                if (routes.Count > 0)
                {
                    return ClaimReading.Compared("Page", routes[0].At, context.Expected == "Page");
                }

                if (context.Code.Supertypes(type!).Any(x => x == "LayoutComponentBase" || x.EndsWith(".LayoutComponentBase", StringComparison.Ordinal)))
                {
                    return ClaimReading.Compared("Layout", type!.At, context.Expected == "Layout");
                }

                return ClaimReading.Absent(type!.At, "Component");
            }
            case "page.route":
                return routes.Count == 0
                    ? ClaimReading.Absent(type!.At)
                    : ClaimReading.Stated(string.Join(", ", routes.Select(x => x.Arguments[0]).Order(StringComparer.Ordinal)), routes[0].At);
            case "security":
            {
                // @attribute [Authorize] secures a page; [AllowAnonymous] opens it. A page with neither is None, from silence.
                if (type!.Attributes.FirstOrDefault(x => x.Name is "Authorize" or "AllowAnonymous") is not { } authorize)
                {
                    return ClaimReading.Absent(type.At, "None");
                }

                var value = authorize.Name == "Authorize" ? "Secured" : "Unsecured";
                return ClaimReading.Compared(value, authorize.At, value == context.Expected);
            }
            case "security.roles" or "security.policy":
            {
                var authorize = type!.Attributes.FirstOrDefault(x => x.Name == "Authorize");
                var value = context.Kind == "security.roles"
                    ? authorize?.Named.GetValueOrDefault("Roles")
                    : authorize?.Named.GetValueOrDefault("Policy") ?? authorize?.Arguments.FirstOrDefault();
                return value is { Length: > 0 }
                    ? ClaimReading.Stated(string.Join(", ", value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)), authorize!.At)
                    : ClaimReading.Absent(authorize?.At ?? type.At);
            }
            default:
            {
                // The page's <PageTitle>: text alone states it; text with code in it is computed.
                if (context.Code.MarkupOf(type!).FirstOrDefault(x => x.Tag == "PageTitle") is not { } title)
                {
                    return ClaimReading.Absent(type!.At);
                }

                return title.Content is { Kind: "string", Value: { } text }
                    ? ClaimReading.Stated(text, title.At)
                    : ClaimReading.CantVerify("unreadable", "The page's <PageTitle> is computed.", title.At);
            }
        }
    }
}

/// <summary>A component's Properties and Event Emitters, and what a Property is to its page.</summary>
internal sealed class Members : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.exists", "member.type", "property.query", "property.route", "property.bindable"];

    public ClaimReading? Read(ClaimContext context)
    {
        // A Property or an Event Emitter of a component, whatever its designer calls them: the vocabulary binds Property or Event.
        var element = context.Element;
        var callback = BlazorExtension.Binds(context, "Event");
        if (context.Kind == "member.exists" && !callback && !BlazorExtension.Binds(context, "Property")
            || element.ParentElement is null || context.TypeOf(element.ParentElement) is not { } type || !BlazorExtension.IsRazor(type))
        {
            return null;
        }

        // Blazor binds a parameter by its name whatever its case: a route's {itemId} is ItemId.
        var member = context.Member ?? context.Code.MembersOf(type).Where(x => x.Kind == "property" && string.Equals(x.Name, element.Name, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Name == element.Name).FirstOrDefault();
        if (member is null)
        {
            return null;
        }

        switch (context.Kind)
        {
            case "member.exists":
            {
                // A parent sets a component's Property, and handles its Event Emitter, only through a parameter.
                // A form-bound value is the page's own code: the designer models a Page's Properties only for route and query parameters.
                if (!callback && BlazorExtension.IsFormBound(member))
                {
                    return ClaimReading.Compared("bound from the form", member.At, false,
                        $"`{member.Name}` is bound from the posted form ([SupplyParameterFromForm]), which the model doesn't state: a Page's Properties are its route and query parameters, and a component's its parameters. Remove the Property.");
                }

                if (!BlazorExtension.IsParameter(member) || callback && !BlazorExtension.IsCallback(member.Type))
                {
                    return ClaimReading.Compared(callback ? "not an EventCallback parameter" : "not a parameter", member.At, false,
                        $"`{member.Name}` isn't a {(callback ? "[Parameter] EventCallback" : "[Parameter]")}, so a parent can't {(callback ? "handle" : "set")} it.");
                }

                return ClaimReading.Found(member);
            }
            case "member.type":
            {
                // An Event Emitter's type is what its EventCallback<T> passes; a bare EventCallback passes nothing.
                if (!BlazorExtension.IsCallback(member.Type))
                {
                    return null;
                }

                var passed = member.Type!.Arguments.FirstOrDefault();
                return context.ExpectedValue is ITypeReference model
                    ? ClaimReading.Compared(passed?.Text ?? "none", member.At, passed is not null && context.TypeMatches(model, passed))
                    : ClaimReading.Compared(passed?.Text ?? "none", member.At, passed is null);
            }
            // What the code leaves out is false from silence: no [SupplyParameterFromQuery], no route segment, no callback.
            case "property.query":
            {
                return member.Attributes.Any(x => x.Name == "SupplyParameterFromQuery")
                    ? ClaimReading.Compared("true", member.At, context.Expected == "true")
                    : ClaimReading.Absent(member.At, "false");
            }
            case "property.route":
            {
                // Blazor binds a route parameter to the parameter property of its name, whatever its case.
                var name = Regex.Escape(member.Name);
                var bound = BlazorExtension.Routes(type).Any(x => Regex.IsMatch(x.Arguments[0], $@"\{{\*?{name}(?:[:?=}}])", RegexOptions.IgnoreCase));
                return bound ? ClaimReading.Compared("true", member.At, context.Expected == "true") : ClaimReading.Absent(member.At, "false");
            }
            default:
            {
                // @bind-X binds a parameter X with an XChanged callback beside it.
                var changed = context.Code.MembersOf(type).FirstOrDefault(x => x.Name == member.Name + "Changed" && x.Kind == "property" && BlazorExtension.IsCallback(x.Type));
                return changed is not null ? ClaimReading.Compared("true", changed.At, context.Expected == "true") : ClaimReading.Absent(member.At, "false");
            }
        }
    }
}

/// <summary>What a component renders, navigates to, opens and calls, read from its markup and code.</summary>
internal sealed class Links : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["intent.uses", "intent.navigates", "intent.opens", "intent.target", "intent.invokes"];

    public ClaimReading? Read(ClaimContext context)
    {
        // A link's claim is on its target end, a Display Component's on the element; the source is a component, or one of its operations.
        var (source, target) = context.Element switch
        {
            IAssociationEnd end => (context.Owner(end), end.TypeReference?.Element as IElement),
            { SpecializationType: "Display Component" } display => (display, display.TypeReference?.Element as IElement),
            _ => (null, null)
        };
        if (BlazorExtension.CodeOf(context, source) is not { } located || target is null)
        {
            return null;
        }

        var (component, type) = located;
        if (!BlazorExtension.IsRazor(type))
        {
            return Class(context, source!, component, type, target);
        }

        // From one of its operations, only that method's code is read.
        var methods = context.Code.MembersOf(type).Where(x => x.Kind == "method").ToList();
        if (source != component && methods.FirstOrDefault(x => x.Name == source!.Name) is { } operation)
        {
            methods = [operation];
        }

        var calls = methods.SelectMany(x => x.Calls).ToList();
        var markup = context.Code.MarkupOf(type).ToList();
        var used = context.TypeOf(target);
        return context.Kind switch
        {
            // A script module it uses (a Behaviour, or any module of a script file) is imported through JS interop, or for a
            // Behaviour loaded for the whole application and acting on an element of its markup.
            "intent.uses" when used is { Kind: not "component" } && BlazorMarkup.ScriptFile(used) is { } script => Script(context, component, target, type, script),
            "intent.uses" when used is null && UiCode.Bound(context, "Behaviour") is not null =>
                ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located."),
            // What it uses that isn't a component (a store, service, utility, view model or proxy) is injected, or held or
            // created as any class's dependency is.
            "intent.uses" when used is { Kind: not "component" } || BlazorExtension.Binds(context, "Used") => Injected(context, type, target, used) is { } injected
                ? ClaimReading.Found(injected, injected.At)
                : used is not null && Dependencies.Find(context.Code, type, used) is { } dependency
                    ? ClaimReading.Found(dependency.Node, dependency.At)
                    : ClaimReading.Absent(type.At),
            "intent.uses" => markup.FirstOrDefault(x => x.Component?.Name == target.Name) is { } tag
                ? ClaimReading.Found(tag, tag.At)
                : ClaimReading.Absent(type.At),
            "intent.opens" => calls.FirstOrDefault(x => x.Name is "Show" or "ShowAsync" or "Open" or "OpenAsync" && x.TypeArguments.Any(a => a.Name == target.Name)) is { } call
                ? ClaimReading.Found(call, call.At)
                : ClaimReading.Absent(type.At),
            "intent.navigates" => Navigates(context, target, markup, calls, type),
            // A Call Service Operation Action: the component's code calls the proxy operation, itself or through the code's own
            // types it calls (CartPage's BasketState calls BasketService), or its markup posts a plain form to the operation.
            "intent.invokes" => Invocation(context, target, methods.Select(x => (type, x))) is { } invoked
                ? ClaimReading.Found(invoked.Call, invoked.Call.At)
                : FormPost(context, component, target, type) ?? ClaimReading.Absent(type.At),
            _ when context.Element is IAssociationEnd { Association.SpecializationType: "Call Service Operation Action" } =>
                Invocation(context, target, methods.Select(x => (type, x))) is { } call
                    ? ClaimReading.Compared(call.Declared is { } declared ? Qualified(declared) : call.Call.ReceiverType?.Text ?? call.Call.Text, call.Call.At, call.OnLocated)
                    : Posted(context, target, type) is { Post: true } form
                        ? ClaimReading.Compared(target.ParentElement?.Name ?? target.Name, form.Node.At, true)
                        : null,
            _ => Target(context, target, markup, type)
        };
    }

    /// <summary>
    /// A link from an element whose code is a plain C# class, not a component: a Store, View Model, UI Service, Utility or
    /// Background Process the designer gives no member for its methods, so the element is the whole class. Its Uses (the
    /// vocabulary binds <c>Used</c>) is a dependency of the class on the used element's type (<see cref="Dependencies"/>). Its
    /// Call Service Operation Action's calls are those of every method and constructor, and of the code's own methods they
    /// reach. Any other source or kind is left to other readers.
    /// </summary>
    private static ClaimReading? Class(ClaimContext context, IElement source, IElement holder, TypeNode type, IElement target)
    {
        if (holder != source || type.Kind == "component" || !type.Files.Any(x => x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        if (context.Kind == "intent.uses" && BlazorExtension.Binds(context, "Used"))
        {
            if (context.TypeOf(target) is not { } used)
            {
                return ClaimReading.CantVerify("blocked", $"Waits on `{target.Name}`, which nothing located.");
            }

            return Dependencies.Find(context.Code, type, used) is { } dependency
                ? ClaimReading.Found(dependency.Node, dependency.At)
                : ClaimReading.Compared("none", type.At, false,
                    $"`{source.Name}` uses `{target.Name}`: in the model, not in the code. No field, property or constructor parameter of `{type.Name}` is a `{used.Name}` or an interface of it, and it creates none. The model states it in its Uses. Give `{type.Name}` the dependency, or remove the Uses.");
        }

        if (context.Element is not IAssociationEnd { Association.SpecializationType: "Call Service Operation Action" } || context.Kind is not ("intent.invokes" or "intent.target"))
        {
            return null;
        }

        var units = context.Code.MembersOf(type).Where(x => x.Kind is "method" or "constructor").Select(x => (type, x)).ToList();
        if (Invocation(context, target, units) is not { } call)
        {
            // What the target is waits on whether it is invoked, which says so.
            return context.Kind == "intent.target" ? null : ClaimReading.Absent(type.At);
        }

        return context.Kind == "intent.invokes"
            ? ClaimReading.Found(call.Call, call.Call.At)
            : ClaimReading.Compared(call.Declared is { } declared ? Qualified(declared) : call.Call.ReceiverType?.Text ?? call.Call.Text, call.Call.At, call.OnLocated);
    }

    /// <summary>
    /// The call of a proxy operation the component's methods make, or the code's own methods they reach (what they call, and
    /// the constructors of what they create), nearest first: of the operation's name, made on its proxy's type, an interface
    /// it implements, or, when nothing locates the proxy, a type of its name. <c>OnLocated</c> is whether the type the call is
    /// made on is the proxy the model's element is located as, or one it implements.
    /// </summary>
    private static (CallNode Call, TypeNode? Declared, bool OnLocated)? Invocation(ClaimContext context, IElement operation, IEnumerable<(TypeNode Type, MemberNode Member)> units)
    {
        var proxy = operation.ParentElement?.Name;
        var located = operation.ParentElement is { } element ? Reach.Located<TypeNode>(context, element, "type.exists") : null;
        bool IsProxy(TypeNode type) => located is not null ? type == located || context.Code.Bases(located).Contains(type) : type.Name == proxy || type.Name == $"I{proxy}";
        var seen = new HashSet<MemberNode>();
        var queue = new Queue<(TypeNode Type, MemberNode Member)>(units.Where(x => seen.Add(x.Member)));
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Member.Calls)
            {
                if (call.Name == operation.Name)
                {
                    var declared = call.Target?.Type ?? (call.ReceiverType is { } receiver ? context.Code.TypeFor(receiver, current.Type) : null);
                    if (declared is not null ? IsProxy(declared) || declared.Name == proxy || declared.Name == $"I{proxy}"
                        : call.ReceiverType?.Name == proxy || call.ReceiverType?.Name == $"I{proxy}")
                    {
                        return (call, declared, declared is null ? located is null : IsProxy(declared));
                    }
                }

                // What it calls, and a method of the code it hands on to be called (AIFunctionFactory.Create(SearchCatalog)).
                IEnumerable<(TypeNode Type, MemberNode Member)> reached = call is { IsReference: true, Target: { } target }
                    ? [(target.Type, target.Member)]
                    : MessageCode.Bodies(context.Code, current.Type, call);
                foreach (var body in reached.Where(x => seen.Add(x.Member)))
                {
                    queue.Enqueue(body);
                }
            }

            foreach (var created in current.Member.Creations)
            {
                if (context.Code.TypeFor(created.Type, current.Type) is { } made)
                {
                    foreach (var constructor in made.Members.Where(x => x.Kind == "constructor" && seen.Add(x)))
                    {
                        queue.Enqueue((made, constructor));
                    }
                }
            }
        }

        return null;
    }

    private static string Qualified(TypeNode type) => type.Namespace.Length > 0 ? $"{type.Namespace}.{type.Name}" : type.Name;

    /// <summary>
    /// A component's Uses of a script module: a JS interop import of its file, else, for a Behaviour (the vocabulary binds
    /// <c>Behaviour</c>), the file loaded for the whole application by a <c>&lt;script src&gt;</c> and acting on an element of
    /// the component's markup, one a selector it passes to the DOM matches. Only a selector naming a class, an id or a
    /// <c>data-</c> attribute marks an element (<see cref="CssSelector.Marks"/>).
    /// </summary>
    private static ClaimReading Script(ClaimContext context, IElement component, IElement target, TypeNode type, string file)
    {
        if (BlazorMarkup.ModuleImport(context.Code, type, file) is { } import)
        {
            return ClaimReading.Found(import, import.At);
        }

        if (UiCode.Bound(context, "Behaviour") is null)
        {
            return ClaimReading.Compared("none", type.At, false,
                $"`{component.Name}` uses `{target.Name}`: in the model, not in the code. `{component.Name}` doesn't import `{file}` through JS interop. The model states it in its Uses. Import the module, or remove the Uses.");
        }

        if (BlazorMarkup.ScriptLoading(context.Code, file) is not { } loading)
        {
            return ClaimReading.Compared("none", type.At, false,
                $"`{component.Name}` uses `{target.Name}`: in the model, not in the code. `{component.Name}` doesn't import `{file}` through JS interop, and no `<script src>` loads it for the application. The model states it in its Uses. Load the script, or remove the Uses.");
        }

        var selectors = BlazorMarkup.MarkingSelectors(context.Code, file);
        if (context.Code.MarkupOf(type).FirstOrDefault(x => selectors.Any(s => s.Matches(x))) is { } marked)
        {
            return ClaimReading.Found(marked, marked.At);
        }

        var acts = selectors.Count == 0 ? "no element by a selector naming a class, an id or a data- attribute" : string.Join(", ", selectors.Select(x => $"`{x.Text}`"));
        return ClaimReading.Compared("none", type.At, false,
            $"`{component.Name}` uses `{target.Name}`: in the model, not in the code. `{file}`, loaded for the application by `<script src=\"{loading.Src}\">` in `{loading.Host.Name}`, acts on {acts}, and no element of `{component.Name}`'s markup matches. The model states it in its Uses. Mark the element it acts on, or remove the Uses.");
    }

    /// <summary>
    /// The routes an operation with Http Settings is served at, as the model gives them: its Route under its Service's Http
    /// Service Settings Route (<c>[controller]</c> filled in, a query string dropped), and its own Route alone when it starts
    /// with a slash, which an attribute route reads as absolute. Empty for an operation without Http Settings.
    /// </summary>
    private static IReadOnlyList<string> Routes(ClaimContext context, IElement operation)
    {
        if (!context.Model.HasStereotype(operation, "Http Settings"))
        {
            return [];
        }

        var own = context.Model.Stereotype(operation, "Http Settings", "Route") ?? "";
        if (operation.ParentElement is not { } service || context.Model.Stereotype(service, "Http Service Settings", "Route") is not { Length: > 0 } prefix)
        {
            return [own];
        }

        prefix = prefix.Split('?')[0].Replace("[controller]", service.Name.EndsWith("Service", StringComparison.Ordinal) ? service.Name[..^"Service".Length] : service.Name);
        var combined = own.Length == 0 ? prefix : prefix.TrimEnd('/') + "/" + own.TrimStart('/');
        return own.StartsWith('/') || own.StartsWith("~/", StringComparison.Ordinal) ? [combined, own] : [combined];
    }

    /// <summary>
    /// The plain form of the component's markup (<see cref="BlazorMarkup.PostedForms"/>) whose action is the operation's route,
    /// and whether the operation takes the POST a form sends. Null when the operation has no Http Settings, or no form posts to
    /// its route.
    /// </summary>
    private static (MarkupNode Node, bool Post)? Posted(ClaimContext context, IElement operation, TypeNode type)
    {
        var routes = Routes(context, operation);
        if (routes.Count == 0)
        {
            return null;
        }

        var post = string.Equals(context.Model.Stereotype(operation, "Http Settings", "Verb"), "POST", StringComparison.OrdinalIgnoreCase);
        return BlazorMarkup.PostedForms(context.Code, type)
            .FirstOrDefault(x => ProxyRequests.Route(x.Action) is { } route && routes.Any(r => ProxyRequests.RouteMatches(route.Segments, r))) is { Node: { } node }
            ? (node, post)
            : null;
    }

    /// <summary>
    /// A component's Call Service Operation Action to an operation with Http Settings, read from its plain form posts: one
    /// posting to the operation's route invokes it when the operation takes a POST. Null when the component posts no form, so
    /// silence says the rest.
    /// </summary>
    private static ClaimReading? FormPost(ClaimContext context, IElement component, IElement operation, TypeNode type)
    {
        var forms = BlazorMarkup.PostedForms(context.Code, type);
        if (forms.Count == 0 || Routes(context, operation) is not [var route, ..])
        {
            return null;
        }

        var verb = context.Model.Stereotype(operation, "Http Settings", "Verb") ?? "";
        switch (Posted(context, operation, type))
        {
            case { Post: true } form:
                return ClaimReading.Found(form.Node, form.Node.At);
            case { } form:
                return ClaimReading.Compared("POST", form.Node.At, false,
                    $"`{component.Name}` posts a form to `{operation.Name}`'s route, but `{operation.Name}` is a `{verb}` endpoint, and a form sends a POST. The model states it in its Call Service Operation Action. Link the operation the form posts to.");
        }

        var actions = forms.Select(x => ProxyRequests.Route(x.Action)).ToList();
        if (actions.Any(x => x is null))
        {
            return ClaimReading.CantVerify("unreadable", "A posted form's action is computed.", forms[actions.FindIndex(x => x is null)].Node.At);
        }

        return ClaimReading.Compared(string.Join(", ", actions.Select(x => x!.Value.Shown)), forms[0].Node.At, false,
            $"`{component.Name}` invokes `{operation.Name}`: in the model, not in the code. `{operation.Name}` is `{verb} {route.Trim('/')}`, and `{component.Name}` posts its forms to {string.Join(", ", actions.Select(x => $"`{x!.Value.Shown}`"))}. The model states it in its Call Service Operation Action. Post the form to the operation's route, or link the operation it posts to.");
    }

    /// <summary>
    /// The member a component has the target injected into (<c>@inject</c>, <c>[Inject]</c>, a field or property of its type):
    /// typed as the type the target is located as, an interface of it, or, with nothing located, a type of its name.
    /// </summary>
    private static MemberNode? Injected(ClaimContext context, TypeNode component, IElement target, TypeNode? located) =>
        context.Code.MembersOf(component)
            .Where(x => x.Kind is "property" or "field" && x.Type is { } t
                        && (located is not null
                            ? context.Code.TypeFor(t, component) is { } resolved && (resolved == located || context.Code.Bases(located).Contains(resolved))
                            : t.Name == target.Name || t.Name == $"I{target.Name}"))
            .OrderByDescending(x => x.Attributes.Any(a => a.Name == "Inject"))
            .FirstOrDefault();

    /// <summary>
    /// An <c>href</c>, a form's <c>action</c> or a <c>NavigateTo</c> to the target page's route, a value in it standing for a
    /// route parameter. One the code computes, where none is to the route, can't be verified.
    /// </summary>
    private static ClaimReading Navigates(ClaimContext context, IElement target, IReadOnlyList<MarkupNode> markup, IReadOnlyList<CallNode> calls, TypeNode type)
    {
        var routes = (target.Stereotypes ?? []).FirstOrDefault(x => x.Name == "Page")?.Properties.FirstOrDefault(x => x.Key == "Route")?.Value?.Split(',') ?? [];
        var patterns = routes.Select(x => x.Trim().Trim('/'))
            .Select(x => new Regex("^" + string.Join("/", x.Split('/').Select(s => s.StartsWith('{') ? "[^/]+" : Regex.Escape(s))) + "$", RegexOptions.IgnoreCase))
            .ToList();
        var places = markup.SelectMany(x => x.Attributes.Where(a => a.Directive is null && (a.Name.Equals("href", StringComparison.OrdinalIgnoreCase) || a.Name == "action")))
            .Select(x => (Uri(x.Literal) is null && Helper(context.Code, x.Value) is { } helper ? helper : x.Literal, x.At))
            .Concat(calls.Where(x => x.Name == "NavigateTo" && x.Arguments.Count > 0).Select(x => ((LiteralNode?)context.Code.StringOf(x.Arguments[0]), x.At)))
            .ToList();
        Span? unreadable = null;
        foreach (var (literal, at) in places)
        {
            if (Uri(literal) is not { } uri)
            {
                unreadable ??= at;
                continue;
            }

            var path = uri.Split('?', '#')[0].Trim('/');
            if (patterns.Any(x => x.IsMatch(path)))
            {
                return ClaimReading.Found(target, at);
            }
        }

        return unreadable is { } computed
            ? ClaimReading.CantVerify("unreadable", "A navigation's URI is computed.", computed)
            : ClaimReading.Absent(type.At);
    }

    /// <summary>
    /// The string an attribute's code gives when it is a call of a static method of the code (<c>@ItemHelper.Url(Item)</c>):
    /// what the method returns, as far as the code says.
    /// </summary>
    private static LiteralNode? Helper(CodeGraph code, string? value) =>
        value is not null && Regex.Match(value.TrimStart('@'), @"^(?:[\w]+\.)*(\w+)\.(\w+)\(") is { Success: true } call
        && code.Types.Where(x => x.Name == call.Groups[1].Value).SelectMany(x => x.Members).FirstOrDefault(x => x.Kind == "method" && x.Name == call.Groups[2].Value) is { Body.Returns: [var returned, ..] }
            ? code.StringOf(returned)
            : null;

    /// <summary>A URI as text, each value in it a route parameter's segment; null when the code gives no text to compare.</summary>
    private static string? Uri(LiteralNode? literal) => literal switch
    {
        { Kind: "string", Value: { } text } => text,
        { Kind: "interpolated", Parts: [{ Literal: { Kind: "string" } }, ..] parts } => string.Concat(parts.Select(x => x.Literal is { Kind: "string", Value: { } text } ? text : "{value}")),
        _ => null
    };

    /// <summary>
    /// The type a Composition's tag, or a Call Service Operation's <c>@inject</c>, resolves to, against the element it targets:
    /// a tag of the same name that resolves to another component reaches that one, not the modelled one.
    /// </summary>
    private static ClaimReading? Target(ClaimContext context, IElement target, IReadOnlyList<MarkupNode> markup, TypeNode type)
    {
        var expected = context.ExpectedValue as IElement ?? target;
        // A Service Proxy in the Services designer is located there.
        var located = context.TypeOf(expected) ?? context.LocatedOf(expected, "type.exists", "Services") as TypeNode;
        if (expected.SpecializationType == "Service Proxy")
        {
            var injected = context.Code.MembersOf(type)
                .Where(x => x.Kind is "property" or "field" && x.Type is { } t && (t.Name == expected.Name || context.Code.TypeFor(t, type) is { } r && r == located))
                .OrderByDescending(x => x.Attributes.Any(a => a.Name == "Inject"))
                .FirstOrDefault();
            return injected is null ? null : Resolved(context.Code.TypeFor(injected.Type!, type), located, expected, injected.At);
        }

        return markup.FirstOrDefault(x => x.Component?.Name == expected.Name) is { Component: { } component } tag
            ? Resolved(context.Code.TypeFor(component, type), located, expected, tag.At)
            : null;
    }

    /// <summary>
    /// The type it resolves to against the target's. When nothing in scope locates the target (a component of a referenced
    /// project outside the application's scope), the tag or name is all the code says, so it is compared by name.
    /// </summary>
    private static ClaimReading Resolved(TypeNode? resolved, TypeNode? located, IElement expected, Span at) => resolved switch
    {
        null when located is null => ClaimReading.Compared(expected.Name, at, true),
        null => ClaimReading.CantVerify("unresolved", "It doesn't resolve to a type in scope.", at),
        _ => ClaimReading.Compared(Qualified(resolved), at, located is null ? resolved.Name == expected.Name : resolved == located)
    };
}
