using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using HotChocolate;

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.QueryResolvers;

public class CustomerType
{
    public CustomerType(Customer customer)
    {
        Id = customer.Id;
        Name = customer.Name;
        Surname = customer.Surname;
        Email = customer.Email;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public async Task<IReadOnlyCollection<InvoiceType>> GetInvoices([Service] IInvoiceRepository invoiceRepository, CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.FindAllAsync(x => x.CustomerId == Id, cancellationToken);
        return invoices.Select(x => new InvoiceType(x)).ToList();
    }
}