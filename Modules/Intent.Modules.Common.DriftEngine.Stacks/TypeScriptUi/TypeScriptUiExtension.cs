using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScriptUi;

/// <summary>
/// Reads the User Interface claims (the <c>intent.ui</c> pack's kinds) that TypeScript front ends answer alike, whatever their
/// framework: where a store, service, utility or proxy is, a store's state and actions (Pinia, Redux Toolkit, Zustand), what a
/// module that isn't a component uses, navigates to and calls, and the request a proxy operation sends. A framework's reader
/// (<c>intent.vue</c>, <c>intent.react</c>) reads its own components. Like them, it reads claims and their bindings, never a
/// designer's element types, and answers only for TypeScript and JavaScript code.
/// </summary>
public sealed class TypeScriptUiExtension : IDriftExtension
{
    public string Id => "intent.typescript.ui";

    public IEnumerable<IClaimReader> ClaimReaders { get; } = [new Modules(), new StoreMembers(), new ModuleLinks(), new Requests(), new BaseUrls(), new PersistedStores()];

    public IEnumerable<ICodeReader> Readers { get; } = [new ClassDependencies(), new StoreGetters()];
}

/// <summary>
/// What a TypeScript class's members are to the designer, so <c>unmodelled.member</c> asks only for those it can state: a member
/// holding an instance of a class of the code's own that the class creates (a MobX root store's <c>activityStore</c>, set to
/// <c>new ActivityStore(this)</c> in its constructor), or that its constructor is handed (a store's <c>rootStore</c>, set from
/// its <c>rootStore: RootStore</c> parameter), is a dependency, which the model states as a <c>Uses</c>
/// (<see cref="CodeGraph.StatedByLink"/>), as a React component's created service and an Angular class's injected one are.
/// </summary>
internal sealed class ClassDependencies : ICodeReader
{
    public void Read(CodeGraph graph)
    {
        foreach (var type in graph.Types.Where(x => x.Kind == "class" && UiCode.IsTypeScript(x)))
        {
            var assigned = type.Members.Where(x => x.Kind == "constructor").SelectMany(x => x.Body?.Assignments ?? []).ToList();
            foreach (var member in type.Members.Where(x => x.Kind is "property" or "field" && !x.IsStatic))
            {
                var values = assigned.Where(x => x.Target == $"this.{member.Name}").Select(x => x.Value).Prepend(member.Value).OfType<ValueNode>().ToList();
                if (values.SelectMany(x => x.Creations).Any(x => graph.TypeFor(x.Type, type) is { Kind: "class" or "component" } created && created != type)
                    || values.Any(x => x.Parameter?.Type is { } given && graph.TypeFor(given, type) is { Kind: "class" or "interface" } held && held != type))
                {
                    graph.StatedByLink.TryAdd(member, "Uses");
                }
            }
        }
    }
}

/// <summary>
/// What a TypeScript store derives from its state can't be one of a Store's Properties (<see cref="CodeGraph.Unstatable"/>):
/// the designer states a Store's state and gives it no getters ("Don't model actions or getters; describe them in its
/// comment"), so <c>unmodelled.member</c> doesn't ask for a get-only accessor (a MobX store's <c>get total()</c>), a
/// <c>computed(...)</c> (<c>isAdmin = computed(() =&gt; this.currentUser()?.role === 'admin')</c>) or a signal's read-only view
/// (<c>user = this.currentUser.asReadonly()</c>). The state they read is the Store's Properties.
/// </summary>
internal sealed class StoreGetters : ICodeReader
{
    public void Read(CodeGraph graph) => graph.Unstatable.Add((elementType, type, member) =>
        elementType == "Store" && UiCode.IsTypeScript(type) && Derives(member)
            ? "it derives its value from the store's state, and the designer states a Store's state, not its getters"
            : null);

    private static bool Derives(MemberNode member) =>
        member.Kind == "property" && member.Modifiers.Contains("get") && !member.Modifiers.Contains("set")
        || member.Value?.Call is { } call && (call is { Name: "computed", Receiver: null or "" } || call is { Name: "asReadonly", Arguments.Count: 0 });
}

/// <summary>
/// <c>proxy.baseUrl</c>: the absolute URL a TypeScript proxy's requests start with, before each operation's route. It is the
/// first part of a request's URL the code joins it from, when that is an absolute URL (<c>this.baseUrl + '/api'</c>,
/// <c>`${environment.apiUrl}/orders`</c>), else the scheme and host of a URL written whole, else the <c>baseURL</c> or
/// <c>prefixUrl</c> of the client instance it is sent with (<c>axios.create({ baseURL })</c>). Requests with no absolute URL
/// state none; requests starting with different ones can't be verified.
/// </summary>
internal sealed class BaseUrls : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["proxy.baseUrl"];

    public ClaimReading? Read(ClaimContext context)
    {
        if ((context.Type ?? context.TypeOf(context.Element)) is not { } type || !UiCode.IsTypeScript(type))
        {
            return null;
        }

        var code = context.Code;
        var requests = Requests.RequestsOf(code, type).ToList();
        if (requests.Count == 0)
        {
            return ClaimReading.CantVerify("notFound", $"`{type.Name}` sends no request the engine reads (a fetch, or an HTTP client's verb method).", type.At);
        }

        var bases = requests.Select(x => (Url: BaseOf(code, x.Request), x.Request.At)).Where(x => x.Url is not null).ToList();
        if (bases.Count == 0)
        {
            return ClaimReading.Absent(requests[0].Request.At);
        }

        if (bases.Select(x => x.Url!.TrimEnd('/')).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"`{type.Name}`'s requests start with different base URLs.", bases[0].At);
        }

        var found = bases[0].Url!.TrimEnd('/');
        return ClaimReading.Compared(found, bases[0].At, string.Equals(found, context.Expected.Trim().TrimEnd('/'), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>The absolute URL a request starts with, as described on the class; null when it starts with none.</summary>
    private static string? BaseOf(CodeGraph code, Requests.Sent request)
    {
        if (request.Value is { } value)
        {
            // The first part the URL is joined from: a + chain's first operand, or a template's first value before any text.
            var first = value.Joined.FirstOrDefault()
                        ?? (value.Literal is { Kind: "interpolated", Parts: [{ Literal: null } leading, ..] } ? leading : null);
            if (first is not null && Absolute(UiCode.Text(code.StringOf(first))) is { } joined)
            {
                return joined;
            }
        }

        if (UiCode.Text(request.Url) is { } whole && Uri.TryCreate(whole.Replace(UiCode.Hole, 'x'), UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https")
        {
            return $"{uri.Scheme}://{uri.Authority}";
        }

        var configured = Requests.BaseUrlOf(code, request);
        return configured is null ? null : Absolute(UiCode.Text(code.StringOf(configured)));
    }

    private static string? Absolute(string? text) =>
        text is not null && !text.Contains(UiCode.Hole) && Uri.TryCreate(text, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https" ? text : null;
}

/// <summary>
/// <c>store.persisted</c> for a TypeScript store (Pinia, Redux Toolkit, Zustand): its state is kept across reloads when its
/// definition reads or writes Web Storage (<c>localStorage</c>, <c>sessionStorage</c>, <c>window.</c> either), directly or in
/// the module's own functions it calls, or through what keeps it there: VueUse's <c>useStorage</c>, <c>useLocalStorage</c> and
/// <c>useSessionStorage</c>, Zustand's <c>persist</c> middleware, or a Pinia store's <c>persist</c> option
/// (pinia-plugin-persistedstate). It answers only for a store a store library makes; another stack reads its own.
/// </summary>
internal sealed class PersistedStores : IClaimReader
{
    private const int MaxDepth = 8;
    private static readonly HashSet<string> Storages = ["localStorage", "sessionStorage", "window.localStorage", "window.sessionStorage"];
    private static readonly HashSet<string> Keepers = ["useStorage", "useLocalStorage", "useSessionStorage", "persist"];

    public IReadOnlyCollection<string> Kinds { get; } = ["store.persisted"];

    public ClaimReading? Read(ClaimContext context)
    {
        var module = context.Type;
        if (module is { Kind: "class" } && UiCode.IsTypeScript(module))
        {
            return Class(context, module);
        }

        if (module is not { Kind: "module" } || !UiCode.IsTypeScript(module) || UiCode.FileOf(context.Code, module.At.File) is not { } file
            || Stores.Of(context.Code, file).FirstOrDefault(x => Stores.Is(x, context.Element.Name)) is not { } store)
        {
            return null;
        }

        if (store.Call.Name == "defineStore" && store.Call.Arguments.SelectMany(x => x.Members).FirstOrDefault(x => x.Member == "persist" && x.Value.Text != "false") is { } option)
        {
            return ClaimReading.Stated("true", option.Value.At);
        }

        var calls = new List<CallNode>();
        foreach (var argument in store.Call.Arguments)
        {
            Collect(argument, calls, 0);
        }

        foreach (var call in calls.ToList().Where(x => x.Receiver is null or ""))
        {
            if (module.Members.FirstOrDefault(x => x.Kind == "method" && x.Name == call.Name) is { } function)
            {
                calls.AddRange(context.Code.CallsFrom(module, function).Select(x => x.Call));
            }
        }

        if (calls.FirstOrDefault(Keeps) is { } kept)
        {
            return ClaimReading.Stated("true", kept.At);
        }

        return context.Expected == "true"
            ? ClaimReading.Compared("false", store.Call.At, false,
                $"`{context.Element.Name}` persisted: model `true`, code `false`. The store reads and writes neither `localStorage` nor `sessionStorage`, so its state isn't kept across reloads. The model states it in the store's Persisted stereotype. Keep its state in Web Storage, or remove Persisted.")
            : ClaimReading.Absent(store.Call.At, "false");
    }

    private static bool Keeps(CallNode call) =>
        call.Name is "getItem" or "setItem" && call.Receiver is { } receiver && Storages.Contains(receiver)
        || call.Receiver is null or "" && Keepers.Contains(call.Name);

    /// <summary>
    /// A store written as a class (MobX's): its state is kept across reloads when its own code reads or writes Web Storage, in its
    /// members, their initializers and the reactions its constructor sets up (<c>reaction(() =&gt; this.token, token =&gt;
    /// window.localStorage.setItem('jwt', token))</c>), or through what keeps it there.
    /// </summary>
    private static ClaimReading Class(ClaimContext context, TypeNode store)
    {
        var calls = new List<CallNode>();
        foreach (var value in store.Members.SelectMany(x => (x.Body?.Calls ?? []).SelectMany(c => c.Arguments).Prepend(x.Value)).OfType<ValueNode>())
        {
            Collect(value, calls, 0);
        }

        calls.AddRange(store.Members.SelectMany(x => x.Body?.Calls ?? []));

        if (calls.FirstOrDefault(Keeps) is { } kept)
        {
            return ClaimReading.Stated("true", kept.At);
        }

        return context.Expected == "true"
            ? ClaimReading.Compared("false", store.At, false,
                $"`{context.Element.Name}` persisted: model `true`, code `false`. The store reads and writes neither `localStorage` nor `sessionStorage`, so its state isn't kept across reloads. The model states it in the store's Persisted stereotype. Keep its state in Web Storage, or remove Persisted.")
            : ClaimReading.Absent(store.At, "false");
    }

    /// <summary>The calls a value makes: its own, its arguments', and those of a function it is, nested ones included.</summary>
    private static void Collect(ValueNode value, List<CallNode> calls, int depth)
    {
        if (depth > MaxDepth)
        {
            return;
        }

        if (value.Call is { } call)
        {
            calls.Add(call);
            foreach (var argument in call.Arguments)
            {
                Collect(argument, calls, depth + 1);
            }
        }

        if (value.Function is { } function)
        {
            calls.AddRange(function.Calls);
            foreach (var argument in function.Calls.SelectMany(x => x.Arguments))
            {
                Collect(argument, calls, depth + 1);
            }
        }

        foreach (var item in value.Items.Concat(value.Members.Select(x => x.Value)))
        {
            Collect(item, calls, depth + 1);
        }
    }
}

/// <summary>
/// A store: what a store library makes in a module, by its id or the name of what holds it. Pinia's <c>defineStore('cart')</c>,
/// Redux Toolkit's <c>createSlice({ name: 'cart' })</c>, Zustand's <c>create(...)</c> held by <c>useCartStore</c>.
/// </summary>
internal static class Stores
{
    private static readonly HashSet<string> Makers = ["defineStore", "createSlice", "create", "createStore"];

    public sealed record Store(TypeNode Module, CallNode Call, string? Id, string? Holder);

    /// <summary>Each store the module declares at its top level.</summary>
    public static IEnumerable<Store> Of(CodeGraph code, FileNode file)
    {
        var module = code.Types.FirstOrDefault(x => x.Kind == "module" && x.Files.Contains(file.Path));
        if (module is null)
        {
            yield break;
        }

        foreach (var call in (file.TopLevel?.Calls ?? []).Where(x => x is { Receiver: null, Target: null, Arguments.Count: > 0 } && Makers.Contains(x.Name)))
        {
            var id = call.Arguments.FirstOrDefault() is { } first ? first.Literal?.Value ?? first.Members.FirstOrDefault(x => x.Member is "id" or "name")?.Value.Literal?.Value : null;
            var holder = module.Members.FirstOrDefault(x => x.At.Line == call.At.Line && x.Kind == "field")?.Name;
            // create and createStore are Zustand's only when a use...Store or ...Store holds them.
            if (call.Name is "create" or "createStore" && (holder is null || !holder.EndsWith("Store", StringComparison.Ordinal)))
            {
                continue;
            }

            yield return new Store(module, call, call.Name is "create" or "createStore" ? null : id, holder);
        }
    }

    /// <summary>The store is the element's: its id is the element's name, or what holds it is <c>use{Name}Store</c>, <c>{name}Store</c> or <c>{name}Slice</c>.</summary>
    public static bool Is(Store store, string name)
    {
        var words = UiCode.Words(name);
        return store.Id is { } id && UiCode.Words(id) == words
               || store.Holder is { } holder && UiCode.Words(holder) is var held && (held == $"use{words}store" || held == $"{words}store" || held == $"{words}slice");
    }

    /// <summary>
    /// What a store gives: a Pinia setup function's returned object, or its options' state, getters and actions; a Redux
    /// slice's initial state and reducers; what Zustand's creator returns, through any middleware it is wrapped in.
    /// </summary>
    public static IEnumerable<InitializerNode> Members(CodeGraph code, Store store)
    {
        var arguments = store.Call.Arguments;
        switch (store.Call.Name)
        {
            case "defineStore":
            {
                var given = arguments.Skip(1).FirstOrDefault() ?? arguments[0];
                return given.Function is { } setup
                    ? setup.Returns.SelectMany(x => x.Members)
                    : given.Members.Where(x => x.Member is "state" or "getters" or "actions")
                        .SelectMany(x => x.Value.Function?.Returns.SelectMany(r => r.Members) ?? x.Value.Members);
            }
            case "createSlice":
            {
                var options = arguments.FirstOrDefault()?.Members ?? [];
                return options.Where(x => x.Member is "initialState" or "reducers" or "selectors").SelectMany(x => ObjectOf(x.Value));
            }
            default:
            {
                // create<State>()((set) => ({ ... })), create(persist((set) => ({ ... }), options)).
                var creator = arguments.FirstOrDefault();
                while (creator is { Function: null, Call: { Arguments.Count: > 0 } wrapped })
                {
                    creator = wrapped.Arguments[0];
                }

                return creator?.Function?.Returns.SelectMany(x => x.Members) ?? [];
            }
        }
    }

    /// <summary>An object's members: written in place, or the module variable a name reads.</summary>
    private static IEnumerable<InitializerNode> ObjectOf(ValueNode value) =>
        value.Members.Count > 0 ? value.Members : value.Field?.Member.Value?.Members ?? [];
}

/// <summary>
/// Where a store, module or proxy is: the file its Implementation Path names, else a TypeScript or JavaScript file named for
/// it in a folder its designer folders end with, and in that file the type of its name, else the file's own code. A store is
/// also found by what a store library makes (<see cref="Stores"/>). Components and contracts are left to the engine's own
/// rule, which finds a type by its name.
/// </summary>
internal sealed class Modules : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["type.exists"];

    public ClaimReading? Read(ClaimContext context)
    {
        var code = context.Code;
        var element = context.Element;
        var path = UiCode.Bound(context, "Path");
        var store = UiCode.Bound(context, "Store") is not null;
        var proxy = UiCode.Bound(context, "Proxy") is not null;
        if (path is null && !store && !proxy && UiCode.Bound(context, "Module") is null || !code.Files.Any(x => UiCode.IsTypeScript(x.Path)))
        {
            return null;
        }

        var name = UiCode.Words(element.Name);
        FileNode? file;
        if (path is not null)
        {
            var named = UiCode.Normalized(path);
            file = code.Files.Where(x => UiCode.Names(x.Path, named)).MinBy(x => x.Path.Length);
            if (file is null)
            {
                return context.Scope.Files.FirstOrDefault(x => UiCode.Names(x, named)) is { } excluded && !context.Scope.Includes(excluded)
                    ? ClaimReading.OutOfScope(excluded)
                    : ClaimReading.Missing("the code", $"`{named}`, which its Implementation Path names");
            }
        }
        else
        {
            var folders = UiCode.Folders(element);
            var candidates = code.Files.Where(x => UiCode.IsTypeScript(x.Path) && !x.Path.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase)
                                                   && UiCode.Words(UiCode.Stem(x.Path)) is var stem
                                                   && (stem == name || store && (stem == name + "store" || stem == $"use{name}store" || stem == name + "slice")))
                .ToList();
            file = candidates.Where(x => UiCode.EndsWith(UiCode.Directory(x.Path), folders)).MinBy(x => x.Path.Length)
                   ?? (candidates.Count == 1 ? candidates[0] : null);
            if (file is null && store && code.Files.SelectMany(x => Stores.Of(code, x)).FirstOrDefault(x => Stores.Is(x, element.Name)) is { } defined)
            {
                return ClaimReading.Found(defined.Module, defined.Call.At);
            }

            if (file is null)
            {
                // A store, service or proxy a C# front end declares is a class of its name, which the engine's own rule finds.
                if (code.Types.Any(x => !UiCode.IsTypeScript(x) && UiCode.Words(x.Name) == name))
                {
                    return null;
                }

                return ClaimReading.Missing(folders.Count > 0 ? $"`{string.Join("/", folders)}/`" : "the code", $"a TypeScript file named for `{element.Name}`");
            }
        }

        var types = code.Types.Where(x => x.Files.Contains(file.Path)).ToList();
        var module = types.FirstOrDefault(x => x.Kind is "module" or "component");
        if (store && Stores.Of(code, file).ToList() is { Count: > 0 } stores)
        {
            // The store of its name, else the file's only one.
            if ((stores.FirstOrDefault(x => Stores.Is(x, element.Name)) ?? (stores.Count == 1 ? stores[0] : null)) is { } made)
            {
                return ClaimReading.Found(made.Module, made.Call.At);
            }
        }

        // The type of its name (a proxy's api object, a class), else the only object or class a proxy's file declares, else the file's own code.
        var type = types.FirstOrDefault(x => x.Kind is not ("module" or "interface" or "typeAlias") && UiCode.Words(x.Name) == name)
                   ?? (proxy && types.Where(x => x.Kind is "object" or "class" && x.Access == "public").ToList() is [var only] ? only : null)
                   ?? module;
        return type is null ? ClaimReading.Missing($"`{file.Path}`", "code") : ClaimReading.Found(type, type.At);
    }
}

/// <summary>A store's state, getters and actions: the members a store library's call gives (<see cref="Stores.Members"/>).</summary>
internal sealed class StoreMembers : IClaimReader
{
    public IReadOnlyCollection<string> Kinds { get; } = ["member.exists"];

    public ClaimReading? Read(ClaimContext context)
    {
        var element = context.Element;
        var owner = context.Type ?? (element.ParentElement is { } parent ? context.TypeOf(parent) : null);
        if (owner is not { Kind: "module" } || !UiCode.IsTypeScript(owner) || UiCode.FileOf(context.Code, owner.At.File) is not { } file)
        {
            return null;
        }

        var words = UiCode.Words(element.Name);
        foreach (var store in Stores.Of(context.Code, file))
        {
            if (Stores.Members(context.Code, store).FirstOrDefault(x => UiCode.Words(x.Member) == words) is { } found)
            {
                return ClaimReading.Found(found, found.Value.At);
            }
        }

        return null;
    }
}

/// <summary>
/// The request a TypeScript proxy operation sends: a <c>fetch</c>, or an HTTP client's verb method (axios, an
/// <c>axios.create()</c> instance, Angular's <c>HttpClient</c>, ky, ofetch), in its method or in the code's own helper it
/// calls, whose URL and verb are read with the helper's parameters bound to what the operation passes
/// (<c>request('GET', `/api/products/${id}`)</c> into <c>fetch(`${baseUrl}${path}`, { method })</c>). A base URL before the
/// path isn't part of the route, but the path of a base URL the client joins a relative URL onto is. Any framework's proxy
/// code is read alike.
/// </summary>
internal sealed class Requests : IClaimReader
{
    private static readonly Regex Client = new(@"(?i)(^|\.)(axios|\$?http|httpclient|client|ky|instance|\$fetch|ofetch)$");

    private static readonly string[] Verbs = ["get", "post", "put", "patch", "delete", "head", "options"];

    public IReadOnlyCollection<string> Kinds { get; } = ["proxy.request", "proxy.verb", "proxy.route", "proxy.version"];

    public ClaimReading? Read(ClaimContext context)
    {
        var operation = context.Element is IAssociationEnd end ? context.Owner(end) : context.Element;
        if (operation?.ParentElement is not { } proxy || context.TypeOf(proxy) is not { } type || !UiCode.IsTypeScript(type))
        {
            return null;
        }

        var code = context.Code;
        var method = context.MemberOf(operation)
                     ?? type.Members.FirstOrDefault(x => x.Kind == "method" && UiCode.Words(x.Name) == UiCode.Words(operation.Name));
        if (method?.Body is null)
        {
            return null;
        }

        if (Request(code, type, method) is not { } request)
        {
            return context.Kind == "proxy.request" ? ClaimReading.Absent(method.At) : ClaimReading.CantVerify("notFound", $"`{method.Name}` sends no request the engine reads (a fetch, or an HTTP client's verb method).", method.At);
        }

        switch (context.Kind)
        {
            case "proxy.request":
                return ClaimReading.Compared("present", request.At, context.Expected == "present");
            case "proxy.verb":
                return request.Verb is { } verb
                    ? ClaimReading.Compared(verb, request.At, string.Equals(verb, context.Expected, StringComparison.OrdinalIgnoreCase))
                    : ClaimReading.CantVerify("unreadable", "The request's method is computed.", request.At);
            case "proxy.route":
            {
                if (request.Url is not { } url || Route(url) is not { } written)
                {
                    return ClaimReading.CantVerify("unreadable", "The request's URL is computed.", request.At);
                }

                // A base URL the code doesn't state (an environment variable's, as axios.defaults.baseURL = process.env.REACT_APP_API_URL
                // gives it) may carry the route's start, so the code states only what follows it: the model's route verifies when it
                // ends with the code's.
                var (route, under) = UnderBase(code, request, url, written);
                var same = UiCode.Matches(route.Path, context.Expected)
                           || Configured(code, request, url) && context.Expected.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries) is var segments
                           && Enumerable.Range(1, Math.Max(segments.Length - 1, 0)).Any(skip => UiCode.Matches(route.Path, string.Join("/", segments.Skip(skip))));
                var reading = ClaimReading.Compared(route.Shown, request.At, same);
                return under is { } at ? reading.Through([at]) : reading;
            }
            case "proxy.version":
            {
                // The version the request names where the called route has v{version}, or its api-version query. The model may
                // allow several versions; the request sends one of them.
                if (request.Url is not { } url || Route(url) is not { } written)
                {
                    return ClaimReading.CantVerify("unreadable", "The request's URL is computed.", request.At);
                }

                var route = UnderBase(code, request, url, written).Route;
                var query = UiCode.Text(url) is { } text && text.IndexOf('?') is var at and >= 0 ? text[at..] : "";
                var called = context.ClaimOn(context.Element, "proxy.route")?.Expected ?? "";
                return Http.HttpExtension.VersionIn(called, route.Path + query) is { } version
                    ? ClaimReading.Compared(version, request.At, context.Expected.Split(',', StringSplitOptions.TrimEntries).Any(x => Http.HttpExtension.Version(x) == Http.HttpExtension.Version(version)))
                    : ClaimReading.Absent(request.At);
            }
            default:
                return null;
        }
    }

    /// <summary>
    /// The base URL a request is sent under, as the code gives it: its client instance's <c>baseURL</c> or <c>prefixUrl</c>
    /// (<c>axios.create({ baseURL })</c>), else what the code sets as its client's default (<c>axios.defaults.baseURL = ...</c>).
    /// </summary>
    internal static ValueNode? BaseUrlOf(CodeGraph code, Sent request)
    {
        if (request.Instance?.Value?.Call?.Arguments.FirstOrDefault()?.Members.FirstOrDefault(x => x.Member is "baseURL" or "prefixUrl")?.Value is { } configured)
        {
            return configured;
        }

        if (request.Client is not { } client)
        {
            return null;
        }

        var target = $"{client}.defaults.baseURL";
        return code.Files.Select(x => x.TopLevel).OfType<FunctionNode>().Concat(code.Types.SelectMany(x => x.Members).Select(x => x.Body).OfType<FunctionNode>())
            .SelectMany(x => x.Assignments).FirstOrDefault(x => x.Target == target)?.Value;
    }

    /// <summary>
    /// The route a relative URL is sent to under the base URL its client joins it onto (<see cref="BaseUrlOf"/>), when the code
    /// states that base: the base's path, then the URL. axios joins <c>products</c>, and <c>/products</c> too, onto
    /// <c>http://localhost:5000/api/</c> as <c>http://localhost:5000/api/products</c>, so the route is <c>api/products</c>; ky's
    /// <c>prefixUrl</c> is joined the same way. An absolute URL, one that starts with a value, and a base whose text the code
    /// doesn't give stay as written. <paramref name="Base"/> is where the joined base is given.
    /// </summary>
    private static ((string Path, string Shown) Route, Span? Base) UnderBase(CodeGraph code, Sent request, LiteralNode url, (string Path, string Shown) route)
    {
        if (UiCode.Text(url) is not { Length: > 0 } text || text[0] == UiCode.Hole || Regex.IsMatch(text, @"^([a-z][a-z\d+\-.]*:)?//", RegexOptions.IgnoreCase)
            || BaseUrlOf(code, request) is not { } baseUrl || code.StringOf(baseUrl) is not { Kind: "string", Value: { } stated })
        {
            return (route, null);
        }

        var path = (Uri.TryCreate(stated, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https" ? uri.AbsolutePath : stated).Trim('/');
        return path.Length == 0
            ? (route, null)
            : (($"/{path}/{route.Path.TrimStart('/')}", $"{path}/{route.Shown.TrimStart('/')}"), baseUrl.At);
    }

    /// <summary>
    /// The request's URL starts with what the code doesn't state: a base URL from configuration, as its leading value or its
    /// client's base (<see cref="BaseUrlOf"/>), whose text the code doesn't give.
    /// </summary>
    private static bool Configured(CodeGraph code, Sent request, LiteralNode url) =>
        UiCode.Text(url) is { Length: > 0 } text && text[0] == UiCode.Hole
        || BaseUrlOf(code, request) is { } baseUrl && code.StringOf(baseUrl) is not { Kind: "string" };

    /// <summary>The first request the method sends, nearest first, following the code's own functions with the calls that lead into them.</summary>
    internal static Sent? Request(CodeGraph code, TypeNode type, MemberNode method)
    {
        var seen = new HashSet<FunctionNode> { method.Body! };
        var queue = new Queue<(TypeNode Type, FunctionNode Unit, IReadOnlyList<CallNode> Through)>([(type, method.Body!, [])]);
        while (queue.TryDequeue(out var current))
        {
            foreach (var call in current.Unit.Calls)
            {
                if (Read(code, call, current.Through) is { } sent)
                {
                    return sent;
                }

                foreach (var (owner, member) in UiCode.Reached(code, current.Type, call))
                {
                    if (member.Body is { } body && seen.Add(body))
                    {
                        queue.Enqueue((owner, body, [.. current.Through, call]));
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// A call that sends a request: its verb and URL, read through the calls that lead to it, placed at the operation's own
    /// call that leads there (its <c>request(...)</c>), since a shared helper's <c>fetch</c> is every operation's.
    /// </summary>
    private static Sent? Read(CodeGraph code, CallNode call, IReadOnlyList<CallNode> through)
    {
        var sent = Sends(code, call, through);
        return sent is null || through.Count == 0 ? sent : sent with { At = through[0].At };
    }

    /// <summary>
    /// A request a call sends, read through the calls that lead to it: its verb and URL, where it is, the URL's value as written,
    /// and the client instance it is sent with (<c>axios.create({ baseURL })</c>), when it is.
    /// </summary>
    /// <param name="Client">What an HTTP client's call is made on: <c>axios</c>, or an instance of one.</param>
    internal sealed record Sent(string? Verb, LiteralNode? Url, Span At, ValueNode? Value = null, MemberNode? Instance = null, string? Client = null);

    internal static Sent? Sends(CodeGraph code, CallNode call, IReadOnlyList<CallNode> through)
    {
        string? Literal(ValueNode value) => code.StringOf(value, through) is { Kind: "string", Value: { } text } ? text : null;

        // fetch(url, { method }), $fetch and ofetch alike; GET when no method is given.
        if (call is { Name: "fetch" or "$fetch" or "ofetch", Arguments.Count: > 0 } && call.Target is null)
        {
            var init = call.Arguments.ElementAtOrDefault(1);
            var verb = init?.Members.FirstOrDefault(x => x.Member == "method")?.Value is { } method ? Literal(method) : init is null || init.Members.Count > 0 ? "GET" : null;
            return new Sent(verb?.ToUpperInvariant(), code.StringOf(call.Arguments[0], through), call.At, call.Arguments[0]);
        }

        // Angular's httpResource(() => url) and httpResource(() => ({ url, method, params })), and its text, blob and
        // arrayBuffer forms: a resource a signal's change sends again; GET when no method is given.
        if (call.Target is null && (call is { Name: "httpResource", Receiver: null } || call is { Name: "text" or "blob" or "arrayBuffer", Receiver: "httpResource" })
            && call.Arguments.FirstOrDefault() is { } resource)
        {
            if ((resource.Function is { } function ? function.Returns.FirstOrDefault() : resource) is not { } request)
            {
                return null;
            }

            var url = request.Members.Count > 0 ? request.Members.FirstOrDefault(x => x.Member == "url")?.Value : request;
            var verb = request.Members.FirstOrDefault(x => x.Member == "method")?.Value is { } method ? Literal(method) : "GET";
            return new Sent(verb?.ToUpperInvariant(), url is null ? null : code.StringOf(url, through), call.At, url);
        }

        // axios.get(url), http.post<T>(url, body), and axios({ method, url }) or client.request({ method, url }).
        if (call.Target is null && call.Receiver is { } receiver && (Client.IsMatch(receiver) || Instance(code, receiver) is not null))
        {
            var instance = Instance(code, receiver);
            if (Verbs.Contains(call.Name) && call.Arguments.Count > 0)
            {
                return new Sent(call.Name.ToUpperInvariant(), code.StringOf(call.Arguments[0], through), call.At, call.Arguments[0], instance, receiver);
            }

            if (call.Name == "request" && call.Arguments.FirstOrDefault() is { Members.Count: > 0 } config)
            {
                var verb = config.Members.FirstOrDefault(x => x.Member == "method")?.Value is { } method ? Literal(method) : "GET";
                var url = config.Members.FirstOrDefault(x => x.Member == "url")?.Value;
                return new Sent(verb?.ToUpperInvariant(), url is null ? null : code.StringOf(url, through), call.At, url, instance, receiver);
            }
        }

        return null;
    }

    /// <summary>
    /// A client the code makes from one: a variable given <c>axios.create(...)</c>, <c>ky.create(...)</c> or
    /// <c>ky.extend(...)</c>, whatever it is named (<c>const catalogApi = axios.create({ baseURL })</c>).
    /// </summary>
    private static MemberNode? Instance(CodeGraph code, string receiver)
    {
        var name = receiver[(receiver.LastIndexOf('.') + 1)..];
        return code.Types.Where(x => x.Kind is "module" or "object" or "class" && UiCode.IsTypeScript(x)).SelectMany(x => x.Members)
            .FirstOrDefault(x => x.Kind is "field" or "property" && x.Name == name && x.Value?.Call is { Name: "create" or "extend", Receiver: { } made } && Client.IsMatch(made));
    }

    /// <summary>The requests a proxy's methods send, the first of each, with the method.</summary>
    internal static IEnumerable<(MemberNode Method, Sent Request)> RequestsOf(CodeGraph code, TypeNode type) =>
        type.Members.Where(x => x.Kind == "method" && x.Body is not null).Select(x => (Method: x, Request: Request(code, type, x)))
            .Where(x => x.Request is not null).Select(x => (x.Method, x.Request!));

    /// <summary>
    /// A URL as a route: from its first <c>/</c> after a base URL (a value before the path, or a scheme and host), without its
    /// query string, its values as parameters.
    /// </summary>
    internal static (string Path, string Shown)? Route(LiteralNode url)
    {
        var text = UiCode.Text(url);
        if (text is null)
        {
            return null;
        }

        var shown = UiCode.Shown(url);
        var path = Regex.Replace(text, @"^[a-z]+://[^/]*", "", RegexOptions.IgnoreCase);
        shown = Regex.Replace(shown, @"^[a-z]+://[^/]*", "", RegexOptions.IgnoreCase);
        var slash = path.IndexOf('/');
        if (slash > 0 && path[..slash].All(x => x == UiCode.Hole))
        {
            path = path[slash..];
            shown = shown[shown.IndexOf('/')..];
        }

        if (path.Length == 0 || path.All(x => x == UiCode.Hole))
        {
            return null;
        }

        return (path.Split('?', '#')[0], shown.Split('?', '#')[0].Trim('/'));
    }
}
