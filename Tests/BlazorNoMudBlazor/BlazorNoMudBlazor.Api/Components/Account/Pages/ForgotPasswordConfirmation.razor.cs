using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account.Pages
{
    public partial class ForgotPasswordConfirmation
    {
    }
}
