using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Api;

public static class BlazorFileExtensions
{
    public static RazorFile AddPageDirective(this RazorFile razorFile, string route)
    {
        razorFile.Directives.Insert(0, new RazorDirective("page", new CSharpStatement($"\"{route}\"")));
        return razorFile;
    }

    public static RazorFile AddInheritsDirective(this RazorFile razorFile, string baseClassName)
    {
        razorFile.Directives.Add(new RazorDirective("inherits", new CSharpStatement(baseClassName)));
        return razorFile;
    }

    public static RazorFile AddInjectDirective(this RazorFile razorFile, string fullyQualifiedTypeName, string propertyName = null)
    {
        var serviceDeclaration = $"{razorFile.Template.UseType(fullyQualifiedTypeName)} {propertyName ?? razorFile.Template.UseType(fullyQualifiedTypeName)}";
        if (!razorFile.Directives.Any(x => x.Keyword == "inject" && x.Expression.ToString() == serviceDeclaration))
        {
            razorFile.Directives.Add(new RazorDirective("inject", new CSharpStatement(serviceDeclaration)));
        }
        return razorFile;
    }

    public static RazorFile AddCodeBlock(this RazorFile razorFile, Action<RazorCodeBlock> configure = null)
    {
        var razorCodeBlock = new RazorCodeBlock(razorFile);
        razorFile.ChildNodes.Add(razorCodeBlock);
        configure?.Invoke(razorCodeBlock);
        return razorFile;
    }
}

public static class RazorFileExtensions
{
    public static HtmlElement SelectHtmlElement(this RazorFile razorFile, string selector)
    {
        return razorFile.SelectHtmlElements(selector).SingleOrDefault();
    }

    public static IEnumerable<HtmlElement> SelectHtmlElements(this RazorFile razorFile, string selector)
    {
        return razorFile.ChildNodes.OfType<HtmlElement>().SelectHtmlElements(selector.Split("/", StringSplitOptions.RemoveEmptyEntries));
    }

    public static IEnumerable<HtmlElement> SelectHtmlElements(this IEnumerable<HtmlElement> nodes, string[] parts)
    {
        var firstPart = parts.FirstOrDefault();
        var foundNodes = nodes.Where(x => x.Name == firstPart).ToList();
        foreach (var found in foundNodes)
        {
            if (parts.Length == 1)
            {
                yield return found;
            }

            foreach (var foundChildren in found.ChildNodes.OfType<HtmlElement>().SelectHtmlElements(parts.Skip(1).ToArray()))
            {
                yield return foundChildren;
            }
        }
    }
}