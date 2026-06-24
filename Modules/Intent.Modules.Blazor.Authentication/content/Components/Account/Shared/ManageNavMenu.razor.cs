using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace <#= Namespace #>Components.Account.Shared
{
    public partial class ManageNavMenu
    {
        private bool hasExternalLogins;

        protected override async Task OnInitializedAsync()
        {
            hasExternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).Any();
        }
    }
}
