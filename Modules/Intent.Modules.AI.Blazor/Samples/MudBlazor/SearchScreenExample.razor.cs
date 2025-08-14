using BlazorServer.Sample.App.Application.Common.Pagination;
using BlazorServer.Sample.App.Application.Customers;
using BlazorServer.Sample.App.Application.Customers.DeleteCustomer;
using BlazorServer.Sample.App.Application.Customers.GetCustomersPaged;
using BlazorServer.Sample.App.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServer.Sample.App.Api.Components.Pages.Customers
{
    public partial class Customer
    {
        public PagedResult<CustomerSummaryDto> Model { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject] public IDialogService DialogService { get; set; } = default!;

        public string SearchText { get; set; }

        private MudTable<CustomerSummaryDto>? _table;

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task LoadCustomersPaged(int pageNo, int pageSize, string? orderBy, string searchTerm)
        {
            try
            {
                Model = await Mediator.Send(new GetCustomersPagedQuery(
                    pageNo: pageNo,
                    pageSize: pageSize,
                    orderBy: orderBy,
                    searchTerm: searchTerm));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private void AddCustomer()
        {
            NavigationManager.NavigateTo("/customers-example/add-customer");
        }

        private void ViewCustomer(Guid customerId)
        {
            NavigationManager.NavigateTo($"/customers-example/view-customer/{customerId}");
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

        private void EditCustomer(Guid customerId)
        {
            NavigationManager.NavigateTo($"/customers-example/edit-customer/{customerId}");
        }

        public class DeleteCustomerModel
        {
            public Guid Id { get; set; }
        }

        /// <summary>
        /// The CancellationToken cancelationToken is required.(IMPORTANT)
        /// </summary>
        /// <param name="customerId"></param>
        public async Task<TableData<CustomerSummaryDto>> LoadServerData(TableState state, CancellationToken cancelationToken)
        {
            string? orderBy = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderBy = $"{state.SortLabel} {(state.SortDirection == SortDirection.Descending ? "desc" : "asc")}";
            }
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            // As MudBlazor calls with only state and cancelationToken, no search term parameter is passed; use empty string
            await LoadCustomersPaged(pageNo, pageSize, orderBy, SearchText);
            var items = Model?.Data ?? new List<CustomerSummaryDto>();
            var totalItems = Model?.TotalCount ?? 0;
            return new TableData<CustomerSummaryDto>
            {
                Items = items,
                TotalItems = totalItems
            };
        }

        /// <summary>
        /// This is how we do confirmation dialogs for example for deletes, using DialogService.ShowMessageBox.(IMPORTANT)
        /// </summary>
        /// <param name="customerId"></param>
        private async void OnDeleteCustomer(Guid customerId)
        {
            var confirmed = await DialogService.ShowMessageBox(
                title: "Delete customer",
                markupMessage: (MarkupString)"Are you sure you want to delete this customer?",
                yesText: "Delete",
                cancelText: "Cancel",
                options: new DialogOptions
                {
                    MaxWidth = MaxWidth.ExtraSmall,
                    CloseOnEscapeKey = false,
                    BackdropClick = false,
                });

            if (confirmed == true)
            {
                await DeleteCustomer(customerId);
                if (_table is not null)
                    await _table.ReloadServerData();
            }
        }

    }
}
