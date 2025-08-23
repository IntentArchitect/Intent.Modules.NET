using Microsoft.AspNetCore.Components;
using MudBlazor;
using RetailArch.Workshops.Orders.Application.Common.Pagination;
using RetailArch.Workshops.Orders.Application.Customers;
using RetailArch.Workshops.Orders.Application.Customers.GetCustomersPaged;
using RetailArch.Workshops.Orders.Infrastructure.Services;

namespace RetailArch.Workshops.Orders.Api.Components.Pages.Customers;

public partial class LookupCustomerDialog : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] public IScopedMediator Mediator { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    [Parameter] public bool MultiSelect { get; set; } = false;
    [Parameter] public string Title { get; set; } = "Select Customer";

    protected string _searchTerm = "";
    protected bool _loading = false;

    public PagedResult<CustomerSummaryDto> Model { get; set; }

    private CustomerSummaryDto? _selectedItem;
    private HashSet<CustomerSummaryDto> _selectedItems = new(new CustomerComparer());

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

    protected async Task<TableData<CustomerSummaryDto>> LoadServerData(TableState state, CancellationToken cancellationToken)
    {
        _loading = true;
        try
        {
            string? orderBy = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderBy = $"{state.SortLabel} {(state.SortDirection == SortDirection.Descending ? "desc" : "asc")}";
            }
            var pageNo = state.Page + 1;
            var pageSize = state.PageSize;
            await LoadCustomersPaged(pageNo, pageSize, orderBy, _searchTerm);
            var items = Model?.Data ?? new List<CustomerSummaryDto>();
            var totalItems = Model?.TotalCount ?? 0;
            return new TableData<CustomerSummaryDto>
            {
                Items = items,
                TotalItems = totalItems
            };

        }
        catch (Exception e)
        {
            Snackbar.Add($"Failed to load customers: {e.Message}", Severity.Error);
            return new TableData<CustomerSummaryDto> { Items = new List<CustomerSummaryDto>(), TotalItems = 0 };
        }
        finally { _loading = false; }
    }

    private sealed class CustomerComparer : IEqualityComparer<CustomerSummaryDto>
    {
        public bool Equals(CustomerSummaryDto? x, CustomerSummaryDto? y) => x?.CustomerId == y?.CustomerId;
        public int GetHashCode(CustomerSummaryDto obj) => obj.CustomerId.GetHashCode();
    }

    protected Task HandleRowClick(TableRowClickEventArgs<CustomerSummaryDto> args)
    {
        // For single-select, close immediately on row click
        if (!MultiSelect)
            MudDialog.Close(DialogResult.Ok(args.Item));

        return Task.CompletedTask;
    }

    protected void ConfirmSelection()
    {
        MudDialog.Close(DialogResult.Ok(_selectedItems.ToList()));
    }

    protected void Cancel() => MudDialog.Cancel();
}
