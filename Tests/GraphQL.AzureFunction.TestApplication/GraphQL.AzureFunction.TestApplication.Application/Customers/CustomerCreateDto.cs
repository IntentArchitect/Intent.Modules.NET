using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Customers
{
    public class CustomerCreateDto
    {
        public CustomerCreateDto()
        {
            Name = null!;
            LastName = null!;
        }

        public string Name { get; set; }
        public string LastName { get; set; }

        public static CustomerCreateDto Create(string name, string lastName)
        {
            return new CustomerCreateDto
            {
                Name = name,
                LastName = lastName
            };
        }
    }
}