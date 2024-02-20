using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.FindCustomerByName
{
    public class FindCustomerByNameQuery : IRequest<CustomerDto>, IQuery
    {
        public FindCustomerByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}