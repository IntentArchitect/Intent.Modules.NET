using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AzureFunctions.NET8.Application.IntegrationServices.Contracts.Services.SampleDomains
{
    public class SampleDomainCreateDto
    {
        public SampleDomainCreateDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }

        public static SampleDomainCreateDto Create(string attribute)
        {
            return new SampleDomainCreateDto
            {
                Attribute = attribute
            };
        }
    }
}