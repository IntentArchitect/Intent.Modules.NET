using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests;

public class XmlItemSorterTests
{
    [Fact]
    public void CommentAboveElementMovesWithIt()
    {
        var result = Sort(
            """
            <ItemGroup>
            <PackageReference Include="C" />
            <!-- About A -->
            <PackageReference Include="A" />
            <PackageReference Include="B" />
            </ItemGroup>
            """, out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
            <!-- About A -->
            <PackageReference Include="A" />
            <PackageReference Include="B" />
            <PackageReference Include="C" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void MultipleAndMultiLineCommentsMoveTogether()
    {
        var result = Sort(
            """
            <ItemGroup>
            <PackageReference Include="B" />
            <!-- First line
            second line -->
            <!-- Another -->
            <PackageReference Include="A" />
            </ItemGroup>
            """, out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
            <!-- First line
            second line -->
            <!-- Another -->
            <PackageReference Include="A" />
            <PackageReference Include="B" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void SameLineTrailingCommentStaysOnItsLine()
    {
        var result = Sort(
            """
            <ItemGroup>
            <PackageReference Include="B" /> <!-- about B -->
            <PackageReference Include="A" />
            </ItemGroup>
            """, out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
            <PackageReference Include="A" />
            <PackageReference Include="B" /> <!-- about B -->
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void TrailingCommentOnItsOwnLineAfterLastElementStaysLast()
    {
        var result = Sort(
            """
            <ItemGroup>
            <PackageReference Include="B" />
            <PackageReference Include="A" />
            <!-- end of group -->
            </ItemGroup>
            """, out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
            <PackageReference Include="A" />
            <PackageReference Include="B" />
            <!-- end of group -->
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void AlreadySortedReturnsFalseAndLeavesTextIdentical()
    {
        const string input =
            """
            <ItemGroup Label="Packages">
            <PackageReference Include="A" />
            <!-- About B -->
            <PackageReference Include="B" /> <!-- trailing -->

            <PackageReference Include="C" />
            </ItemGroup>
            """;

        var result = Sort(input, out var changed);

        changed.ShouldBeFalse();
        result.ShouldBe(input.ReplaceLineEndings());
    }

    [Fact]
    public void ContainerAttributesAndSpacingAreKept()
    {
        var result = Sort(
            """
            <ItemGroup Condition="'$(X)' == 'true'" Label="Packages">
            <PackageReference Include="B" />

            <PackageReference Include="A" />
            </ItemGroup>
            """, out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup Condition="'$(X)' == 'true'" Label="Packages">
            <PackageReference Include="A" />

            <PackageReference Include="B" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void NonSortableElementsKeepTheirPosition()
    {
        var result = Sort(
            """
            <ItemGroup>
            <PackageReference Include="C" />
            <!-- About the None -->
            <None Include="x.txt" />
            <PackageReference Include="B" />
            <!-- About A -->
            <PackageReference Include="A" />
            </ItemGroup>
            """, out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
            <!-- About A -->
            <PackageReference Include="A" />
            <!-- About the None -->
            <None Include="x.txt" />
            <PackageReference Include="B" />
            <PackageReference Include="C" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void LeadingCommentMovesWhenWhitespaceIsNotPreserved()
    {
        var container = XElement.Parse(
            """
            <ItemGroup>
            <PackageReference Include="B" />
            <!-- About A -->
            <PackageReference Include="A" />
            </ItemGroup>
            """, LoadOptions.SetLineInfo);

        var changed = XmlItemSorter.SortChildren(container, IsPackageReference, x => x.OrderBy(GetInclude));

        changed.ShouldBeTrue();
        container.Nodes().Select(Describe).ShouldBe(["A: About A", "A", "B"]);
    }

    [Fact]
    public void SameLineCommentMovesWithItsElementWhenWhitespaceIsNotPreserved()
    {
        var container = XElement.Parse(
            """
            <ItemGroup>
            <PackageReference Include="B" /> <!-- About B -->
            <PackageReference Include="A" />
            </ItemGroup>
            """, LoadOptions.SetLineInfo);

        var changed = XmlItemSorter.SortChildren(container, IsPackageReference, x => x.OrderBy(GetInclude));

        changed.ShouldBeTrue();
        container.Nodes().Select(Describe).ShouldBe(["A", "B", "B: About B"]);
    }

    [Fact]
    public void SameLineCommentAfterMultiLineElementMovesWithItWhenWhitespaceIsNotPreserved()
    {
        var container = XElement.Parse(
            """
            <ItemGroup>
            <PackageReference Include="B">
            <Version>1.0.0</Version>
            </PackageReference> <!-- About B -->
            <PackageReference Include="A">
            <Version>1.0.0</Version>
            </PackageReference>
            </ItemGroup>
            """, LoadOptions.SetLineInfo);

        var changed = XmlItemSorter.SortChildren(container, IsPackageReference, x => x.OrderBy(GetInclude));

        changed.ShouldBeTrue();
        container.Nodes().Select(Describe).ShouldBe(["A", "B", "B: About B"]);
    }

    [Fact]
    public void FewerThanTwoSortableElementsIsANoOp()
    {
        const string input =
            """
            <ItemGroup>
            <!-- Only one -->
            <PackageReference Include="A" />
            </ItemGroup>
            """;

        var result = Sort(input, out var changed);

        changed.ShouldBeFalse();
        result.ShouldBe(input.ReplaceLineEndings());
    }

    [Fact]
    public void AddedElementsInsertedBeforeASameLineCommentDoNotTakeItOver()
    {
        // As MSBuild's AddItem does: each new item goes straight after the same sibling
        var result = SortWithAdded(
            """
            <ItemGroup>
              <PackageReference Include="A" />
              <PackageReference Include="C" /> <!-- about C -->
            </ItemGroup>
            """,
            container => InsertAfter(container, "C", "E", "D"),
            x => x.OrderBy(GetInclude),
            out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
              <PackageReference Include="A" />
              <PackageReference Include="C" /> <!-- about C -->
              <PackageReference Include="D" />
              <PackageReference Include="E" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void AddedElementInsertedBelowALeadingCommentDoesNotTakeItOver()
    {
        var result = SortWithAdded(
            """
            <ItemGroup>
              <!-- about B -->
              <PackageReference Include="B" />
            </ItemGroup>
            """,
            container => InsertBefore(container, "B", "A"),
            x => x.OrderBy(GetInclude),
            out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
              <PackageReference Include="A" />
              <!-- about B -->
              <PackageReference Include="B" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void AddedElementAlreadyWhereItBelongsIsNotAChange()
    {
        const string expected =
            """
            <ItemGroup>
              <PackageReference Include="A" />
              <PackageReference Include="B" />
              <PackageReference Include="C" />
            </ItemGroup>
            """;

        var result = SortWithAdded(
            """
            <ItemGroup>
              <PackageReference Include="A" />
              <PackageReference Include="C" />
            </ItemGroup>
            """,
            container => InsertAfter(container, "A", "B"),
            x => x.OrderBy(GetInclude),
            out var changed);

        changed.ShouldBeFalse();
        result.ShouldBe(expected.ReplaceLineEndings());
    }

    [Fact]
    public void AddedElementWithoutSortingGoesAfterTheLastSortableUnit()
    {
        var result = SortWithAdded(
            """
            <ItemGroup>
              <PackageReference Include="B" /> <!-- about B -->
              <None Include="readme.md" />
            </ItemGroup>
            """,
            container => InsertAfter(container, "B", "A"),
            x => x,
            out var changed);

        changed.ShouldBeTrue();
        result.ShouldBe(
            """
            <ItemGroup>
              <PackageReference Include="B" /> <!-- about B -->
              <PackageReference Include="A" />
              <None Include="readme.md" />
            </ItemGroup>
            """.ReplaceLineEndings());
    }

    [Fact]
    public void AddedElementsAreLeftWhereTheyAreWhenThereIsNothingToPlaceThemAfter()
    {
        const string input =
            """
            <ItemGroup>
              <PackageReference Include="B" />
              <PackageReference Include="A" />
            </ItemGroup>
            """;

        var container = XElement.Parse(input.ReplaceLineEndings("\n"), LoadOptions.PreserveWhitespace);
        var changed = XmlItemSorter.SortChildren(container, IsPackageReference, x => x.OrderBy(GetInclude), container.Elements().ToArray());

        changed.ShouldBeTrue();
        container.Elements().Select(GetInclude).ShouldBe(["A", "B"]);
    }

    private static string SortWithAdded(
        string xml,
        Func<XElement, IReadOnlyCollection<XElement>> add,
        Func<IEnumerable<XElement>, IEnumerable<XElement>> order,
        out bool changed)
    {
        var container = XElement.Parse(xml.ReplaceLineEndings("\n"), LoadOptions.PreserveWhitespace);
        var added = add(container);
        changed = XmlItemSorter.SortChildren(container, IsPackageReference, order, added);
        return container.ToString(SaveOptions.DisableFormatting).ReplaceLineEndings();
    }

    // Each new element is put directly after the sibling, preceded by a line break and its indent
    private static IReadOnlyCollection<XElement> InsertAfter(XElement container, string siblingInclude, params string[] includes)
    {
        var sibling = container.Elements().Single(x => GetInclude(x) == siblingInclude);
        var added = includes.Select(NewPackageReference).ToArray();
        foreach (var element in added)
        {
            sibling.AddAfterSelf(new XText("\n  "), element);
        }

        return added;
    }

    private static IReadOnlyCollection<XElement> InsertBefore(XElement container, string siblingInclude, string include)
    {
        var sibling = container.Elements().Single(x => GetInclude(x) == siblingInclude);
        var added = NewPackageReference(include);
        sibling.AddBeforeSelf(added, new XText("\n  "));
        return [added];
    }

    private static XElement NewPackageReference(string include) => new("PackageReference", new XAttribute("Include", include));

    private static string Sort(string xml, out bool changed)
    {
        var container = XElement.Parse(xml.ReplaceLineEndings("\n"), LoadOptions.PreserveWhitespace);
        changed = XmlItemSorter.SortChildren(container, IsPackageReference, x => x.OrderBy(GetInclude));
        return container.ToString(SaveOptions.DisableFormatting).ReplaceLineEndings();
    }

    private static bool IsPackageReference(XElement element) => element.Name.LocalName == "PackageReference";

    private static string GetInclude(XElement element) => element.Attribute("Include")?.Value;

    // Comments are described against the next element, or the previous one when last,
    // so a test can assert which element each comment ended up beside.
    private static string Describe(XNode node)
    {
        if (node is XElement element)
        {
            return GetInclude(element) ?? element.Name.LocalName;
        }

        var comment = (XComment)node;
        var owner = comment.ElementsAfterSelf().FirstOrDefault() is { } next && comment.NodesAfterSelf().First() == next
            ? next
            : comment.ElementsBeforeSelf().Last();
        return $"{GetInclude(owner)}: {comment.Value.Trim()}";
    }
}
