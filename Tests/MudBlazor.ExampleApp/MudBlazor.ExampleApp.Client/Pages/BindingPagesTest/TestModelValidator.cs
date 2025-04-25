using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.FluentValidation.ModelDefinitionValidator", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.BindingPagesTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestModelValidator : AbstractValidator<TestModel>
    {
        [IntentManaged(Mode.Merge)]
        public TestModelValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}