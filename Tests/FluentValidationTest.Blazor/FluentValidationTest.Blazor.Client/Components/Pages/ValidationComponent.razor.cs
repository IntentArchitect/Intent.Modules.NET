using System;
using System.Threading.Tasks;
using FluentValidationTest.Blazor.Client.Contracts.Services.ValidationScenarios.RecursiveDtos;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace FluentValidationTest.Blazor.Client.Components.Pages
{
    public partial class ValidationComponent
    {
        public RecursiveNodeDto Model { get; set; }
        [Inject]
        public IRecursiveDtosService RecursiveDtosService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
        }

        private async Task Operation()
        {
            try
            {
                await RecursiveDtosService.ValidateRecursiveNodeAsync(Model);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}