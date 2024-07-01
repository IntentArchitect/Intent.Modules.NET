using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class CreateCustomerAddressDto
    {
        public CreateCustomerAddressDto()
        {
        }

        public string? Line1 { get; set; }

        public static CreateCustomerAddressDto Create(string? line1)
        {
            return new CreateCustomerAddressDto
            {
                Line1 = line1
            };
        }
    }
}