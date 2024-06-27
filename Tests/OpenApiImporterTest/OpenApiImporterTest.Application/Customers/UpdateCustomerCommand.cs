using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
        }

        public string? Name { get; set; }
        public bool Isac { get; set; }
        public Guid Id { get; set; }

        public static UpdateCustomerCommand Create(string? name, bool isac, Guid id)
        {
            return new UpdateCustomerCommand
            {
                Name = name,
                Isac = isac,
                Id = id
            };
        }
    }
}