using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.DialogCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Pages.Customers.Components
{
    public partial class ConfirmationDialog
    {
        [Parameter]
        public string ContentText { get; set; }
        [Parameter]
        public string ButtonText { get; set; }
        [Parameter]
        public Color Color { get; set; }
        [CascadingParameter]
        public IMudDialogInstance Dialog { get; set; }

        private void Cancel()
        {
            Dialog.Cancel();
        }

        private void Submit()
        {
            Dialog.Close(true);
        }
    }
}