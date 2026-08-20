using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.Pages.Auth;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.PageCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Pages.Auth
{
    public partial class Register
    {
        private MudForm _form;
        private bool _onRegisterClickedProcessing = false;
        public RegisterModel Model { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task OnRegisterClicked()
        {
            await _form!.Validate();
            if (!_form.IsValid)
            {
                return;
            }
        }
    }
}