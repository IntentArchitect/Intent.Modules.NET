using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItemById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetItemByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}