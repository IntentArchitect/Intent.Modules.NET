using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Optionals.GetOptionalById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOptionalByIdQueryValidator : AbstractValidator<GetOptionalByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOptionalByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}