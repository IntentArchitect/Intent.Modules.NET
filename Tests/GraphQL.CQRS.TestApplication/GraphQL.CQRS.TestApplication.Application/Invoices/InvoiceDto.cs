using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application.Common.Mappings;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomerById;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using HotChocolate;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
        }

        public Guid Id { get; set; }
        public int No { get; set; }
        public DateTime Created { get; set; }
        public Guid CustomerId { get; set; }

        public static InvoiceDto Create(Guid id, int no, DateTime created, Guid customerId)
        {
            return new InvoiceDto
            {
                Id = id,
                No = no,
                Created = created,
                CustomerId = customerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }

        //public async Task<CustomerDto> GetCustomer(
        //    CancellationToken cancellationToken,
        //    [Service] ISender mediator)
        //{
        //    return await mediator.Send(new GetCustomerByIdQuery { Id = CustomerId }, cancellationToken);
        //}

        public async Task<CustomerDto> GetCustomer(
            CancellationToken cancellationToken,
            [Service] ICustomerRepository repository,
            [Service] IMapper mapper)
        {
            var entity = await repository.FindByIdAsync(CustomerId, cancellationToken);
            return entity.MapToCustomerDto(mapper);
        }
    }
}