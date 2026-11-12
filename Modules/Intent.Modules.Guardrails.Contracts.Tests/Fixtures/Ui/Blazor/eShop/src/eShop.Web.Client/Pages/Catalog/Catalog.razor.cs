using System;
using System.Threading.Tasks;
using eShop.Web.Client.Contracts.Catalog.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace eShop.Web.Client.Pages.Catalog
{
    public partial class Catalog
    {
        private const int PageSize = 9;
        [SupplyParameterFromQuery]
        public int? BrandId { get; set; }
        [SupplyParameterFromQuery]
        public int? ItemTypeId { get; set; }
        [SupplyParameterFromQuery]
        public int Page { get; set; } = 1;
        public PaginatedItems<CatalogItemDto> Model { get; set; }
        [Inject]
        public ICatalogService CatalogService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await CatalogService.ItemsByTypeIdAndBrandIdAsync(
                    ItemTypeId, BrandId, PageSize, Page);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}
