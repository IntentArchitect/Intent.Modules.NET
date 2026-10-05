using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Common.UserMenuCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Layout
{
    public partial class UserMenu
    {
        [Parameter]
        public string Title { get; set; } = "Account menu";

        /// <summary>The trigger content shown in the closed menu (e.g. an icon).</summary>
        [Parameter]
        public RenderFragment? Trigger { get; set; }

        /// <summary>The menu items rendered inside the dropdown panel.</summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
