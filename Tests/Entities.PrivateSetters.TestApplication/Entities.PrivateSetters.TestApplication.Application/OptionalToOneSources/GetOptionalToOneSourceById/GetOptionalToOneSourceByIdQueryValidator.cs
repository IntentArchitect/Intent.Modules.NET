using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.GetOptionalToOneSourceById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOptionalToOneSourceByIdQueryValidator : AbstractValidator<GetOptionalToOneSourceByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetOptionalToOneSourceByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}