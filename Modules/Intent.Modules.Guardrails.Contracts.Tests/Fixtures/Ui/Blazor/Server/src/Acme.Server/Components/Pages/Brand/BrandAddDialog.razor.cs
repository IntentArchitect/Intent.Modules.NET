using Intent.RoslynWeaver.Attributes;
using Intent.Sample.Blazor.Server.Application.Brands.CreateBrand;
using Mediator;
using Microsoft.AspNetCore.Components;
using MudBlazor;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Intent.Sample.Blazor.Server.Api.Components.Pages.Brand
{
    public partial class BrandAddDialog
    {
        public CreateBrandModel Model { get; set; } = new();

        [Inject]
        public IScopedMediator Mediator { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [CascadingParameter]
        public IMudDialogInstance Dialog { get; set; } = default!;

        private MudForm? _form;
        private bool _isSaving;

        private async Task SaveAsync()
        {
            if (_form == null || _isSaving)
            {
                return;
            }

            await _form.Validate();
            if (!_form.IsValid)
            {
                return;
            }

            _isSaving = true;
            try
            {
                await Mediator.Send(new CreateBrandCommand(
                    name: Model.Name));
                Snackbar.Add("Brand created.", Severity.Success);
                Dialog.Close(DialogResult.Ok(true));
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private void Cancel()
        {
            Dialog.Cancel();
        }

        public class CreateBrandModel
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
