using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Customers
{
    public class CustomerUpdateDto
    {
        public CustomerUpdateDto()
        {
            Name = null!;
            LastName = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public static CustomerUpdateDto Create(Guid id, string name, string lastName)
        {
            return new CustomerUpdateDto
            {
                Id = id,
                Name = name,
                LastName = lastName
            };
        }
    }
}