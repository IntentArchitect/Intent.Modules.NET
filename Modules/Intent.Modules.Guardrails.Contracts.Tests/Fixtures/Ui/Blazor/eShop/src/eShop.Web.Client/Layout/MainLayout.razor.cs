using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutCodeBehindTemplate", Version = "1.0")]

namespace eShop.Web.Client.Layout
{
    public partial class MainLayout
    {
        [IntentIgnore]
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [IntentIgnore]
        private void RedirectToLogin()
        {
            NavigationManager.NavigateTo($"Account/Login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}", forceLoad: true);
        }

        private void NavigateToCatalog(int? brandId, int itemTypeId)
        {
            NavigationManager.NavigateTo($"?BrandId={brandId}&ItemTypeId={itemTypeId}");
        }
    }
}
