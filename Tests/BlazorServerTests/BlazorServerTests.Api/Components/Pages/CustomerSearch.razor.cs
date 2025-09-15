using BlazorServerTests.Application.Common.Pagination;
using BlazorServerTests.Application.Customers;
using BlazorServerTests.Application.Customers.GetCustomers;
using BlazorServerTests.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Pages
{
    public partial class CustomerSearch
    {
        public PagedResult<CustomerDto> Model { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        public string? SearchText { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = null;
        private MudTable<CustomerDto>? _table;

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task LoadCustomers(int pageNo, int pageSize, string? orderBy, string? searchTerm, bool? isActive)
        {
            try
            {
                Model = await Mediator.Send(new GetCustomersQuery(
                    pageNo: pageNo,
                    pageSize: pageSize,
                    orderBy: orderBy,
                    searchTerm: searchTerm,
                    isActive: isActive));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private void AddCustomer()
        {
            NavigationManager.NavigateTo("/customer-add");
        }

        public async Task<TableData<CustomerDto>> LoadServerData(TableState state, CancellationToken cancellationToken)
        {
            string? orderBy = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderBy = $"{state.SortLabel} {(state.SortDirection == SortDirection.Descending ? "desc" : "asc")}";
            }
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            await LoadCustomers(pageNo, pageSize, orderBy, SearchText, IsActive);
            var items = Model?.Data ?? Enumerable.Empty<CustomerDto>();
            var totalItems = Model?.TotalCount ?? 0;
            return new TableData<CustomerDto>
            {
                Items = items,
                TotalItems = totalItems
            };
        }
    }
}
