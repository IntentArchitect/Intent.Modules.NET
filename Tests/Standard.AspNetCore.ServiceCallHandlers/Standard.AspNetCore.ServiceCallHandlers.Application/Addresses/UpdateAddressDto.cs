using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Addresses
{
    public class UpdateAddressDto
    {
        public UpdateAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }

        public static UpdateAddressDto Create(string line1, string line2)
        {
            return new UpdateAddressDto
            {
                Line1 = line1,
                Line2 = line2
            };
        }
    }
}