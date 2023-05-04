using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using HotChocolate;

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.QueryResolvers;

public class InvoiceType
{
    public InvoiceType(Invoice invoice)
    {
        Id = invoice.Id;
        No = invoice.No;
        Created = invoice.Created;
        CustomerId = invoice.CustomerId;
        InvoiceLines = invoice.InvoiceLines.ToList();
    }

    public Guid Id { get; set; }

    public int No { get; set; }

    public DateTime Created { get; set; }

    public Guid CustomerId { get; set; }

    public IReadOnlyCollection<InvoiceLine> InvoiceLines { get; set; }

    public async Task<CustomerType> GetCustomer([Service] ICustomerRepository customerRepository)
    {
        var customer = await customerRepository.FindByIdAsync(CustomerId);
        return new CustomerType(customer);
    }
}