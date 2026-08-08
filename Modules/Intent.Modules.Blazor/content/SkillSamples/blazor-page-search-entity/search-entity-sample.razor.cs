using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
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
        public PagedResult<CustomerSummaryDto>? CustomersModels { get; set; }
        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        public string? SearchText { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public string? IsActiveText
        {
            get => IsActive?.ToString().ToLowerInvariant();
            set => IsActive = string.IsNullOrEmpty(value) ? null : bool.Parse(value);
        }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        private Guid? _pendingDeleteId;
        private bool _showDeleteConfirm;

        protected override async Task OnInitializedAsync()
        {
            await ReloadAsync();
        }

        private async Task OnSearchKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await ReloadAsync();
            }
        }

        private async Task ReloadAsync()
        {
            PageNo = 1;
            await LoadCustomers();
        }

        private async Task PreviousPage()
        {
            if (PageNo <= 1) return;
            PageNo--;
            await LoadCustomers();
        }

        private async Task NextPage()
        {
            PageNo++;
            await LoadCustomers();
        }

        private async Task LoadCustomers()
        {
            CustomersModels = await Mediator.Send(new GetCustomersQuery(
                pageNo: PageNo,
                pageSize: PageSize,
                orderBy: null,
                searchTerm: SearchText,
                isActive: IsActive));
        }

        private void OnDeleteCustomer(Guid customerId)
        {
            _pendingDeleteId = customerId;
            _showDeleteConfirm = true;
        }

        private void CancelDelete()
        {
            _pendingDeleteId = null;
            _showDeleteConfirm = false;
        }

        private async Task ConfirmDeleteAsync()
        {
            if (_pendingDeleteId is null) return;
            await Mediator.Send(new DeleteCustomerCommand(
                id: _pendingDeleteId.Value));
            _pendingDeleteId = null;
            _showDeleteConfirm = false;
            await LoadCustomers();
        }

        private void NavigateToCustomerAddPage()
        {
            NavigationManager.NavigateTo("templates/pages/customers/add");
        }

        private void NavigateToCustomerEditPage(Guid customerId)
        {
            NavigationManager.NavigateTo($"templates/pages/customers/edit/{customerId}");
        }

        private void NavigateToCustomerViewPage(Guid customerId)
        {
            NavigationManager.NavigateTo($"templates/pages/customers/view/{customerId}");
        }
    }
}
