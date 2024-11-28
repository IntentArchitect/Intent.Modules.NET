using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreMvc.Application.ClientsService
{
    public class ClientUpdateDto
    {
        public ClientUpdateDto()
        {
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }

        public static ClientUpdateDto Create(Guid id, string? name)
        {
            return new ClientUpdateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}