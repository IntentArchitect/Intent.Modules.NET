using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Clients
{
    public class ClientCreate
    {
        public ClientCreate()
        {
        }

        public static ClientCreate Create()
        {
            return new ClientCreate
            {
            };
        }
    }
}