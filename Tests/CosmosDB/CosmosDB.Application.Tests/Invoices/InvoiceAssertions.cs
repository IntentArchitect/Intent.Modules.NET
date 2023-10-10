using System.Collections.Generic;
using System.Linq;
using CosmosDB.Application.Invoices;
using CosmosDB.Application.Invoices.CreateInvoiceLineItem;
using CosmosDB.Application.Invoices.UpdateInvoice;
using CosmosDB.Application.Invoices.UpdateInvoiceLineItem;
using CosmosDB.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public static class InvoiceAssertions
    {
        public static void AssertEquivalent(CreateInvoiceLineItemCommand expectedDto, LineItem actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Description.Should().Be(expectedDto.Description);
            actualEntity.Quantity.Should().Be(expectedDto.Quantity);
        }

        public static void AssertEquivalent(
            IEnumerable<InvoiceLineItemDto> actualDtos,
            IEnumerable<LineItem> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().BeNullOrEmpty();
                return;
            }

            actualDtos.Should().HaveSameCount(actualDtos);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var entity = expectedEntities.ElementAt(i);
                var dto = actualDtos.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Id.Should().Be(entity.Id);
                dto.Description.Should().Be(entity.Description);
                dto.Quantity.Should().Be(entity.Quantity);
            }
        }

        public static void AssertEquivalent(InvoiceLineItemDto actualDto, LineItem expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.Description.Should().Be(expectedEntity.Description);
            actualDto.Quantity.Should().Be(expectedEntity.Quantity);
        }

        public static void AssertEquivalent(UpdateInvoiceLineItemCommand expectedDto, LineItem actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Description.Should().Be(expectedDto.Description);
            actualEntity.Quantity.Should().Be(expectedDto.Quantity);
        }

        public static void AssertEquivalent(IEnumerable<InvoiceDto> actualDtos, IEnumerable<Invoice> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().BeNullOrEmpty();
                return;
            }

            actualDtos.Should().HaveSameCount(actualDtos);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var entity = expectedEntities.ElementAt(i);
                var dto = actualDtos.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Id.Should().Be(entity.Id);
                dto.ClientId.Should().Be(entity.ClientIdentifier);
                dto.Date.Should().Be(entity.Date);
                dto.Number.Should().Be(entity.Number);
            }
        }

        public static void AssertEquivalent(InvoiceDto actualDto, Invoice expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.ClientId.Should().Be(expectedEntity.ClientIdentifier);
            actualDto.Date.Should().Be(expectedEntity.Date);
            actualDto.Number.Should().Be(expectedEntity.Number);
        }

        public static void AssertEquivalent(UpdateInvoiceCommand expectedDto, Invoice actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.ClientIdentifier.Should().Be(expectedDto.ClientId);
            actualEntity.Date.Should().Be(expectedDto.Date);
            actualEntity.Number.Should().Be(expectedDto.Number);
        }
    }
}