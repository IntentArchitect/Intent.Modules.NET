using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfAffiliates
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEfAffiliateDtoValidator : AbstractValidator<UpdateEfAffiliateDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEfAffiliateDtoValidator()
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