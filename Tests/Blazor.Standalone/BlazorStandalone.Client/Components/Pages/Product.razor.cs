using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorStandalone.Client.Contracts.BlazorProductService.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorStandalone.Client.Components.Pages
{
    public partial class Product
    {
        public List<ProductDto>? Model { get; set; }
        [Inject]
        public IBlazorProductDefaultService BlazorProductDefaultService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var productDtos = await BlazorProductDefaultService.GetProductsAsync();
                Model = productDtos
                    .Select(m => new ProductDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Ref = m.Ref,
                        Qty = m.Qty
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}