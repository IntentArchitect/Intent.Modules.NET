using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Concurrency.UpdateEntityAfterEtagWasChangedByPreviousOperationTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityAfterEtagWasChangedByPreviousOperationTestHandler : IRequestHandler<UpdateEntityAfterEtagWasChangedByPreviousOperationTest>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateEntityAfterEtagWasChangedByPreviousOperationTestHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            UpdateEntityAfterEtagWasChangedByPreviousOperationTest request,
            CancellationToken cancellationToken)
        {
            // Create a new customer
            var customer = new Customer
            {
                Name = "John",
                Surname = "Doe",
                IsActive = true,
                ShippingAddress = new Address(
                line1: "123 Street",
                line2: "Suburb",
                    city: "City",
                    postalCode: "0000"
                )
            };

            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // Read the two copies of the customer
            var customerA = await _customerRepository.FindByIdAsync(customer.Id, cancellationToken);
            if (customerA is null)
            {
                throw new NotFoundException($"Could not find CustomerA '{customer.Id}'");
            }
            var customerB = await _customerRepository.FindByIdAsync(customer.Id, cancellationToken);
            if (customerB is null)
            {
                throw new NotFoundException($"Could not find CustomerB '{customer.Id}'");
            }

            // Change the first copy
            customerA.Name = "Jane";
            _customerRepository.Update(customerA);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // Change the second copy
            // If Optimistic Concurrency is implemented correctly, this should throw an exception
            customerB.Name = "John";
            _customerRepository.Update(customerB);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}