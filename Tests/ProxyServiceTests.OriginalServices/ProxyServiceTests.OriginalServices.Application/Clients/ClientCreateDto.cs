using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Clients
{
    public class ClientCreateDto
    {
        public ClientCreateDto()
        {
        }

        public string Name { get; set; } = "n/a";

        public static ClientCreateDto Create(string name = "n/a")
        {
            return new ClientCreateDto
            {
                Name = name
            };
        }
    }
}