using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.DeleteBuyer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteBuyerCommandValidator : AbstractValidator<DeleteBuyerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteBuyerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}