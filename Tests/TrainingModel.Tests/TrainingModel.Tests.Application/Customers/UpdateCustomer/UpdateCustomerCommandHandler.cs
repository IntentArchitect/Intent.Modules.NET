using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Common;
using TrainingModel.Tests.Domain.Common.Exceptions;
using TrainingModel.Tests.Domain.Entities;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.Name = request.Name;
            customer.Surname = request.Surname;
            customer.Email = request.Email;
            customer.IsActive = request.IsActive;
            customer.Preferences ??= new Preferences();
            customer.Preferences.Specials = request.Specials;
            customer.Preferences.News = request.News;
            customer.Address = UpdateHelper.CreateOrUpdateCollection(customer.Address, request.Address, (e, d) => e.Id == d.Id, CreateOrUpdateAddress);
        }

        [IntentManaged(Mode.Fully)]
        private static Address CreateOrUpdateAddress(Address? entity, UpdateCustomerCommandAddressDto dto)
        {
            entity ??= new Address();
            entity.Id = dto.Id;
            entity.Line1 = dto.Line1;
            entity.Line2 = dto.Line2;
            entity.City = dto.City;
            entity.Postal = dto.Postal;
            entity.AddressType = dto.AddressType;
            return entity;
        }
    }
}