using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eShop.Web.Client.Contracts.Ordering.Services.Orders;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace eShop.Web.Client.Pages.User
{
    public partial class Orders
    {
        public List<OrderSummaryDto> Model { get; set; }
        [Inject]
        public IOrdersService OrdersService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [IntentIgnore]
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeComponentAsync();
                StateHasChanged();
            }
        }

        private async Task InitializeComponentAsync()
        {
            try
            {
                Model = await OrdersService.GetOrdersAsync();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}
