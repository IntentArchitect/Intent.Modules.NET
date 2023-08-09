using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.Services.Integration
{
    public class CustomDTO
    {
        public static CustomDTO Create(string referenceNumber)
        {
            return new CustomDTO
            {
                ReferenceNumber = referenceNumber
            };
        }

        public string ReferenceNumber { get; set; }
    }
}