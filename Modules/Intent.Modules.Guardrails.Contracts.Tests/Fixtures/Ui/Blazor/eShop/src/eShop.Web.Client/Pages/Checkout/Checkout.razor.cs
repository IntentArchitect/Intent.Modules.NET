using System;
using System.Linq;
using System.Threading.Tasks;
using eShop.Web.Client.Contracts.Basket.Services.CustomerBaskets;
using eShop.Web.Client.Contracts.Ordering.Services.Orders;
using eShop.Web.Client.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace eShop.Web.Client.Pages.Checkout
{
    public partial class Checkout
    {
        private MudForm _form;
        private bool _checkoutCartProcessing = false;
        [IntentIgnore]
        [Inject]
        public BasketState BasketState { get; set; }
        public BasketDto Basket { get; set; }
        public ShippingInfo ShippingInformation { get; set; } = new ShippingInfo();
        [Inject]
        public IBasketService BasketService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public IOrdersService OrdersService { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Basket = await BasketService.GetBasketByIdAsync(BasketState.Id);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task CheckoutCart()
        {
            try
            {
                _checkoutCartProcessing = true;
                await OrdersService.CreateOrderAsync(new CreateOrderCommand
                {
                    UserId = Basket.BuyerId,
                    UserName = Basket.BuyerId,
                    Street = ShippingInformation.Street,
                    City = ShippingInformation.City,
                    OrderItems = Basket.BasketItems
                        .Select(oi => new OrderItemDto
                        {
                            ProductId = oi.ProductId,
                            ProductName = oi.ProductName,
                            UnitPrice = oi.UnitPrice,
                            Units = oi.Quantity
                        })
                        .ToList()
                });
                NavigationManager.NavigateTo("orders");
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _checkoutCartProcessing = false;
            }
        }

        public class ShippingInfo
        {
            public string Street { get; set; }
            public string City { get; set; }
        }
    }
}
