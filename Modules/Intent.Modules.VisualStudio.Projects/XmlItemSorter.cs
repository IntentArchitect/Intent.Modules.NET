using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Intent.Modules.VisualStudio.Projects;

/// <summary>
/// Sorts the child elements of an XML container (e.g. a <c>.csproj</c> <c>ItemGroup</c>)
/// without separating them from the comments a developer wrote about them.
/// </summary>
/// <remarks>
/// Each sortable element is moved as a <em>unit</em> made up of:
/// <list type="bullet">
/// <item>its leading comments: every comment between the previous element and it, and</item>
/// <item>its same-line trailing comments: comments after it with no line break in between.</item>
/// </list>
/// Everything else stays where it is: the whitespace between units, non-sortable elements
/// (with the comments above them), and a comment on its own line after the last element.
/// When whitespace was not preserved on load, "same line" is judged from line info
/// (<see cref="LoadOptions.SetLineInfo"/>); without either, a comment is treated as leading.
/// </remarks>
internal static class XmlItemSorter
{
    /// <summary>
    /// Reorders <paramref name="container"/>'s child elements matched by
    /// <paramref name="isSortable"/> into the order returned by <paramref name="order"/>.
    /// </summary>
    /// <param name="added">
    /// Elements just added to the container. They can't have comments of their own, but whatever
    /// added them may have put them between an existing element and a comment about it, e.g.
    /// MSBuild's <c>AddItem</c>, which inserts next to a sibling. They are first moved after the
    /// last existing unit, indented like it, so the sort places them with no comments attached.
    /// </param>
    /// <returns>
    /// <see langword="false"/> when the container's content is unchanged, i.e. the elements were
    /// already in order and any <paramref name="added"/> ones already sat where they belong.
    /// </returns>
    public static bool SortChildren(
        XElement container,
        Func<XElement, bool> isSortable,
        Func<IEnumerable<XElement>, IEnumerable<XElement>> order,
        IReadOnlyCollection<XElement> added = null)
    {
        if (added is not { Count: > 0 })
        {
            return Reorder(container, isSortable, order);
        }

        var before = container.ToString(SaveOptions.DisableFormatting);
        PlaceAfterExistingUnits(container, isSortable, added);
        Reorder(container, isSortable, order);

        return container.ToString(SaveOptions.DisableFormatting) != before;
    }

    private static void PlaceAfterExistingUnits(XElement container, Func<XElement, bool> isSortable, IReadOnlyCollection<XElement> added)
    {
        var addedHere = container.Elements().Where(added.Contains).ToList();
        if (addedHere.Count == 0 ||
            !container.Elements().Any(x => isSortable(x) && !added.Contains(x)))
        {
            return;
        }

        var preservesWhitespace = container.Nodes().Any(x => x is XText);
        foreach (var element in addedHere)
        {
            if (IsWhitespace(element.PreviousNode))
            {
                element.PreviousNode!.Remove();
            }

            element.Remove();
        }

        var nodes = container.Nodes().ToList();
        var lastUnit = GetUnits(nodes, isSortable, preservesWhitespace).Last();

        // Only the last line of the whitespace before it, so a blank line above the last unit isn't repeated
        var indent = nodes.ElementAtOrDefault(lastUnit.Start - 1) is XText { Value: var whitespace } && string.IsNullOrWhiteSpace(whitespace)
            ? whitespace[Math.Max(whitespace.LastIndexOf('\n'), 0)..]
            : null;

        nodes[lastUnit.End].AddAfterSelf(addedHere.SelectMany(element => indent != null
            ? new XNode[] { new XText(indent), element }
            : new XNode[] { element }));
    }

    private static bool Reorder(
        XElement container,
        Func<XElement, bool> isSortable,
        Func<IEnumerable<XElement>, IEnumerable<XElement>> order)
    {
        var nodes = container.Nodes().ToList();
        var units = GetUnits(nodes, isSortable, preservesWhitespace: nodes.Any(x => x is XText));
        if (units.Count < 2)
        {
            return false;
        }

        var sorted = order(units.Select(x => x.Element)).ToList();
        if (sorted.Count != units.Count || sorted.Except(units.Select(x => x.Element)).Any())
        {
            throw new InvalidOperationException("The order function must return exactly the elements it was given.");
        }

        if (sorted.SequenceEqual(units.Select(x => x.Element)))
        {
            return false;
        }

        var unitsByElement = units.ToDictionary(x => x.Element);
        var unitsByStart = units.ToDictionary(x => x.Start);
        var slot = 0;
        var result = new List<XNode>(nodes.Count);

        for (var index = 0; index < nodes.Count; index++)
        {
            if (!unitsByStart.TryGetValue(index, out var unitInSlot))
            {
                result.Add(nodes[index]);
                continue;
            }

            var unit = unitsByElement[sorted[slot++]];
            result.AddRange(nodes.Skip(unit.Start).Take(unit.End - unit.Start + 1));
            index = unitInSlot.End;
        }

        container.ReplaceNodes(result);
        return true;
    }

    private static List<Unit> GetUnits(List<XNode> nodes, Func<XElement, bool> isSortable, bool preservesWhitespace)
    {
        var units = new List<Unit>();
        var boundary = -1;

        for (var index = 0; index < nodes.Count; index++)
        {
            if (nodes[index] is not XElement element)
            {
                continue;
            }

            if (!isSortable(element))
            {
                boundary = index;
                continue;
            }

            var start = index;
            for (var previous = index - 1; previous > boundary; previous--)
            {
                if (nodes[previous] is XComment)
                {
                    start = previous;
                }
                else if (!IsWhitespace(nodes[previous]))
                {
                    break;
                }
            }

            var end = index;
            while (true)
            {
                if (end + 2 < nodes.Count &&
                    nodes[end + 1] is XText { Value: var gap } &&
                    string.IsNullOrWhiteSpace(gap) &&
                    !gap.Contains('\n') &&
                    nodes[end + 2] is XComment)
                {
                    end += 2;
                    continue;
                }

                if (end + 1 < nodes.Count &&
                    nodes[end + 1] is XComment adjacentComment &&
                    (preservesWhitespace || IsOnLastLineOf(adjacentComment, element)))
                {
                    end += 1;
                    continue;
                }

                break;
            }

            units.Add(new Unit(element, start, end));
            boundary = end;
            index = end;
        }

        return units;
    }

    private static bool IsWhitespace(XNode node) => node is XText text && string.IsNullOrWhiteSpace(text.Value);

    /// <summary>
    /// Best effort for when whitespace wasn't preserved: line info only gives where an element
    /// starts, so its closing tag is assumed to be on the line after its last child element.
    /// </summary>
    private static bool IsOnLastLineOf(XComment comment, XElement element)
    {
        IXmlLineInfo commentLine = comment;
        IXmlLineInfo elementLine = element;
        if (!commentLine.HasLineInfo() || !elementLine.HasLineInfo())
        {
            return false;
        }

        IXmlLineInfo lastChild = element.Elements().LastOrDefault();
        var lastLine = lastChild?.HasLineInfo() == true
            ? lastChild.LineNumber + 1
            : elementLine.LineNumber;

        return commentLine.LineNumber == lastLine;
    }

    private record Unit(XElement Element, int Start, int End);
}
