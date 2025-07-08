using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using CleanArchitecture.Comprehensive.HttpClients.Application.Interfaces.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Implementation.Customers
{
    [IntentManaged(Mode.Merge)]
    public class CustomerService : ICustomerService
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public CustomerService(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task GetCustomersQuery(CancellationToken cancellationToken = default)
        {
            var result = await _customersService.GetCustomersAsync(cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task GetCustomerByIdQuery(CancellationToken cancellationToken = default)
        {
            var result = await _customersService.GetCustomerByIdAsync(default, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCustomerCommand(CancellationToken cancellationToken = default)
        {
            await _customersService.UpdateCustomerAsync(new UpdateCustomerCommand
            {
                Id = default,
                Name = default,
                Surname = default,
                Email = default,
                Address = new UpdateCustomerAddressDto
                {
                    Line1 = default,
                    Line2 = default,
                    City = default,
                    Postal = default,
                    Id = default
                }
            }, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCustomerCommand(CancellationToken cancellationToken = default)
        {
            await _customersService.DeleteCustomerAsync(default, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateCustomerCommand(CancellationToken cancellationToken = default)
        {
            var result = await _customersService.CreateCustomerAsync(new CreateCustomerCommand
            {
                Name = default,
                Surname = default,
                Email = default,
                Address = new CreateCustomerAddressDto
                {
                    Line1 = default,
                    Line2 = default,
                    City = default,
                    Postal = default
                }
            }, cancellationToken);
        }
    }
}