using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.AzureFunction.TestApplication.Application.Common.Pagination;
using GraphQL.AzureFunction.TestApplication.Application.Customers;
using GraphQL.AzureFunction.TestApplication.Application.Interfaces;
using GraphQL.AzureFunction.TestApplication.Domain.Common.Exceptions;
using GraphQL.AzureFunction.TestApplication.Domain.Entities;
using GraphQL.AzureFunction.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CustomersService : ICustomersService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CustomersService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> CreateCustomer(CustomerCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newCustomer = new Customer
            {
                Name = dto.Name,
                LastName = dto.LastName,
            };
            _customerRepository.Add(newCustomer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCustomer.MapToCustomerDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> FindCustomerById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _customerRepository.FindByIdAsync(id, cancellationToken);

            if (element is null)
            {
                throw new NotFoundException($"Could not find Customer {id}");
            }
            return element.MapToCustomerDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> FindCustomers(CancellationToken cancellationToken = default)
        {
            var elements = await _customerRepository.FindAllAsync(cancellationToken);
            return elements.MapToCustomerDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> UpdateCustomer(
            Guid id,
            CustomerUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingCustomer = await _customerRepository.FindByIdAsync(id, cancellationToken);

            if (existingCustomer is null)
            {
                throw new NotFoundException($"Could not find Customer {id}");
            }
            existingCustomer.Name = dto.Name;
            existingCustomer.LastName = dto.LastName;
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return existingCustomer.MapToCustomerDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            var existingCustomer = await _customerRepository.FindByIdAsync(id, cancellationToken);

            if (existingCustomer is null)
            {
                throw new NotFoundException($"Could not find Customer {id}");
            }
            _customerRepository.Remove(existingCustomer);
            return existingCustomer.MapToCustomerDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<CustomerDto>> GetCustomersPaged(
            int pageNo,
            int pageCount,
            List<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var existingCustomers = await _customerRepository.FindAllAsync(pageNo, pageCount, cancellationToken);
            return existingCustomers.MapToPagedResult(customer => customer.MapToCustomerDto(_mapper));
        }
    }
}