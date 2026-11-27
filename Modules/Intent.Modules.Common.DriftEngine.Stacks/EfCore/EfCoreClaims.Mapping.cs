using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Stacks.Relational;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>
/// How EF Core states what the model says about mapping a class's members and relationships: whether a member is mapped,
/// owned, a concurrency token, stored through a conversion, a field or a hi-lo sequence, and how a relationship deletes,
/// navigates and names its foreign key column.
/// </summary>
internal sealed partial class EfCoreClaims
{
    private static readonly HashSet<string> NumberTypes = new(StringComparer.Ordinal)
    {
        "int", "long", "short", "byte", "sbyte", "uint", "ulong", "ushort", "Int32", "Int64", "Int16", "Byte", "SByte", "UInt32", "UInt64", "UInt16"
    };

    /// <summary>The type of the class the claim's element belongs to; null when its <c>type.exists</c> located none.</summary>
    private static TypeNode? OwnerType(ClaimContext context) => context.Owner(context.Element) is { } owner ? context.TypeOf(owner) : null;

    /// <summary>
    /// Two configuration classes for one entity, both reaching its namespace: which configures it is the DbContext's choice,
    /// so a value read from either, or from their silence, isn't the code's.
    /// </summary>
    private static ClaimReading? TwoConfigurations(ClaimContext context, EntityConfig? entity, TypeNode type)
    {
        var classes = entity?.ConfigurationClasses
            .Where(x => context.Code.Files.FirstOrDefault(f => f.Path == x.At.File) is not { } file || context.Code.Reaches(file, type.Namespace))
            .ToList() ?? [];
        return classes.Count > 1
            ? ClaimReading.CantVerify("ambiguous", $"{classes.Count} configuration classes configure `{type.Name}`.", classes[0].At, classes.Select(x => x.At).ToList())
            : null;
    }

    /// <summary>
    /// Whether EF Core maps an attribute's member to a column: <c>Ignore(o =&gt; o.Total)</c> or <c>[NotMapped]</c> says it
    /// doesn't. A field is mapped only when the configuration names it (<c>Property("_cardNumber")</c>), as EF Core never maps
    /// a field by convention; a property is mapped by convention unless the code says otherwise, so its silence is <c>true</c>.
    /// An unmapped property can't be an attribute of a persisted class at all (<see cref="CodeGraph.Unmodellable"/>), so the
    /// code settles it: the correction removes the attribute.
    /// </summary>
    private static ClaimReading? Mapped(ClaimContext context, EfCoreModel model)
    {
        if (context.Member is not { } member || OwnerType(context) is not { } type)
        {
            return null;
        }

        var entity = EntityOf(context, model, type);
        if (TwoConfigurations(context, entity, type) is { } ambiguous)
        {
            return ambiguous;
        }

        var remove = new ModelCorrection(ModelCorrection.RemoveElement, context.Element.Id);
        if (entity?.Calls.LastOrDefault(x => x.Name == "Ignore" && x.Selected.Contains(member.Name)) is { } ignored)
        {
            return ClaimReading.Stated("false", ignored.At).Correcting(remove);
        }

        if (member.Attributes.FirstOrDefault(x => x.Name == "NotMapped") is { } notMapped)
        {
            return ClaimReading.Stated("false", notMapped.At).Correcting(remove);
        }

        var configured = entity?.Properties.GetValueOrDefault(member.Name);
        if (configured is [var property, ..])
        {
            return ClaimReading.Stated("true", property.At);
        }

        return ClaimReading.Defaulted(member.Kind == "field" ? "false" : "true", member.At);
    }

    /// <summary>
    /// Whether EF Core maps a value object attribute as a value, which is all a Value Object type states: as an owned type
    /// (<c>OwnsOne</c> or <c>OwnsMany</c> of it, <c>[Owned]</c> on its type, <c>modelBuilder.Owned&lt;T&gt;()</c>), a complex
    /// type (<c>ComplexProperty</c>, <c>[ComplexType]</c>), or one column through a value converter
    /// (<c>Property(x =&gt; x.AccountId).HasConversion(...)</c>, or a context's <c>ConfigureConventions</c> converting its
    /// type). The designer has no construct choosing among them, so any verifies (user decision, 2026-11-26). EF Core never
    /// maps a class as a value unless the code says so, so silence is <c>false</c>.
    /// </summary>
    private static ClaimReading? Owned(ClaimContext context, EfCoreModel model)
    {
        if (context.Member is not { } member || OwnerType(context) is not { } type)
        {
            return null;
        }

        var entity = EntityOf(context, model, type);
        if (TwoConfigurations(context, entity, type) is { } ambiguous)
        {
            return ambiguous;
        }

        if (entity?.Chains.LastOrDefault(x => x[0].Name is "OwnsOne" or "OwnsMany" or "ComplexProperty" or "ComplexCollection" && x[0].Selected.Contains(member.Name)) is { } chain)
        {
            return ClaimReading.Stated("true", chain[0].At);
        }

        if (entity?.Properties.GetValueOrDefault(member.Name)?.LastOrDefault(x => x.Name == "HasConversion") is { } conversion)
        {
            return ClaimReading.Stated("true", conversion.At);
        }

        var held = member.Type is { } reference ? context.Code.TypeFor(reference.Arguments is [var item] ? item : reference, type) : null;
        if (held?.Attributes.FirstOrDefault(x => x.Name is "Owned" or "ComplexType") is { } annotated)
        {
            return ClaimReading.Stated("true", annotated.At);
        }

        if ((held?.Name ?? member.Type?.Name) is { } name && model.Owned.TryGetValue(name, out var owned))
        {
            return ClaimReading.Stated("true", owned);
        }

        // configurationBuilder.Properties<AccountId>().HaveConversion<AccountIdConverter>(), for every property of its type.
        if (member.Type is { } valueType && model.ContextsOf(type.Name).SelectMany(x => x.Conventions)
                .Where(x => x.Call.Name == "HasConversion" && x.Type.Name == (valueType is { Name: "Nullable", Arguments: [var inner] } ? inner.Name : valueType.Name))
                .Select(x => x.Call).FirstOrDefault() is { } converted)
        {
            return ClaimReading.Stated("true", converted.At);
        }

        return ClaimReading.Absent(entity?.At ?? member.At, "false");
    }

    /// <summary>
    /// Whether a member is the entity's concurrency token: <c>IsRowVersion()</c>, <c>IsConcurrencyToken()</c>,
    /// <c>[Timestamp]</c> or <c>[ConcurrencyCheck]</c>. EF Core checks no concurrency unless the code says so.
    /// </summary>
    private static ClaimReading? Version(ClaimContext context, EfCoreModel model)
    {
        if (context.Member is not { } member || OwnerType(context) is not { } type)
        {
            return null;
        }

        var entity = EntityOf(context, model, type);
        if (TwoConfigurations(context, entity, type) is { } ambiguous)
        {
            return ambiguous;
        }

        if (entity?.Properties.GetValueOrDefault(member.Name)?.LastOrDefault(x => x.Name is "IsRowVersion" or "IsConcurrencyToken") is { } token)
        {
            return ClaimReading.Stated(token.Arguments is ["false"] ? "false" : "true", token.At);
        }

        return member.Attributes.FirstOrDefault(x => x.Name is "Timestamp" or "ConcurrencyCheck") is { } annotation
            ? ClaimReading.Stated("true", annotation.At)
            : ClaimReading.Absent(member.At, "false");
    }

    /// <summary>
    /// How an enum is stored: <c>HasConversion&lt;string&gt;()</c> (or an <c>EnumToStringConverter</c>) is <c>String</c>, a
    /// conversion to an integer type is <c>Number</c>. EF Core's default, its number, is never read in place of a modelled
    /// storage (decision 4); a converter whose target only its code says is <c>unreadable</c>.
    /// </summary>
    private static ClaimReading? EnumStorage(ClaimContext context, EfCoreModel model)
    {
        if (context.Member is not { } member || OwnerType(context) is not { } type)
        {
            return null;
        }

        var entity = EntityOf(context, model, type);
        if (TwoConfigurations(context, entity, type) is { } ambiguous)
        {
            return ambiguous;
        }

        var calls = entity?.Properties.GetValueOrDefault(member.Name) ?? [];
        if (calls.LastOrDefault(x => x.Name == "HasConversion") is not { } conversion)
        {
            return ClaimReading.Absent(calls.FirstOrDefault()?.At ?? member.At);
        }

        var stored = conversion.TypeArguments.Count > 0 ? Storage(conversion.TypeArguments[0].Name)
            : conversion.Written is [var written] ? StorageOf(written)
            : null;
        return stored is not null
            ? ClaimReading.Stated(stored, conversion.At)
            : ClaimReading.CantVerify("unreadable", $"The code converts it with `HasConversion({string.Join(", ", conversion.Written)})`, which the rule can't read without running the code: what it stores is in the converter's code.", conversion.At);

        static string? Storage(string name) =>
            name is "string" or "String" || name.StartsWith("EnumToString", StringComparison.Ordinal) ? "String"
            : NumberTypes.Contains(name) || name.StartsWith("EnumToNumber", StringComparison.Ordinal) ? "Number"
            : null;

        // HasConversion(typeof(string)), HasConversion(new EnumToStringConverter<OrderStatus>()).
        static string? StorageOf(string written) =>
            written.StartsWith("typeof(", StringComparison.Ordinal) ? Storage(written["typeof(".Length..].TrimEnd(')').Split('.')[^1])
            : written.StartsWith("new ", StringComparison.Ordinal) ? Storage(written["new ".Length..].Split('<', '(')[0].Split('.')[^1])
            : null;
    }

    /// <summary>
    /// The sequence a key takes its values from in blocks: <c>UseHiLo("orderseq")</c>. <c>UseHiLo()</c> with no name uses EF
    /// Core's default sequence, which a modelled one is never read as (decision 4).
    /// </summary>
    private static ClaimReading? HiLo(ClaimContext context, EfCoreModel model)
    {
        if (context.Member is not { } member || OwnerType(context) is not { } type)
        {
            return null;
        }

        var entity = EntityOf(context, model, type);
        if (TwoConfigurations(context, entity, type) is { } ambiguous)
        {
            return ambiguous;
        }

        var calls = entity?.Properties.GetValueOrDefault(member.Name) ?? [];
        if (calls.LastOrDefault(x => x.Name is "UseHiLo" or "UseSequenceHiLo" or "ForSqlServerUseSequenceHiLo") is not { Arguments.Count: > 0 } hilo)
        {
            return ClaimReading.Absent(calls.FirstOrDefault()?.At ?? member.At);
        }

        var arguments = new Arguments(context);
        var sequence = arguments.Of(hilo, 0);
        return sequence is null && arguments.Unread is { } unread ? arguments.Unreadable(unread) : Value(sequence, hilo.At);
    }

    /// <summary>
    /// The field an attribute is mapped through: the configuration maps the field by name (<c>Property("_cardNumber")</c>), or
    /// the attribute's property through it (<c>Property(p =&gt; p.CardNumber).HasField("_number")</c>). A field EF Core would
    /// find by convention is never read in place of a modelled one (decision 4).
    /// </summary>
    private static ClaimReading? BackingField(ClaimContext context, EfCoreModel model)
    {
        if ((context.Type ?? OwnerType(context)) is not { } type)
        {
            return null;
        }

        var entity = EntityOf(context, model, type);
        if (TwoConfigurations(context, entity, type) is { } ambiguous)
        {
            return ambiguous;
        }

        if (entity?.Properties.GetValueOrDefault(context.Expected) is [var mapped, ..])
        {
            return ClaimReading.Stated(context.Expected, mapped.At);
        }

        return entity?.Properties.GetValueOrDefault(context.Element.Name)?.LastOrDefault(x => x.Name == "HasField") is { Arguments: [var field] } through
            ? ClaimReading.Stated(field, through.At)
            : ClaimReading.Absent(entity?.At ?? type.At);
    }

    /// <summary>
    /// The chains that configure an association's relationship, from either side: one that starts at a navigation of it
    /// (<c>HasOne(o =&gt; o.Buyer)</c> in Order's configuration), or at the other class's navigation back to its owner, whose
    /// <c>WithOne</c> or <c>WithMany</c> names it (<c>HasMany(b =&gt; b.Orders).WithOne(o =&gt; o.Buyer)</c>).
    /// </summary>
    private static List<(IReadOnlyList<FluentCall> Chain, TypeNode From, MemberNode Navigation)> RelationshipChains(ClaimContext context, EfCoreModel model, IAssociationEnd association)
    {
        var chains = new List<(IReadOnlyList<FluentCall>, TypeNode, MemberNode)>();
        foreach (var end in new[] { association, association.OtherEnd() }.OfType<IAssociationEnd>().Where(x => x.IsNavigable))
        {
            if (context.MemberOf(end) is not { } navigation || context.Owner(end) is not { } ownerElement || context.TypeOf(ownerElement) is not { } owner)
            {
                continue;
            }

            chains.AddRange((EntityOf(context, model, owner)?.Chains ?? [])
                .Where(x => x[0].Name is "HasOne" or "HasMany" or "OwnsOne" or "OwnsMany" && x[0].Selected is [var named] && named == navigation.Name)
                .Select(x => (x, owner, navigation)));
            if (end.TypeReference?.Element is IElement targetElement && context.TypeOf(targetElement) is { } target)
            {
                chains.AddRange((EntityOf(context, model, target)?.Chains ?? [])
                    .Where(x => x[0].Name is "HasOne" or "HasMany" && Leads(context, x[0], target, owner)
                                && x.Any(c => c.Name is "WithOne" or "WithMany" && c.Selected is [var named] && named == navigation.Name))
                    .Select(x => (x, target, navigation)));
            }
        }

        return chains.DistinctBy(x => x.Item1).ToList();
    }

    /// <summary>A chain's first call leads to a type: the navigation it names holds it, or its type argument names it.</summary>
    private static bool Leads(ClaimContext context, FluentCall call, TypeNode from, TypeNode to) => call.Selected is [var named]
        ? context.Code.MembersOf(from).Any(x => x.Name == named && Navigations.Holds(x, to))
        : call.TypeArguments.Any(x => x.Name == to.Name);

    /// <summary>
    /// The class an attribute's References stereotype names, as its vocabulary binds it (<c>{Target}</c>), else as the
    /// stereotype's Target holds it.
    /// </summary>
    private static string? ReferenceTarget(ClaimContext context)
    {
        if (context.Claim.Bindings?.GetValueOrDefault("Target") is { Length: > 0 } bound)
        {
            return bound;
        }

        var property = context.Element.Stereotypes?.FirstOrDefault(x => x.Name == "References")?.Properties.FirstOrDefault(x => x.Key == "Target");
        return (property as IStereotypeProperty<IElement>)?.Value?.Name
               ?? (property?.Value is { Length: > 0 } id ? context.Model.Elements(context.Claim.Designer).FirstOrDefault(x => x.Id == id)?.Name : null);
    }

    /// <summary>
    /// The chains that make an attribute a foreign key to a class with no navigation on either side:
    /// <c>HasOne&lt;PaymentMethod&gt;().WithMany().HasForeignKey(o =&gt; o.PaymentId)</c> in the owner's configuration, or
    /// <c>HasMany&lt;Order&gt;().WithOne().HasForeignKey(o =&gt; o.PaymentId)</c> in the target's.
    /// </summary>
    private static List<IReadOnlyList<FluentCall>> ReferenceChains(ClaimContext context, EfCoreModel model, TypeNode owner, string key, string target)
    {
        bool Bare(IReadOnlyList<FluentCall> chain, string toward) =>
            chain[0].Name is "HasOne" or "HasMany" && chain[0].Selected.Count == 0 && chain[0].TypeArguments is [var named] && named.Name == toward
            && chain.All(x => x.Name is not ("WithOne" or "WithMany") || x.Selected.Count == 0)
            && chain.LastOrDefault(x => x.Name == "HasForeignKey") is { Selected: [var foreignKey] } && foreignKey == key;

        var targetType = context.Code.Types.FirstOrDefault(x => x.Name == target);
        return (EntityOf(context, model, owner)?.Chains.Where(x => Bare(x, target)) ?? [])
            .Concat(targetType is null ? [] : EntityOf(context, model, targetType)?.Chains.Where(x => Bare(x, owner.Name)) ?? [])
            .ToList();
    }

    /// <summary>
    /// What a relationship does to its dependents when its principal is deleted: <c>OnDelete(DeleteBehavior.Restrict)</c> on
    /// the chain that configures it, an association's from either side or a References attribute's navigation-less one. EF
    /// Core's default for the relationship is never read in place of a modelled behaviour (decision 4).
    /// </summary>
    private static ClaimReading? OnDelete(ClaimContext context, EfCoreModel model)
    {
        List<IReadOnlyList<FluentCall>> chains;
        Span? at;
        if (context.Element is IAssociationEnd association)
        {
            var found = RelationshipChains(context, model, association);
            chains = found.Select(x => x.Chain).ToList();
            at = found.FirstOrDefault().From?.At ?? (context.Owner(association) is { } source ? context.TypeOf(source)?.At : null);
        }
        else if (ReferenceTarget(context) is { } target && (context.Type ?? OwnerType(context)) is { } owner)
        {
            chains = ReferenceChains(context, model, owner, context.MemberOf(context.Element)?.Name ?? context.Element.Name, target);
            at = owner.At;
        }
        else
        {
            return null;
        }

        var deletes = chains.Select(x => x.LastOrDefault(c => c.Name == "OnDelete")).OfType<FluentCall>().ToList();
        if (deletes.Select(Behavior).Distinct(StringComparer.Ordinal).Count() > 1)
        {
            return ClaimReading.CantVerify("ambiguous", $"{deletes.Count} chains configure what deleting it does, differently.", deletes[0].At, deletes.Select(x => x.At).ToList());
        }

        if (deletes.FirstOrDefault() is { } onDelete)
        {
            return ClaimReading.Stated(Behavior(onDelete), onDelete.At);
        }

        var silent = chains.FirstOrDefault()?[0].At ?? at;
        return silent is null ? null : ClaimReading.Absent(silent);

        static string Behavior(FluentCall call) => call.Written.FirstOrDefault()?.Split('.')[^1] ?? "";
    }

    /// <summary>
    /// A relationship with no navigation, as an attribute's References stereotype states it: a chain from either class whose
    /// <c>HasOne&lt;T&gt;()</c> or <c>HasMany&lt;T&gt;()</c> names no navigation, nor does its <c>WithMany()</c> or
    /// <c>WithOne()</c>, and whose <c>HasForeignKey</c> is the attribute. A navigated relationship is an association's.
    /// </summary>
    private static ClaimReading? Reference(ClaimContext context, EfCoreModel model)
    {
        if (ReferenceTarget(context) is not { } target || (context.Type ?? OwnerType(context)) is not { } owner)
        {
            return null;
        }

        var key = context.MemberOf(context.Element)?.Name ?? context.Element.Name;
        var chains = ReferenceChains(context, model, owner, key, target);
        return chains.Count > 0
            ? ClaimReading.Stated("present", chains[0][0].At)
            : ClaimReading.Compared("absent", EntityOf(context, model, owner)?.At ?? owner.At, false,
                $"`{owner.Name}.{context.Element.Name}` references `{target}` in the model, but that relationship isn't in the code: no `HasOne<{target}>().WithMany().HasForeignKey(x => x.{key})` configures it without a navigation. Configure it, or model it as an association if the code navigates it.");
    }

    /// <summary>
    /// Whether an association's source end is navigable. Where the model says so, its navigation is mapped unless the code
    /// ignores it. Where it doesn't, the code has no navigation back that EF Core pairs with the target end's: the chain that
    /// configures the relationship says so (<c>WithMany()</c> names none, <c>WithMany(b =&gt; b.Orders)</c> one), else EF
    /// Core's convention pairs the target class's only property typed as the source class that no other association draws.
    /// </summary>
    private static ClaimReading? Navigable(ClaimContext context, EfCoreModel model)
    {
        if (context.ExpectedValue is not ModelNavigability relationship || context.Member is not { } member)
        {
            return null;
        }

        if (context.Expected == "true")
        {
            var holder = context.TypeOf(relationship.TargetClass);
            if (holder is not null && EntityOf(context, model, holder)?.Calls.LastOrDefault(x => x.Name == "Ignore" && x.Selected.Contains(member.Name)) is { } ignored)
            {
                return ClaimReading.Stated("false", ignored.At);
            }

            return member.Attributes.FirstOrDefault(x => x.Name == "NotMapped") is { } notMapped
                ? ClaimReading.Stated("false", notMapped.At)
                : ClaimReading.Stated("true", member.At);
        }

        if (context.TypeOf(relationship.SourceClass) is not { } owner || context.TypeOf(relationship.TargetClass) is not { } target)
        {
            return ClaimReading.CantVerify("notFound", $"`{relationship.TargetClass.Name}` isn't located, so whether it navigates back to `{relationship.SourceClass.Name}` can't be read.");
        }

        foreach (var (chain, from, _) in RelationshipChains(context, model, relationship.Target))
        {
            if (from == owner && chain[0].Selected is [var named] && named == member.Name)
            {
                // From the source class its With names the navigation back, or says there is none.
                if (chain.FirstOrDefault(x => x.Name is "WithOne" or "WithMany") is { } with)
                {
                    return ClaimReading.Stated(with.Selected.Count > 0 ? "true" : "false", with.At);
                }

                continue;
            }

            // From the target class it starts at the navigation back, or at none.
            return ClaimReading.Stated(chain[0].Selected.Count > 0 ? "true" : "false", chain[0].At);
        }

        var drawn = Navigations.DrawnElsewhere(context, relationship);
        var ignoredNames = EntityOf(context, model, target)?.Calls.Where(x => x.Name == "Ignore").SelectMany(x => x.Selected).ToHashSet(StringComparer.Ordinal) ?? [];
        var candidates = context.Code.MembersOf(target)
            .Where(x => x.Kind == "property" && x != member && !x.IsStatic && Navigations.Holds(x, owner) && !drawn.Contains(x)
                        && !ignoredNames.Contains(x.Name) && x.Attributes.All(a => a.Name != "NotMapped"))
            .ToList();
        return candidates switch
        {
            [] => ClaimReading.Absent(target.At, "false"),
            [var only] => ClaimReading.Defaulted("true", only.At),
            _ => ClaimReading.CantVerify("ambiguous", $"{candidates.Count} properties of `{target.Name}` could navigate back to `{owner.Name}`.", candidates[0].At, candidates.Select(x => x.At).ToList())
        };
    }

    /// <summary>
    /// The column a relationship's foreign key is: the property the relationship's <c>HasForeignKey</c> names, with its
    /// <c>HasColumnName</c> or <c>[Column]</c>. A column EF Core names after the property is a convention, never read in place
    /// of a modelled one (decision 4).
    /// </summary>
    private static ClaimReading? JoinColumn(ClaimContext context, EfCoreModel model)
    {
        if (context.Element is not IAssociationEnd association || context.Member is not { } member)
        {
            return null;
        }

        var chains = RelationshipChains(context, model, association).Select(x => x.Chain).ToList();
        if (chains.Select(x => x.LastOrDefault(c => c.Name == "HasForeignKey")).OfType<FluentCall>().FirstOrDefault(x => x.Selected.Count == 1) is not { } foreignKey)
        {
            return ClaimReading.Absent(chains.FirstOrDefault()?[0].At ?? member.At);
        }

        var name = foreignKey.Selected[0];
        var classes = new[] { context.Owner(association), association.TypeReference?.Element as IElement }.OfType<IElement>().Select(context.TypeOf).OfType<TypeNode>();
        var arguments = new Arguments(context);
        foreach (var type in classes)
        {
            if (EntityOf(context, model, type)?.Properties.GetValueOrDefault(name)?.LastOrDefault(x => x.Name == "HasColumnName") is { } column)
            {
                var value = arguments.Of(column, 0);
                return value is null && arguments.Unread is { } unread ? arguments.Unreadable(unread) : Value(value, column.At);
            }

            if (context.Code.MembersOf(type).FirstOrDefault(x => x.Name == name)?.Attributes.FirstOrDefault(x => x.Name == "Column" && x.Arguments.Count > 0) is { } attribute)
            {
                var value = arguments.Of(attribute, 0);
                return value is null && arguments.Unread is { } unread ? arguments.Unreadable(unread) : Value(value, attribute.At);
            }
        }

        return ClaimReading.Absent(foreignKey.At);
    }
}
