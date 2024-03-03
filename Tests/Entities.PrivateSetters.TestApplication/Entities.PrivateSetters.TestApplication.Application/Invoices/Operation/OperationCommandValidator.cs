using Entities.PrivateSetters.TestApplication.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices.Operation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OperationCommandValidator : AbstractValidator<OperationCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public OperationCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Lines)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<OperationLineDataContractDto>()!));
        }
    }
}