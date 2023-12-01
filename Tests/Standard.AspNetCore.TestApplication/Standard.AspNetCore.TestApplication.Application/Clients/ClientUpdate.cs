using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Clients
{
    public class ClientUpdate
    {
        public ClientUpdate()
        {
        }

        public Guid Id { get; set; }

        public static ClientUpdate Create(Guid id)
        {
            return new ClientUpdate
            {
                Id = id
            };
        }
    }
}