using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Jwt.Components.Account.Pages
{
    public partial class ForgotPasswordJwt
    {
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

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