using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomerByName
{
    public class GetCustomerByNameQuery : IRequest<List<CustomerDto>>, IQuery
    {
        public GetCustomerByNameQuery(string? name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        public string? Name { get; set; }
        public string Surname { get; set; }
    }
}