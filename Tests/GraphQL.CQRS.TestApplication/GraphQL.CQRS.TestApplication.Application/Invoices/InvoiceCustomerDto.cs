using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application.Common.Mappings;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesForCustomer;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using HotChocolate;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices
{
    public class InvoiceCustomerDto : IMapFrom<Customer>
    {
        public InvoiceCustomerDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }

        public static InvoiceCustomerDto Create(string name, string surname, string email, Guid id)
        {
            return new InvoiceCustomerDto
            {
                Name = name,
                Surname = surname,
                Email = email,
                Id = id
            };
        }

        public async Task<List<InvoiceDto>> GetInvoices(CancellationToken cancellationToken, [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesForCustomerQuery(customerId: Id), cancellationToken);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, InvoiceCustomerDto>();
        }
    }
}