using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Layout
{
    public partial class NavLinks
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<NavItem> Items { get; set; } = [];

        public sealed record NavItem(string Label, string Href, string? Icon = null);
    }
}
