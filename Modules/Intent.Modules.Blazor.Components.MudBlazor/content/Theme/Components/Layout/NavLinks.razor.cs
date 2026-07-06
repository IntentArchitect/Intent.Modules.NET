using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace <#= Namespace #>Components.Layout
{
    public partial class NavLinks
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<NavItem> Items { get; set; } = [];

        private static NavLinkMatch GetMatch(NavItem item) => item.Href == "/"
            ? NavLinkMatch.All
            : NavLinkMatch.Prefix;

    public sealed record NavItem(string Label, string Href, string? Icon = null);
    }
}
