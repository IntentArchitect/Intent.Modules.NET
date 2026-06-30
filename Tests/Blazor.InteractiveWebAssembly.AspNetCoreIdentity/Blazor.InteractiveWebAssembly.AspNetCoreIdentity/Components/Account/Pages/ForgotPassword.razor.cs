using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.ForgotPasswordCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class ForgotPassword
    {
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        protected override void OnInitialized()
        {
            Input ??= new();
        }

        private async Task OnValidSubmitAsync()
        {
            await AuthService.ForgotPassword(Input.Email);
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";
        }
    }
}