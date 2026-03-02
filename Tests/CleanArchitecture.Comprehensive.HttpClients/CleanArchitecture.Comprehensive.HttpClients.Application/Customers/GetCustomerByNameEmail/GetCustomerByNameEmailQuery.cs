using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.GetCustomerByNameEmail
{
    public class GetCustomerByNameEmailQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomerByNameEmailQuery(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}