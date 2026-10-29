using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Drift
{
    /// <summary>A placeholder a kind of fact's recipes can use, and what it names, as the configure-drift-check skill documents it.</summary>
    public sealed record FactPlaceholder(string Name, string Description);

    /// <summary>
    /// A kind of fact a designer's model states: what it is called in a recipe file, how the report labels it, the
    /// placeholders its recipes can use, and the model's vocabulary for its value. The module that contributes the
    /// designer (<see cref="DriftDesigner"/>) defines its kinds; the drift module knows none of them.
    /// </summary>
    public sealed class FactKind
    {
        /// <param name="key">The kind as a recipe file names it, e.g. <c>column.maxLength</c>. A key starting <c>intent.</c> is a modelled interaction.</param>
        /// <param name="designer">The designer's name (<see cref="DriftDesigner.Name"/>).</param>
        /// <param name="label">How a report row names the fact, after its element, e.g. <c>max length</c>.</param>
        /// <param name="placeholders">The placeholders the kind's recipes can use, in order, each with what it names.</param>
        /// <param name="values">The model's vocabulary for the fact's value, as the configure-drift-check skill lists it.</param>
        /// <param name="source">Where in the model the fact comes from, as the configure-drift-check skill lists it.</param>
        /// <param name="own">
        /// The placeholders that name the element the fact is about, as opposed to what it belongs to: a column fact's
        /// attribute rather than its class, an endpoint fact's operation rather than its Service, a relationship's target
        /// and ends rather than its source, an intent fact's target rather than the handler or member it is modelled on.
        /// The kind's other placeholders name the element's owner.
        /// </param>
        public FactKind(string key, string designer, string label, IReadOnlyList<FactPlaceholder> placeholders, string values, string source, IReadOnlyList<string> own)
        {
            Key = key;
            Designer = designer;
            Label = label;
            PlaceholderDescriptions = placeholders;
            Placeholders = placeholders.Select(x => x.Name).ToList();
            Values = values;
            Source = source;
            Own = own;
            if (own.Count == 0 || own.Any(x => !Placeholders.Contains(x)))
            {
                throw new ArgumentException($"The {key} fact's own placeholders ({string.Join(", ", own)}) must be some of its placeholders ({string.Join(", ", Placeholders)}).", nameof(own));
            }
        }

        public string Key { get; }
        public string Designer { get; }
        public string Label { get; }

        /// <summary>The placeholders' names, in order.</summary>
        public IReadOnlyList<string> Placeholders { get; }

        /// <summary>The placeholders, in order, each with what it names.</summary>
        public IReadOnlyList<FactPlaceholder> PlaceholderDescriptions { get; }

        public string Values { get; }
        public string Source { get; }

        /// <summary>The placeholders that name the fact's own element; the others name its owner.</summary>
        public IReadOnlyList<string> Own { get; }
    }

    /// <summary>One fact the model states about one element.</summary>
    public sealed class ModelFact
    {
        public FactKind Kind { get; init; }

        /// <summary>The element as the report names it, e.g. <c>sapPlants.differential</c>.</summary>
        public string Element { get; init; }

        /// <summary>
        /// The ids of the model element that states the fact and of the elements it belongs to, outermost first (a
        /// class, then its attribute), for the report's links to them.
        /// </summary>
        public IReadOnlyList<string> ElementIds { get; init; } = Array.Empty<string>();

        public string Value { get; init; }

        public IReadOnlyDictionary<string, string> Placeholders { get; init; }

        /// <summary>The operation's parameter names in model order, used to choose between overloads.</summary>
        public IReadOnlyList<string> ParameterNames { get; init; }
    }
}
