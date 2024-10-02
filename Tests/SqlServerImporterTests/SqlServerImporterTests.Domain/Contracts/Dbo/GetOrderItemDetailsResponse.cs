using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Contracts.Dbo
{
    public record GetOrderItemDetailsResponse
    {
        public GetOrderItemDetailsResponse(Guid id,
            Guid orderId,
            int quantity,
            decimal amount,
            Guid productId,
            string refNo,
            string name,
            string surname,
            string email)
        {
            Id = id;
            OrderId = orderId;
            Quantity = quantity;
            Amount = amount;
            ProductId = productId;
            RefNo = refNo;
            Name = name;
            Surname = surname;
            Email = email;
        }

        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public int Quantity { get; init; }
        public decimal Amount { get; init; }
        public Guid ProductId { get; init; }
        public string RefNo { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
        public string Email { get; init; }
    }
}