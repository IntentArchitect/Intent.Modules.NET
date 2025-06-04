using AspNetCoreCleanArchitecture.Sample.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.UpdateBuyer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBuyerCommandValidator : AbstractValidator<UpdateBuyerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBuyerCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull()
                .MaximumLength(30);

            RuleFor(v => v.Surname)
                .NotNull()
                .MaximumLength(30);

            RuleFor(v => v.Email)
                .NotNull()
                .MaximumLength(256);

            RuleFor(v => v.Address)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateBuyerAddressDto>()!);
        }
    }
}