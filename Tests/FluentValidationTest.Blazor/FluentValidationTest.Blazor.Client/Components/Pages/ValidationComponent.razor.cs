using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidationTest.Blazor.Client.Contracts.Services.SelfReferenceValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace FluentValidationTest.Blazor.Client.Components.Pages
{
    public partial class ValidationComponent
    {
        [Inject]
        public ISelfReferenceValidationService SelfReferenceValidationService { get; set; } = default!;
        [Inject]
        public ISelfRefDtoService SelfRefDtoService { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
        }

        private async Task UploadSelfRefDtoCommand(string entry, List<SelfRefDto> entries)
        {
            try
            {
                await SelfReferenceValidationService.UploadSelfRefDtoAsync(new UploadSelfRefDtoCommand
                {
                    Entry = entry,
                    SelfRefDtos = entries
                });
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }

        private async Task SelfRefDtoServiceUpload(UploadWrapperDto param)
        {
            try
            {
                await SelfRefDtoService.UploadAsync(param);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
        }
    }
}