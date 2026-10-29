using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.Drift
{
    /// <summary>
    /// What a module supplies for one designer's drift check. The drift module knows no designer: the module that owns a
    /// designer's contract templates contributes one of these from its drift report template (<see cref="IDriftDesignerSource"/>),
    /// and the drift module's rule and skills cover every designer contributed, in <see cref="Order"/>. One contribution
    /// per designer: a designer has one recipe file and one catalog of facts.
    /// </summary>
    /// <param name="Name">
    /// The designer as the drift check names it, e.g. <c>Domain</c>: its recipe file is <c>guardrails/domain.drift.json</c>,
    /// its report <c>DomainDriftReport.mdx</c>.
    /// </param>
    /// <param name="DesignerId">The designer's id, to read its model.</param>
    /// <param name="Order">Where the rule and skills list the designer, lowest first.</param>
    /// <param name="Kinds">Every kind of fact the designer's model states.</param>
    /// <param name="ReadFacts">Reads every fact the designer's model states, for the application with the given id.</param>
    /// <param name="ReadElements">The application's elements in the designer by id, for the report's links and the run's error.</param>
    /// <param name="ValidateTaskId">The id of the module task that validates the designer's recipes (<see cref="RecipeValidationTask"/>).</param>
    /// <param name="Guidance">The designer's text in the rule and skills.</param>
    public sealed record DriftDesigner(
        string Name,
        string DesignerId,
        int Order,
        IReadOnlyList<FactKind> Kinds,
        Func<IDesigner, string, IReadOnlyList<ModelFact>> ReadFacts,
        Func<IDesigner, string, IReadOnlyDictionary<string, IElement>> ReadElements,
        string ValidateTaskId,
        DriftGuidance Guidance);

    /// <summary>A designer's text in the drift module's rule and skills, in Markdown.</summary>
    /// <param name="FactsText">What the designer's facts are about, as one phrase, e.g. <c>database tables, keys, columns and relationships</c>.</param>
    /// <param name="RuleRows">The rule's table rows for the designer (what code changes need which model change), each ending in a line break, then a blank line.</param>
    /// <param name="GuideRows">The configure-drift-check skill's index rows for the designer's technology guides, which its own module writes beside the skill, then a blank line.</param>
    /// <param name="IntentLinks">The designer's interaction associations, what they start from and what they link to.</param>
    /// <param name="NewIntent">An interaction the code can gain, which the check never reports.</param>
    /// <param name="MovedIntent">
    /// What the intent drift reference adds, with a leading space, for work that moved to another element; <see langword="null"/> for nothing.
    /// </param>
    public sealed record DriftGuidance(string FactsText, string RuleRows, string GuideRows, string IntentLinks, string NewIntent, string MovedIntent = null);

    /// <summary>
    /// A template that contributes a designer to the drift check: the drift module's rule and skills find every template
    /// instance of the run that implements it, whether or not it writes a file.
    /// </summary>
    public interface IDriftDesignerSource
    {
        DriftDesigner Designer { get; }
    }

    /// <summary>
    /// A template whose generated C# file the drift check compares with the code on disk, for the designer it names
    /// (<see cref="DriftDesigner.Name"/>).
    /// </summary>
    public interface IDriftStructureSource : ICSharpFileBuilderTemplate
    {
        string Designer { get; }
    }
}
