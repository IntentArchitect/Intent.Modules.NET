using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftEngine.Extensions;

namespace Intent.Modules.Common.DriftEngine;

/// <summary>
/// Coverage of the model (vocabulary.md, R6, R7, R32): nothing modelled is skipped silently. Each designer's element types,
/// applied stereotypes, stereotype properties set away from their defaults and association types are read by some pack's
/// vocabulary, some extension's code (its claims, or what it declares it reads), or declared non-checkable with a reason.
/// Anything else is an <c>engine/noVocabulary</c> error, once per element and feature.
/// </summary>
public sealed partial class DriftEngine
{
    private const string NoVocabulary = "engine/noVocabulary";

    // Expressions quote with ', which the default encoder writes as '.
    private static readonly System.Text.Json.JsonSerializerOptions Unescaped = new() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private static readonly Regex StereotypeExpression = new(@"(?<fn>hasStereotype|stereotype|enclosing)\('(?<name>[^']+)'\)(?:\.(?<property>[A-Za-z_]\w*)|\['(?<quoted>[^']+)'\])?", RegexOptions.Compiled);

    /// <summary>An expression reading an element's children of a type, such as an index's columns.</summary>
    private static readonly Regex ChildrenExpression = new(@"children\('(?<type>[^']+)'\)", RegexOptions.Compiled);

    /// <summary>An expression reading an element's links of a type, such as its Generalization.</summary>
    private static readonly Regex AssociationsExpression = new(@"associations\('(?<type>[^']+)'\)", RegexOptions.Compiled);

    /// <summary>Kinds that read an element's children of a type, besides the element they are claimed on.</summary>
    private static readonly IReadOnlyDictionary<string, string[]> ChildReads = new Dictionary<string, string[]>(StringComparer.Ordinal)
    {
        ["operation.parameters"] = ["Parameter"]
    };

    /// <summary>The engine's own configuration on the Codebase Structure designer, which it reads itself.</summary>
    private static readonly string[] EngineStereotypes = ["Drift Scope", "Drift Check"];

    /// <summary>What a designer's features are read through, or declared non-checkable by.</summary>
    private sealed class DesignerReads
    {
        public HashSet<string> Elements { get; } = new(StringComparer.Ordinal);

        public HashSet<string> Associations { get; } = new(StringComparer.Ordinal);

        /// <summary>Each stereotype read, with the properties read by name; null when all of them are.</summary>
        public Dictionary<string, HashSet<string>?> Stereotypes { get; } = new(StringComparer.Ordinal);

        public bool Any => Elements.Count > 0 || Associations.Count > 0 || Stereotypes.Count > 0;

        public void Stereotype(string name, string? property)
        {
            if (Stereotypes.TryGetValue(name, out var properties))
            {
                if (properties is not null && property is not null)
                {
                    properties.Add(property);
                }
                else if (property is null)
                {
                    Stereotypes[name] = null;
                }

                return;
            }

            Stereotypes[name] = property is null ? null : new HashSet<string>(StringComparer.Ordinal) { property };
        }
    }

    /// <summary>A non-claim in use: the feature, its reason and who declared it, with how many elements it covers.</summary>
    private sealed record NonClaim(string Designer, string Feature, string Reason, string DeclaredBy)
    {
        public int Uses { get; set; }
    }

    private IEnumerable<Finding> Unread()
    {
        var findings = new List<Finding>();
        var nonClaims = NonClaims();
        foreach (var designer in _input.Designers.Keys)
        {
            var elements = Elements(designer);
            var packages = Packages(designer);
            var reads = ReadsOf(designer);
            var designerNonClaim = nonClaims.FirstOrDefault(x => x.Designer == designer && x.Feature == "");
            if (designerNonClaim is not null)
            {
                designerNonClaim.Uses += elements.Count;
                continue;
            }

            if (!reads.Any)
            {
                // A designer no pack or extension reads at all is one finding, not one per element (R7).
                if (elements.Count > 0)
                {
                    findings.Add(new Finding(NoVocabulary, $"{designer} (DriftEngine)", "ModelCode", "error",
                        $"The {designer} designer has {elements.Count} elements, and no installed pack or extension says what any of them states in code, so none of it is checked. Install the module whose extension reads the {designer} designer, or declare it non-checkable with a reason.",
                        new Anchor(Text: designer)));
                }

                continue;
            }

            NonClaim? Covered(string feature) => nonClaims.FirstOrDefault(x => x.Designer == designer && x.Feature == feature);

            foreach (var package in packages)
            {
                findings.AddRange(UnreadStereotypes(designer, package, $"Package `{package.Name}`", new Anchor(Text: $"{designer} package {package.Name}"), reads, Covered));
            }

            foreach (var element in elements)
            {
                if (!reads.Elements.Contains(element.SpecializationType))
                {
                    if (Covered($"element:{element.SpecializationType}") is { } nonClaim)
                    {
                        nonClaim.Uses++;
                    }
                    else
                    {
                        findings.Add(new Finding(NoVocabulary, $"{designer} (DriftEngine)", "ModelCode", "error",
                            $"`{element.Name}` is a {element.SpecializationType}, and no pack says what a {element.SpecializationType} states in code, so it isn't checked. Add vocabulary for it, or declare it non-checkable with a reason.",
                            new Anchor(Element: element.Id)));
                    }

                    // An element nothing reads isn't reported again for its stereotypes and links.
                    continue;
                }

                findings.AddRange(UnreadStereotypes(designer, element, $"`{element.Name}`", new Anchor(Element: element.Id), reads, Covered));
                foreach (var end in Ends(element).Where(IsTargetEnd).DistinctBy(x => x.Id))
                {
                    var association = end.Association!.SpecializationType;
                    if (reads.Associations.Contains(association))
                    {
                        // A read link's ends carry stereotypes of their own, such as an index on a navigation.
                        foreach (var side in new[] { end.Association.SourceEnd, end }.OfType<IAssociationEnd>())
                        {
                            findings.AddRange(UnreadStereotypes(designer, side, $"`{element.Name}`'s {association} to `{end.TypeReference?.Element?.Name ?? end.Name}`", new Anchor(Element: end.Id), reads, Covered));
                        }

                        continue;
                    }

                    if (Covered($"association:{association}") is { } nonClaim)
                    {
                        nonClaim.Uses++;
                        continue;
                    }

                    findings.Add(new Finding(NoVocabulary, $"{designer} (DriftEngine)", "ModelCode", "error",
                        $"`{element.Name}`'s {association} to `{end.TypeReference?.Element?.Name ?? end.Name}`: no pack says what a {association} states in code, so it isn't checked. Add vocabulary for it, or declare it non-checkable with a reason.",
                        new Anchor(Element: end.Id)));
                }
            }
        }

        // Each non-claim in use is listed in the Software Factory log, so a reviewer sees what isn't checked and why (R32).
        findings.AddRange(nonClaims.Where(x => x.Uses > 0).Select(x => new Finding("scope/coverage", ScopeArea, "ModelCode", "info",
            $"Not checked: the {x.Designer} designer's {Describe(x.Feature)} ({x.Uses} in the model). {x.Reason} (declared by {x.DeclaredBy})",
            new Anchor(Text: x.Designer))));
        return findings;
    }

    private static string Describe(string feature) => feature switch
    {
        "" => "content",
        _ when feature.StartsWith("element:", StringComparison.Ordinal) => $"{feature["element:".Length..]} elements",
        _ when feature.StartsWith("association:", StringComparison.Ordinal) => $"{feature["association:".Length..]} links",
        _ when feature.StartsWith("stereotype:", StringComparison.Ordinal) => $"{feature["stereotype:".Length..].Replace("/", " stereotype's ")}{(feature.Contains('/') ? " property" : " stereotype")}",
        _ => feature
    };

    private IEnumerable<Finding> UnreadStereotypes(string designer, IHasStereotypes owner, string display, Anchor subject, DesignerReads reads,
        Func<string, NonClaim?> covered)
    {
        foreach (var stereotype in owner.Stereotypes ?? [])
        {
            if (covered($"stereotype:{stereotype.Name}") is { } whole)
            {
                whole.Uses++;
                continue;
            }

            if (!reads.Stereotypes.TryGetValue(stereotype.Name, out var properties))
            {
                yield return new Finding(NoVocabulary, $"{designer} (DriftEngine)", "ModelCode", "error",
                    $"{display}'s `{stereotype.Name}` stereotype: no pack says what it states in code, so it isn't checked. Add vocabulary for it, or declare it non-checkable with a reason.",
                    subject);
                continue;
            }

            if (properties is null)
            {
                continue;
            }

            foreach (var (name, value) in SetProperties(stereotype))
            {
                if (properties.Contains(name))
                {
                    continue;
                }

                if (covered($"stereotype:{stereotype.Name}/{name}") is { } nonClaim)
                {
                    nonClaim.Uses++;
                    continue;
                }

                yield return new Finding(NoVocabulary, $"{designer} (DriftEngine)", "ModelCode", "error",
                    $"{display}'s `{stereotype.Name}` {name} is `{value}`, and no pack says what it states in code, so it isn't checked. Add vocabulary for it, or declare it non-checkable with a reason.",
                    subject);
            }
        }
    }

    /// <summary>A stereotype's properties set away from their defaults, by name: a checkbox left unticked or a box left empty isn't one.</summary>
    private static IEnumerable<(string Name, string Value)> SetProperties(IStereotype stereotype)
    {
        var definitions = stereotype.Definition?.Properties ?? [];
        foreach (var property in stereotype.Properties ?? [])
        {
            var definition = definitions.FirstOrDefault(x => x.Id == property.PropertyDefinitionId);
            var name = definition?.Name ?? property.Key;
            var value = property.Value?.Trim() ?? "";
            var fallback = definition?.DefaultValue?.Trim() ?? "";
            if (value.Length == 0 || value == fallback || fallback.Length == 0 && value == "false")
            {
                continue;
            }

            yield return (name, value);
        }
    }

    /// <summary>What reads a designer: the packs' vocabulary, the claims made on its elements, and what extensions declare.</summary>
    private DesignerReads ReadsOf(string designer)
    {
        var reads = new DesignerReads();
        foreach (var pack in _packs)
        {
            var vocabulary = pack.Data["vocabulary"]?[designer];
            foreach (var entry in pack.Vocabulary(designer))
            {
                if (entry["for"]?["element"]?.GetValue<string>() is { } element)
                {
                    reads.Elements.Add(element);
                }

                if (entry["for"]?["associationEnd"]?.GetValue<string>() is { } association)
                {
                    reads.Associations.Add(association);
                }

                if (entry["for"]?["stereotype"]?.GetValue<string>() is { } stereotype)
                {
                    reads.Stereotype(stereotype, null);
                }

                foreach (var child in ChildReads.GetValueOrDefault(entry["kind"]?.GetValue<string>() ?? "") ?? [])
                {
                    reads.Elements.Add(child);
                }
            }

            // The stereotypes and properties its entries' expressions name, and its conventions' and rules' for the designer.
            var text = string.Join("\n", new[] { vocabulary, pack.Data["conventions"]?[designer], pack.Data["rules"]?[designer] }.OfType<JsonNode>().Select(x => x.ToJsonString(Unescaped)));
            foreach (Match match in StereotypeExpression.Matches(text))
            {
                var property = match.Groups["property"].Success ? match.Groups["property"].Value : match.Groups["quoted"].Success ? match.Groups["quoted"].Value : null;
                reads.Stereotype(match.Groups["name"].Value, match.Groups["fn"].Value == "hasStereotype" ? "" : property);
            }

            foreach (Match match in ChildrenExpression.Matches(text))
            {
                reads.Elements.Add(match.Groups["type"].Value);
            }

            foreach (Match match in AssociationsExpression.Matches(text))
            {
                reads.Associations.Add(match.Groups["type"].Value);
            }
        }

        foreach (var claim in _claims.Where(x => x.Designer == designer))
        {
            if (claim.Source is IAssociationEnd end)
            {
                if (end.Association?.SpecializationType is { } association)
                {
                    reads.Associations.Add(association);
                }
            }
            else
            {
                reads.Elements.Add(claim.Source.SpecializationType);
            }

            foreach (var child in ChildReads.GetValueOrDefault(claim.Kind) ?? [])
            {
                reads.Elements.Add(child);
            }
        }

        foreach (var read in _extensions.SelectMany(x => x.Reads).Where(x => x.Designer == designer))
        {
            if (read.Element is not null)
            {
                reads.Elements.Add(read.Element);
            }

            if (read.Association is not null)
            {
                reads.Associations.Add(read.Association);
            }

            if (read.Stereotype is not null)
            {
                reads.Stereotype(read.Stereotype, read.Property);
            }
        }

        // A File Config's namespace and file locate a type among several of its name (DriftEngine.TypeExists), in a designer
        // something reads.
        if (reads.Any)
        {
            reads.Stereotype("File Config", "Namespace");
            reads.Stereotype("File Config", "Filename");
        }

        if (designer == CodebaseStructure)
        {
            foreach (var stereotype in EngineStereotypes)
            {
                reads.Stereotype(stereotype, null);
            }
        }

        // hasStereotype reads presence alone, recorded as the empty property; it reads none of the stereotype's values.
        return reads;
    }

    /// <summary>Every pack's and extension's non-claims, keyed by designer and feature (<c>""</c> for the whole designer).</summary>
    private List<NonClaim> NonClaims()
    {
        var nonClaims = new List<NonClaim>();
        foreach (var pack in _packs)
        {
            foreach (var (designer, vocabulary) in pack.Data["vocabulary"] as JsonObject ?? [])
            {
                foreach (var item in (vocabulary?["nonClaims"] as JsonArray ?? []).OfType<JsonNode>())
                {
                    nonClaims.Add(new NonClaim(item["designer"]?.GetValue<string>() ?? designer, Feature(item["for"]), item["reason"]?.GetValue<string>() ?? "", pack.Id));
                }
            }
        }

        foreach (var extension in _extensions)
        {
            foreach (var feature in extension.NonClaims)
            {
                var key = feature.Element is not null ? $"element:{feature.Element}"
                    : feature.Association is not null ? $"association:{feature.Association}"
                    : feature.Stereotype is not null ? $"stereotype:{feature.Stereotype}{(feature.Property is null ? "" : $"/{feature.Property}")}"
                    : "";
                nonClaims.Add(new NonClaim(feature.Designer, key, feature.Reason ?? "", extension.Id));
            }
        }

        return nonClaims;

        static string Feature(JsonNode? selector) => selector switch
        {
            null => "",
            _ when selector["element"]?.GetValue<string>() is { } element => $"element:{element}",
            _ when selector["association"]?.GetValue<string>() is { } association => $"association:{association}",
            _ when selector["stereotype"]?.GetValue<string>() is { } stereotype =>
                $"stereotype:{stereotype}{(selector["property"]?.GetValue<string>() is { } property ? $"/{property}" : "")}",
            _ => ""
        };
    }
}
