using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Customers.DeleteCustomer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCustomerCommandValidator()
        {
        }
    }
}