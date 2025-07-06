using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Blazor.Api.ButtonModelStereotypeExtensions;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

namespace Intent.Modules.Blazor.Api;
public static class SecuredHelper
{
    public static string AuthorizationAttribute(this Intent.Blazor.Api.ComponentModelStereotypeExtensions.Secured model, ICSharpTemplate template)
    {
        var authText = GetSecuredText(model.Policy(), model.Roles());

        if (string.IsNullOrWhiteSpace(authText))
        {
            return $"[{template.UseType("Microsoft.AspNetCore.Authorization.Authorize")}]";
        }

        return $"[{template.UseType("Microsoft.AspNetCore.Authorization.Authorize")}({authText})]";
    }

    public static IHtmlElement AuthorizeComponent(this IHtmlElement htmlElement, IElement element, IRazorComponentTemplate template)
    {
        // no secured stereotype
        if (!element.HasStereotype(Intent.Blazor.Api.ComponentModelStereotypeExtensions.Secured.DefinitionId))
        {
            return htmlElement;
        }

        template.AddUsing("Microsoft.AspNetCore.Components.Authorization");

        var securedStereotypes = element.GetStereotypes(Intent.Blazor.Api.ComponentModelStereotypeExtensions.Secured.DefinitionId).Reverse();

        var iterationIndex = 1;
        IHtmlElement? currentElement = null;
        IHtmlElement? childElement = null;
        foreach (var secure in securedStereotypes)
        {
            IHtmlElement authElement = new HtmlElement("AuthorizeView", template.RazorFile);
            var contextName = string.Empty;

            var roles = secure.GetProperty<string>("Roles");
            if (!string.IsNullOrWhiteSpace(roles))
            {
                authElement.AddAttribute("Roles", roles);
                contextName = $"{roles.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower)}Context";
            }

            var policy = secure.GetProperty<string>("Policy");
            if (!string.IsNullOrWhiteSpace(policy))
            {
                authElement.AddAttribute("Policy", policy);
                contextName = $"{policy.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower)}Context";
            }

            if (iterationIndex != securedStereotypes.Count())
            {
                authElement.AddAttribute("Context", contextName);
            }

            iterationIndex++;

            if (currentElement is not null)
            {
                authElement.AddChildNode(currentElement);
            }
            currentElement = authElement;
            childElement ??= authElement;
        }

        // this adds the main outer auth element to the parent
        htmlElement.AddHtmlElement(currentElement!);

        // return the inner most element, this is the one any child entities must be added to
        return childElement!;
    }

    public static void AuthorizeComponent(IRazorComponentTemplate template, IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node)
    {
        // no secured stereotype
        if (!component.HasStereotype(Intent.Blazor.Api.ComponentModelStereotypeExtensions.Secured.DefinitionId))
        {
            return;
        }

        template.AddUsing("Microsoft.AspNetCore.Components.Authorization");

        foreach (var element in razorNodes)
        {
            var currentElement = element;
            var iterationIndex = 1;

            var securedStereotypes = component.GetStereotypes(Intent.Blazor.Api.ComponentModelStereotypeExtensions.Secured.DefinitionId).Reverse();
            foreach (var secure in securedStereotypes)
            {
                IHtmlElement authElement = new HtmlElement("AuthorizeView", template.RazorFile);
                var contextName = string.Empty;

                var roles = secure.GetProperty<string>("Roles");
                if (!string.IsNullOrWhiteSpace(roles))
                {
                    authElement.AddAttribute("Roles", roles);
                    contextName = $"{roles.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower)}Context";
                }

                var policy = secure.GetProperty<string>("Policy");
                if (!string.IsNullOrWhiteSpace(policy))
                {
                    authElement.AddAttribute("Policy", policy);
                    contextName = $"{policy.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower)}Context";
                }

                if (iterationIndex != securedStereotypes.Count())
                {
                    authElement.AddAttribute("Context", contextName);
                }

                iterationIndex++;
                authElement.AddChildNode(currentElement);
                currentElement = authElement;
            }

            ScanAndReplaceNodes(node, currentElement, node);
        }
    }

    private static bool ScanAndReplaceNodes(IRazorFileNode element, IRazorFileNode authElement, IRazorFileNode node)
    {
        var index = node.ChildNodes.IndexOf(element);

        if (index >= 0)
        {
            node.InsertChildNode(index, authElement);
            node.ChildNodes.Remove(element);
            return true;
        }

        foreach (var childNode in node.ChildNodes)
        {
            var result = ScanAndReplaceNodes(childNode, authElement, node);
            if (result)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetSecuredText(string? policy, string? roles)
    {
        if (string.IsNullOrWhiteSpace(policy) && string.IsNullOrWhiteSpace(roles))
        {
            return string.Empty;
        }

        var attributeParameters = string.Empty;

        if (!string.IsNullOrWhiteSpace(policy))
        {
            attributeParameters = $"Policy = \"{policy}\"";
        }

        if (!string.IsNullOrWhiteSpace(roles))
        {
            attributeParameters += $"{(attributeParameters == string.Empty ? "" : ", ")}Roles = \"{roles}\"";
        }

        return attributeParameters;
    }
}
