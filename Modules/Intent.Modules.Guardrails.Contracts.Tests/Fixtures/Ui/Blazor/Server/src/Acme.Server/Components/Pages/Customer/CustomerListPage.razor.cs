using Intent.RoslynWeaver.Attributes;
using Intent.Sample.Blazor.Server.Application.Common.Pagination;
using Intent.Sample.Blazor.Server.Application.Customers;
using Intent.Sample.Blazor.Server.Application.Customers.GetCustomers;
using Mediator;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Intent.Sample.Blazor.Server.Api.Components.Pages.Customer
{
    public partial class CustomerListPage
    {
        public PagedResult<CustomerSummaryDto> CustomersModels { get; set; } = default!;

        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private MudTable<CustomerSummaryDto>? _table;

        private async Task LoadCustomers(int pageNo, int pageSize, string? orderBy)
        {
            try
            {
                CustomersModels = await Mediator.Send(new GetCustomersQuery(
                    pageNo: pageNo,
                    pageSize: pageSize,
                    orderBy: orderBy));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        public async Task<TableData<CustomerSummaryDto>> LoadServerData(TableState state, CancellationToken cancellationToken)
        {
            await LoadCustomers(state.Page + 1, state.PageSize, state.SortLabel);
            return new TableData<CustomerSummaryDto>
            {
                Items = CustomersModels?.Data ?? [],
                TotalItems = CustomersModels?.TotalCount ?? 0
            };
        }

        private void NavigateToCustomerAddPage()
        {
            NavigationManager.NavigateTo("customer/customer-add-page");
        }

        private void NavigateToCustomerEditPage(Guid customerId)
        {
            NavigationManager.NavigateTo($"customer/edit/{customerId}");
        }
    }
}
