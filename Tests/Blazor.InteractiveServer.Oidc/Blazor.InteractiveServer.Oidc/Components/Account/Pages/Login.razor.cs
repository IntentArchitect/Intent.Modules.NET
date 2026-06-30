using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.LoginCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Oidc.Components.Account.Pages
{
    public partial class Login
    {
        private string? errorMessage;
        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();
        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        [IgnoreAntiforgeryToken]
        public async Task LoginUser()
        {
            await AuthService.Login(Input.Email, Input.Password, Input.RememberMe, ReturnUrl);
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
    }
}