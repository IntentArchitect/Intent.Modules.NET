using Microsoft.AspNetCore.Components;

namespace BlazorServerTests.Api.Components.Layout
{
    public partial class UserMenu
    {
        [Parameter]
        public string Title { get; set; } = "Account menu";

        /// <summary>The trigger content shown in the closed menu (e.g. an icon).</summary>
        [Parameter]
        public RenderFragment Trigger { get; set; }

        /// <summary>The menu items rendered inside the dropdown panel.</summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
