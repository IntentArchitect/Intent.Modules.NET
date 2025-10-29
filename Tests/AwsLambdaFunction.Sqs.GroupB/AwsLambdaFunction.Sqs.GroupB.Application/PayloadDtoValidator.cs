using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PayloadDtoValidator : AbstractValidator<PayloadDto>
    {
        [IntentManaged(Mode.Merge)]
        public PayloadDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Data)
                .NotNull();
        }
    }
}