using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Address = null!;
        }

        public string? Name { get; set; }
        public bool Isac { get; set; }
        public CreateCustomerAddressDto Address { get; set; }
        public string? NullableString { get; set; }
        public bool? NullableBool { get; set; }

        public static CreateCustomerCommand Create(
            string? name,
            bool isac,
            CreateCustomerAddressDto address,
            string? nullableString,
            bool? nullableBool)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                Isac = isac,
                Address = address,
                NullableString = nullableString,
                NullableBool = nullableBool
            };
        }
    }
}