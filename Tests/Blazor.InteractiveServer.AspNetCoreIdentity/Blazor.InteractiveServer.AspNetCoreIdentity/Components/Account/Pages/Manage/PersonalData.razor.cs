using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class PersonalData
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _ = await UserAccessor.GetRequiredUserAsync(HttpContext);
        }

        private void NavigateToDeletePersonalData()
        {
            NavigationManager.NavigateTo("Account/Manage/DeletePersonalData");
        }
    }
}
