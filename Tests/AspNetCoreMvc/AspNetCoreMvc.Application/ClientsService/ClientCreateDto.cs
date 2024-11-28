using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreMvc.Application.ClientsService
{
    public class ClientCreateDto
    {
        public ClientCreateDto()
        {
        }

        public string? Name { get; set; }

        public static ClientCreateDto Create(string? name)
        {
            return new ClientCreateDto
            {
                Name = name
            };
        }
    }
}