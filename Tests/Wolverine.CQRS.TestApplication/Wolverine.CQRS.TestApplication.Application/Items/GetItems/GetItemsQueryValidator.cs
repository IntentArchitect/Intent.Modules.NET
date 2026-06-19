using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.QueryValidator", Version = "2.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItems
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetItemsQueryValidator : AbstractValidator<GetItemsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetItemsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}