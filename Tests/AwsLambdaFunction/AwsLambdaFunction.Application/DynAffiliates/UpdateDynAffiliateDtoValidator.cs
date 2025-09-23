using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynAffiliates
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDynAffiliateDtoValidator : AbstractValidator<UpdateDynAffiliateDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDynAffiliateDtoValidator()
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