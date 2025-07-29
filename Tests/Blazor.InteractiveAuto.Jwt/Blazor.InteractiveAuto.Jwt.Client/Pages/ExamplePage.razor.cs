using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Jwt.Client.Pages
{
    public partial class ExamplePage
    {
        [Parameter]
        public string Title { get; set; } = "Example Page";

        protected override async Task OnInitializedAsync()
        {
        }
    }
}