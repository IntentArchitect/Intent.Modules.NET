using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public class UpdateOrderCustomerDto
    {
        public UpdateOrderCustomerDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static UpdateOrderCustomerDto Create(string name, Guid id)
        {
            return new UpdateOrderCustomerDto
            {
                Name = name,
                Id = id
            };
        }
    }
}