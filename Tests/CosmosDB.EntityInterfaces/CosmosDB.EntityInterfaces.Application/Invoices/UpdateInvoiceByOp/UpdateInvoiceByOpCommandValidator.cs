using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices.UpdateInvoiceByOp
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateInvoiceByOpCommandValidator : AbstractValidator<UpdateInvoiceByOpCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateInvoiceByOpCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Number)
                .NotNull();

            RuleFor(v => v.ClientIdentifier)
                .NotNull();
        }
    }
}