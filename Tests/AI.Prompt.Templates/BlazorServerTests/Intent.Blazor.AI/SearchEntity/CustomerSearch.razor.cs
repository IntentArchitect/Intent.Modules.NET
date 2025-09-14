using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UI.AI.Samples.Application.Common.Pagination;
using UI.AI.Samples.Application.Customers;
using UI.AI.Samples.Application.Customers.DeleteCustomer;
using UI.AI.Samples.Application.Customers.GetCustomers;
using UI.AI.Samples.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace UI.AI.Samples.Api.Components.Pages.Templates.Pages
{
    public partial class CustomerSearch
    {
        public PagedResult<CustomerSummaryDto> CustomersModels { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;

        public string? SearchText { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = null;
        private MudTable<CustomerSummaryDto>? _table;

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task DeleteCustomer(Guid customerId)
        {
            try
            {
                await Mediator.Send(new DeleteCustomerCommand(
                    id: customerId));
                Snackbar.Add("Customer deleted successfully.", Severity.Success);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task LoadCustomers(int pageNo, int pageSize, string? orderBy, string? searchTerm, bool? isActive)
        {
            try
            {
                CustomersModels = await Mediator.Send(new GetCustomersQuery(
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

        /// <summary>
        /// The CancellationToken cancelationToken is required.(IMPORTANT)
        /// </summary>
        public async Task<TableData<CustomerSummaryDto>> LoadServerData(TableState state, CancellationToken cancellationToken)
        {
            string? orderBy = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderBy = $"{state.SortLabel} {(state.SortDirection == SortDirection.Descending ? "desc" : "asc")}";
            }
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            await LoadCustomers(pageNo, pageSize, orderBy, SearchText, IsActive);
            var items = CustomersModels?.Data ?? new List<CustomerSummaryDto>();
            var totalItems = CustomersModels?.TotalCount ?? 0;
            return new TableData<CustomerSummaryDto>
            {
                Items = items,
                TotalItems = totalItems
            };
        }

        private void AddCustomer()
        {
            NavigationManager.NavigateTo("/templates/pages/customers/add");
        }

        private void EditCustomer(Guid customerId)
        {
            NavigationManager.NavigateTo($"/templates/pages/customers/edit/{customerId}");
        }

        private void ViewCustomer(Guid customerId)
        {
            NavigationManager.NavigateTo($"/templates/pages/customers/view/{customerId}");
        }

        /// <summary>
        /// This is how we do confirmation dialogs for example for deletes, using DialogService.ShowMessageBox.(IMPORTANT)
        /// </summary>
        private async void OnDeleteCustomer(Guid customerId)
        {
            var confirmed = await DialogService.ShowMessageBox(
                title: "Delete customer",
                markupMessage: (MarkupString)"Are you sure you want to delete this customer?",
                yesText: "Delete",
                cancelText: "Cancel",
                options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = false, BackdropClick = false }
            );
            if (confirmed == true)
            {
                await DeleteCustomer(customerId);
                if (_table is not null)
                    await _table.ReloadServerData();
            }
        }

        public class DeleteCustomerModel
        {
            public Guid? Id { get; set; }
        }
    }
}
