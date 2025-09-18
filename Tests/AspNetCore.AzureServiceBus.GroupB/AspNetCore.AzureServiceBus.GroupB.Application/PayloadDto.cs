using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Application
{
    public class PayloadDto
    {
        public PayloadDto()
        {
            Data = null!;
        }

        public string Data { get; set; }

        public static PayloadDto Create(string data)
        {
            return new PayloadDto
            {
                Data = data
            };
        }
    }
}