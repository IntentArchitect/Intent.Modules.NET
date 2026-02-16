using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallGetCustomerExtraFieldsQuery
{
    public class CallGetCustomerExtraFieldsQuery : IRequest<List<CustomerDto>>, ICommand
    {
        public CallGetCustomerExtraFieldsQuery(Guid id, string field1, string field2)
        {
            Id = id;
            Field1 = field1;
            Field2 = field2;
        }

        public Guid Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}