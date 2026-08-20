using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Account.Shared
{
    public partial class ShowRecoveryCodes
    {
        [Parameter]
        public string[] RecoveryCodes { get; set; } = [];

        [Parameter]
        public string? StatusMessage { get; set; }
    }
}
