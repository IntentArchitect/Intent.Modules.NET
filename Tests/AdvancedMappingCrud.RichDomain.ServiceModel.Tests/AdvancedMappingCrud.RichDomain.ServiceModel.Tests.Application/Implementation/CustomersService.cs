using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CustomersService : ICustomersService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CustomersService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateCustomer(CustomerCreateDto dto, CancellationToken cancellationToken = default)
        {
            var customer = new Customer(
                user: new User(
                    companyId: dto.User.CompanyId,
                    contactDetailsVO: new ContactDetailsVO(
                        cell: dto.User.ContactDetailsVO.Cell,
                        email: dto.User.ContactDetailsVO.Email),
                    addresses: dto.User.Addresses
                        .Select(a => new AddressDC(
                            line1: a.Line1,
                            line2: a.Line2,
                            city: a.City,
                            postal: a.Postal))
                        .ToList()),
                login: dto.Login);

            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> FindCustomerById(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.FindByIdAsync(id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{id}'");
            }
            return customer.MapToCustomerDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> FindCustomers(CancellationToken cancellationToken = default)
        {
            var customers = await _customerRepository.FindAllAsync(cancellationToken);
            return customers.MapToCustomerDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.FindByIdAsync(id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{id}'");
            }

            _customerRepository.Remove(customer);
        }

        public void Dispose()
        {
        }
    }
}