using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Customers.GetCustomerById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerByIdQueryValidator()
        {
        }
    }
}