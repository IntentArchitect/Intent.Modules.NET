using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomersPaged
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersPagedQueryValidator : AbstractValidator<GetCustomersPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedQueryValidator()
        {
        }
    }
}