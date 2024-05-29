using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.GetSubmissionById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetSubmissionByIdQueryValidator : AbstractValidator<GetSubmissionByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetSubmissionByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}