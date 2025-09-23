using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynAffiliates
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDynAffiliateDtoValidator : AbstractValidator<CreateDynAffiliateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDynAffiliateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}