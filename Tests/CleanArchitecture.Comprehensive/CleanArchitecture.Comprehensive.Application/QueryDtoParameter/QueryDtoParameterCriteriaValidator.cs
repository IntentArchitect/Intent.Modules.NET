using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryDtoParameterCriteriaValidator : AbstractValidator<QueryDtoParameterCriteria>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public QueryDtoParameterCriteriaValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field1)
                .NotNull();

            RuleFor(v => v.Field2)
                .NotNull();
        }
    }
}