using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Auth
{
    public partial class Login
    {
        private MudForm _form;
        private bool _onLoginClickedProcessing = false;
        public string ErrorMessage { get; set; }
        public LoginModel Model { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task OnLoginClicked()
        {
            await _form!.Validate();
            if (!_form.IsValid)
            {
                return;
            }
        }

        private void OnRegisterClicked()
        {
        }
    }
}