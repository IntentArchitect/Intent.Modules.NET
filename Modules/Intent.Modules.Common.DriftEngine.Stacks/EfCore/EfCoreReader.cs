using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>
/// EF Core's fluent configuration, read from the in-scope C# syntax: <c>modelBuilder.Entity&lt;T&gt;(entity =&gt; { ... })</c>
/// and an <c>IEntityTypeConfiguration&lt;T&gt;</c>'s <c>Configure(EntityTypeBuilder&lt;T&gt; builder)</c>. Each statement on
/// the builder is a call chain; a chain that starts with <c>Property(e =&gt; e.X)</c> configures that property.
/// </summary>
internal sealed class EfCoreReader : ICodeReader
{
    /// <summary>The calls with an overload that names members by string: <c>Property("_alias")</c>, <c>HasForeignKey("BuyerId")</c>.</summary>
    private static readonly HashSet<string> NamingCalls =
    [
        "Property", "HasKey", "HasAlternateKey", "HasForeignKey", "HasPrincipalKey", "HasIndex", "HasOne", "HasMany", "WithOne", "WithMany", "Ignore", "Navigation",
        "OwnsOne", "OwnsMany"
    ];

    public void Read(CodeGraph graph)
    {
        if (CSharpSyntax.Of(graph) is not { } csharp)
        {
            return;
        }

        IdentityTypes.AddTo(graph);
        var model = new EfCoreModel();
        foreach (var (file, tree) in csharp.Trees)
        {
            Read(tree.GetRoot(), file, model, csharp);
        }

        // EF Core is in the code when it configures an entity or declares a DbContext; otherwise it reads nothing, so its
        // claim readers leave every claim to another framework's, or to can't verify.
        if (model.Entities.Count > 0 || graph.Types.Any(x => graph.Supertypes(x).Any(s => s.EndsWith("DbContext", StringComparison.Ordinal))))
        {
            graph.Set(model);
            Unmapped(graph, model);
        }
    }

    /// <summary>
    /// The properties EF doesn't map, <c>[NotMapped]</c> or <c>Ignore(e =&gt; e.X)</c>d by their entity's configuration, which no
    /// designer can state: the model persists every attribute of a persisted class, so one of these is <c>column.mapped</c>'s
    /// finding as an attribute, and must not be <c>unmodelled.member</c>'s without one. A migrated EF6 application's entities
    /// wrap an int column in a <c>[NotMapped]</c> enum property.
    /// </summary>
    private static void Unmapped(CodeGraph graph, EfCoreModel model)
    {
        foreach (var type in graph.Types.Where(x => x.Kind is "class" or "record"))
        {
            var ignored = model.Of(type, graph)?.Calls.Where(x => x.Name == "Ignore").SelectMany(x => x.Selected).ToHashSet(StringComparer.Ordinal) ?? [];
            foreach (var member in type.Members.Where(x => x.Kind == "property"))
            {
                if (member.Attributes.Any(x => x.Name == "NotMapped"))
                {
                    graph.Unmodellable[member] = "EF doesn't map it ([NotMapped])";
                }
                else if (ignored.Contains(member.Name))
                {
                    graph.Unmodellable[member] = "EF doesn't map it (its entity's configuration Ignores it)";
                }
            }
        }
    }

    private static void Read(SyntaxNode root, string file, EfCoreModel model, CSharpSyntax csharp)
    {
        foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier.Text: "Entity", TypeArgumentList.Arguments: [var entityType] } }
                && invocation.ArgumentList.Arguments is [{ Expression: SimpleLambdaExpressionSyntax lambda }])
            {
                Configure(model, Name(entityType), lambda.Parameter.Identifier.Text, lambda.Body, file, csharp);
            }
            else if (invocation.Expression is MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier.Text: "Entity", TypeArgumentList.Arguments: [var chainedType] } }
                     && invocation.ArgumentList.Arguments.Count == 0 && invocation.Parent is MemberAccessExpressionSyntax)
            {
                // modelBuilder.Entity<Buyer>().HasIndex(b => b.IdentityGuid).IsUnique(): the chain after Entity<T>() configures T.
                ExpressionSyntax top = invocation;
                while (top.Parent is MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax next } member && member.Expression == top)
                {
                    top = next;
                }

                Add(Config(model, Name(chainedType), invocation, file), Calls((InvocationExpressionSyntax)top, file, csharp).Skip(1).ToList());
            }
            else if (invocation.Expression is MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier.Text: "Owned", TypeArgumentList.Arguments: [var ownedType] } name }
                     && invocation.ArgumentList.Arguments.Count == 0)
            {
                // modelBuilder.Owned<Address>(): Address is owned wherever an entity holds it.
                model.Owned.TryAdd(Name(ownedType), SpanOf(name, file));
            }
        }

        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (method.Identifier.Text == "Configure"
                && method.ParameterList.Parameters is [{ Type: GenericNameSyntax { Identifier.Text: "EntityTypeBuilder", TypeArgumentList.Arguments: [var entityType] } } parameter]
                && ((SyntaxNode?)method.Body ?? method.ExpressionBody) is { } body)
            {
                Configure(model, Name(entityType), parameter.Identifier.Text, body, file, csharp);
                if (method.FirstAncestorOrSelf<TypeDeclarationSyntax>() is { } configuration)
                {
                    model.Configurations[configuration.Identifier.Text] = Name(entityType);
                    model.Entities[Name(entityType)].ConfigurationClasses.Add((configuration.Identifier.Text, SpanOf(configuration, file)));
                }
            }
        }

        foreach (var type in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            Context(model, type, file, csharp);
        }
    }

    /// <summary>A DbContext, a class with <c>OnModelCreating</c> or a <c>DbSet&lt;T&gt;</c>: its default schema and what it maps.</summary>
    private static void Context(EfCoreModel model, ClassDeclarationSyntax type, string file, CSharpSyntax csharp)
    {
        var sets = type.Members.OfType<PropertyDeclarationSyntax>().Select(x => x.Type).OfType<GenericNameSyntax>()
            .Where(x => x.Identifier.Text is "DbSet" or "IDbSet" && x.TypeArgumentList.Arguments.Count == 1).ToList(); // IDbSet: EF6's
        var onModelCreating = type.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(x => x.Identifier.Text == "OnModelCreating");
        var identity = IdentityEntities(type).ToList();
        if (sets.Count == 0 && onModelCreating is null && identity.Count == 0)
        {
            return;
        }

        var context = new ContextConfig { Name = type.Identifier.Text, At = SpanOf(type, file) };
        model.Contexts[context.Name] = context;
        foreach (var set in sets)
        {
            Maps(context, context.Entities, Name(set.TypeArgumentList.Arguments[0]), set, file);
        }

        foreach (var (entity, at) in identity)
        {
            Maps(context, context.Entities, Name(entity), at, file);
        }

        foreach (var invocation in onModelCreating?.DescendantNodes().OfType<InvocationExpressionSyntax>() ?? [])
        {
            if (invocation.Expression is not MemberAccessExpressionSyntax access)
            {
                continue;
            }

            switch (access.Name)
            {
                case GenericNameSyntax { Identifier.Text: "Entity", TypeArgumentList.Arguments: [var entity] }:
                    Maps(context, context.Entities, Name(entity), access.Name, file);
                    break;
                case { Identifier.Text: "ApplyConfiguration" } when invocation.ArgumentList.Arguments is [{ Expression: BaseObjectCreationExpressionSyntax created }]:
                    if (created is ObjectCreationExpressionSyntax { Type: var configuration })
                    {
                        Maps(context, context.Configurations, Name(configuration), access.Name, file);
                    }

                    break;
                case { Identifier.Text: "ApplyConfigurationsFromAssembly" }:
                    context.AppliesAssembly = true;
                    // typeof(OrderingContext).Assembly names the assembly by a type in it.
                    context.Assembly ??= (SpanOf(access.Name, file),
                        invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression is MemberAccessExpressionSyntax { Expression: TypeOfExpressionSyntax of, Name.Identifier.Text: "Assembly" } ? Name(of.Type) : null);
                    break;
                case { Identifier.Text: "HasDefaultSchema" }:
                    context.DefaultSchema = Calls(invocation, file, csharp).LastOrDefault();
                    break;
            }
        }

        foreach (var method in type.Members.OfType<MethodDeclarationSyntax>().Where(x => x.Identifier.Text is "OnModelCreating" or "ConfigureConventions"))
        {
            foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (invocation.Expression is MemberAccessExpressionSyntax { Name: GenericNameSyntax { Identifier.Text: "Properties", TypeArgumentList.Arguments: [var propertyType] } })
                {
                    Convention(context, invocation, csharp.TypeOf(propertyType, file), file, csharp);
                }
            }
        }
    }

    /// <summary>
    /// What a context configures for every property of a type: EF6's <c>modelBuilder.Properties&lt;string&gt;().Configure(c =&gt;
    /// c.HasMaxLength(50))</c>, and EF Core's <c>configurationBuilder.Properties&lt;string&gt;().HaveMaxLength(50)</c> or
    /// <c>Properties&lt;string&gt;(b =&gt; b.HaveMaxLength(50))</c> in <c>ConfigureConventions</c>. EF Core's calls are named as a
    /// property's own (<c>HaveMaxLength</c> is <c>HasMaxLength</c>, <c>AreUnicode</c> is <c>IsUnicode</c>), so one rule reads
    /// both. One filtered by <c>Where</c> or <c>Having</c> configures only the properties its condition picks, which can't be
    /// told without running it, so it isn't read.
    /// </summary>
    private static void Convention(ContextConfig context, InvocationExpressionSyntax properties, TypeRefNode type, string file, CSharpSyntax csharp)
    {
        var calls = new List<FluentCall>();
        var builders = properties.ArgumentList.Arguments.Select(x => x.Expression).OfType<SimpleLambdaExpressionSyntax>().ToList();
        ExpressionSyntax current = properties;
        while (current.Parent is MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax next } member && member.Expression == current)
        {
            switch (member.Name.Identifier.Text)
            {
                case "Where" or "Having":
                    return;
                case "Configure":
                    builders.AddRange(next.ArgumentList.Arguments.Select(x => x.Expression).OfType<SimpleLambdaExpressionSyntax>());
                    break;
                default:
                    calls.Add(Calls(next, file, csharp)[^1]);
                    break;
            }

            current = next;
        }

        foreach (var builder in builders)
        {
            foreach (var chain in builder.Body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Where(x => x.Parent is not MemberAccessExpressionSyntax))
            {
                if (Calls(chain, file, csharp) is [{ } first, ..] chained && first.Receiver == builder.Parameter.Identifier.Text)
                {
                    calls.AddRange(chained);
                }
            }
        }

        foreach (var call in calls)
        {
            var name = call.Name.StartsWith("Have", StringComparison.Ordinal) ? "Has" + call.Name[4..]
                : call.Name.StartsWith("Are", StringComparison.Ordinal) ? "Is" + call.Name[3..]
                : call.Name;
            context.Conventions.Add((type, call with { Name = name }));
        }
    }

    /// <summary>
    /// The entities ASP.NET Core Identity's DbContext base maps through the <c>DbSet</c>s it declares itself:
    /// <c>IdentityUserContext&lt;TUser, ...&gt;</c> its user, and <c>IdentityDbContext&lt;TUser, TRole, ...&gt;</c> its user and
    /// role, so <c>ApplicationDbContext : IdentityDbContext&lt;ApplicationUser&gt;</c> maps <c>ApplicationUser</c>.
    /// </summary>
    private static IEnumerable<(TypeSyntax Entity, SyntaxNode At)> IdentityEntities(ClassDeclarationSyntax type)
    {
        foreach (var name in type.BaseList?.Types.Select(x => x.Type).OfType<GenericNameSyntax>() ?? [])
        {
            var arguments = name.TypeArgumentList.Arguments;
            switch (name.Identifier.Text)
            {
                case "IdentityUserContext":
                    yield return (arguments[0], name);
                    break;
                case "IdentityDbContext":
                    yield return (arguments[0], name);
                    if (arguments.Count > 1)
                    {
                        yield return (arguments[1], name);
                    }

                    break;
            }
        }
    }

    private static void Maps(ContextConfig context, HashSet<string> names, string name, SyntaxNode at, string file)
    {
        names.Add(name);
        context.MappedAt.TryAdd(name, SpanOf(at, file));
    }

    private static void Configure(EfCoreModel model, string entity, string builder, SyntaxNode body, string file, CSharpSyntax csharp)
    {
        var config = Config(model, entity, body, file);
        // An expression-bodied lambda's body is the chain itself: Entity<UserActivity>(x => x.HasKey(...)).
        foreach (var chain in body.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Where(x => x.Parent is not MemberAccessExpressionSyntax))
        {
            var calls = Calls(chain, file, csharp);
            if (calls.Count > 0 && calls[0].Receiver == builder)
            {
                Add(config, calls);
            }
        }
    }

    private static EntityConfig Config(EfCoreModel model, string entity, SyntaxNode at, string file)
    {
        if (!model.Entities.TryGetValue(entity, out var config))
        {
            config = model.Entities[entity] = new EntityConfig { Entity = entity, At = SpanOf(at, file) };
        }

        return config;
    }

    /// <summary>One chain on an entity's builder: a property's, from its <c>Property(e =&gt; e.X)</c> on, or the entity's own.</summary>
    private static void Add(EntityConfig config, List<FluentCall> calls)
    {
        if (calls.Count == 0)
        {
            return;
        }

        if (calls[0].Name == "Property" && calls[0].Selected is [var property])
        {
            if (!config.Properties.TryGetValue(property, out var list))
            {
                list = config.Properties[property] = [];
            }

            list.AddRange(calls);
            if (calls[0].TypeArguments is [var type] && calls[0].Arguments is [var argument] && argument == property)
            {
                config.Declared[property] = (type, calls[0].At);
            }
        }
        else
        {
            calls = Ef6Relationship(calls);
            config.Calls.AddRange(calls);
            config.Chains.Add(calls);
        }
    }

    /// <summary>
    /// An EF6 relationship chain as EF Core writes it, so every relationship rule reads one API (ClientB's API application's
    /// DataContext is EF6): <c>HasRequired(e =&gt; e.X)</c> is <c>HasOne(e =&gt; e.X)</c>, <c>HasOptional</c> too, and the <c>WithRequired</c>
    /// and <c>WithOptional</c> forms are <c>WithOne</c>. <c>IsRequired()</c> is added, at the call that states it, where EF
    /// Core's would mean the same: a required reference with many or no inverse (<c>HasRequired(e =&gt; e.Account)</c>), or a
    /// collection whose inverse is required (<c>HasMany(e =&gt; e.Items).WithRequired(e =&gt; e.Account)</c>). A one-to-one
    /// with one optional end is left to its foreign key, as EF Core's <c>IsRequired()</c> there names only the dependent.
    /// </summary>
    private static List<FluentCall> Ef6Relationship(List<FluentCall> calls)
    {
        if (calls[0].Name is not ("HasRequired" or "HasOptional" or "HasMany"))
        {
            return calls;
        }

        var with = calls.FirstOrDefault(x => x.Name.StartsWith("With", StringComparison.Ordinal));
        var requiredBy = calls[0].Name == "HasRequired" && (with is null || with.Name == "WithMany" || with.Name.StartsWith("WithRequired", StringComparison.Ordinal)) ? calls[0]
            : calls[0].Name == "HasMany" && with?.Name.StartsWith("WithRequired", StringComparison.Ordinal) == true ? with
            : null;
        var translated = calls.Select(x => x.Name switch
        {
            "HasRequired" or "HasOptional" => x with { Name = "HasOne" },
            "WithRequired" or "WithRequiredDependent" or "WithRequiredPrincipal" or "WithOptional" or "WithOptionalDependent" or "WithOptionalPrincipal" => x with { Name = "WithOne" },
            _ => x
        }).ToList();
        if (requiredBy is not null && translated.All(x => x.Name != "IsRequired"))
        {
            translated.Add(new FluentCall { Name = "IsRequired", At = requiredBy.At });
        }

        return translated;
    }

    /// <summary>A call chain, first call first: <c>entity.Property(e =&gt; e.X).HasMaxLength(50)</c>.</summary>
    private static List<FluentCall> Calls(InvocationExpressionSyntax last, string file, CSharpSyntax csharp)
    {
        var calls = new List<FluentCall>();
        ExpressionSyntax current = last;
        while (current is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } invocation)
        {
            var arguments = invocation.ArgumentList.Arguments;
            var lambdas = arguments.Select(x => x.Expression).OfType<LambdaExpressionSyntax>().ToList();

            // A call names members by its lambdas, or else by strings when every argument is one: HasIndex(e => e.A, "IX_A")
            // names its index, not a member.
            var named = lambdas.Count == 0 && NamingCalls.Contains(access.Name.Identifier.Text) && arguments.Count > 0
                        && arguments.All(x => x.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression));
            calls.Insert(0, new FluentCall
            {
                Name = access.Name.Identifier.Text,
                Arguments = arguments.Select(x => Value(x.Expression)).ToList(),
                Written = arguments.Select(x => x.Expression.ToString()).ToList(),
                Selected = named ? arguments.Select(x => Value(x.Expression)).ToList() : lambdas.SelectMany(Selected).ToList(),
                TypeArguments = access.Name is GenericNameSyntax generic ? generic.TypeArgumentList.Arguments.Select(x => csharp.TypeOf(x, file)).ToList() : [],
                At = SpanOf(access.Name, file)
            });
            current = access.Expression;
        }

        if (calls.Count > 0 && current is IdentifierNameSyntax receiver)
        {
            calls[0] = calls[0] with { Receiver = receiver.Identifier.Text };
        }

        return calls;
    }

    /// <summary>The members a selector lambda names: <c>e =&gt; e.Id</c>, <c>e =&gt; new { e.A, e.B }</c>.</summary>
    private static IEnumerable<string> Selected(LambdaExpressionSyntax lambda) => lambda.Body switch
    {
        MemberAccessExpressionSyntax access => [access.Name.Identifier.Text],
        AnonymousObjectCreationExpressionSyntax anonymous => anonymous.Initializers.Select(x => x.Expression).OfType<MemberAccessExpressionSyntax>().Select(x => x.Name.Identifier.Text),
        _ => []
    };

    /// <summary>A string literal's value, otherwise the argument as written.</summary>
    private static string Value(ExpressionSyntax expression) =>
        expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression) ? literal.Token.ValueText : expression.ToString();

    private static string Name(TypeSyntax type) => type switch
    {
        QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
        SimpleNameSyntax simple => simple.Identifier.Text,
        _ => type.ToString()
    };

    private static Span SpanOf(SyntaxNode node, string file) => CSharpSyntax.SpanOf(node, file);
}
