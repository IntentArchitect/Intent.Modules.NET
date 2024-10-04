using System;
using Intent.Modules.Common.CSharp.RazorBuilder;

namespace Intent.Modules.Blazor.Api;

public static class RazorFileNodeExtensions
{
    public static IRazorFileNode AddHtmlElement(this IRazorFileNode node, string name, Action<IHtmlElement>? configure = null)
    {
        var htmlElement = new HtmlElement(name, (IRazorFile)node.File);
        node.AddChildNode(htmlElement);
        configure?.Invoke(htmlElement);
        return htmlElement;
    }

}