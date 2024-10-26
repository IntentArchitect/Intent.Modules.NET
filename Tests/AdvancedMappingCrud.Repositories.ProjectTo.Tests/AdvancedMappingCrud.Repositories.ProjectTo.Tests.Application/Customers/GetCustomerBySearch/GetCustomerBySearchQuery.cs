using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomerBySearch
{
    public class GetCustomerBySearchQuery : IRequest<CustomerDto>, IQuery
    {
        public GetCustomerBySearchQuery(string? name, string? email)
        {
            Name = name;
            Email = email;
        }

        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}