using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.SampleDomains
{
    public class SampleDomainUpdateDto
    {
        public SampleDomainUpdateDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static SampleDomainUpdateDto Create(Guid id, string attribute)
        {
            return new SampleDomainUpdateDto
            {
                Id = id,
                Attribute = attribute
            };
        }
    }
}