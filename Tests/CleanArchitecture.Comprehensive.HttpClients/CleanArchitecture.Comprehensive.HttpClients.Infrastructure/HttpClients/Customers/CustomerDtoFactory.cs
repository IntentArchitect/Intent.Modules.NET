using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.Fakes.ResponseDtoFactory", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.HttpClients.Customers
{
    public static class CustomerDtoFactory
    {
        public static CustomerDto CreateDefault()
        {
            return new CustomerDto
            {
                Id = Guid.Empty,
                Email = "string",
                Name = "string",
                Surname = "string"
            };
        }

        public static CustomerDto CreateDefault(Action<CustomerDto> configure)
        {
            return FactoryHelpers.Configure(CreateDefault(), configure);
        }

        public static List<CustomerDto> CreateDefaultList(int count, Action<CustomerDto, int>? configure = null)
        {
            return FactoryHelpers.List(CreateDefault, count, configure);
        }
    }
}