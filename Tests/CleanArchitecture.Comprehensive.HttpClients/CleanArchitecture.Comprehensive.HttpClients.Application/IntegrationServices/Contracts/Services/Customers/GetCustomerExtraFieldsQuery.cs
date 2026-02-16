using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
{
    public class GetCustomerExtraFieldsQuery
    {
        public GetCustomerExtraFieldsQuery()
        {
            Field1 = null!;
            Field2 = null!;
        }

        public Guid Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }

        public static GetCustomerExtraFieldsQuery Create(Guid id, string field1, string field2)
        {
            return new GetCustomerExtraFieldsQuery
            {
                Id = id,
                Field1 = field1,
                Field2 = field2
            };
        }
    }
}