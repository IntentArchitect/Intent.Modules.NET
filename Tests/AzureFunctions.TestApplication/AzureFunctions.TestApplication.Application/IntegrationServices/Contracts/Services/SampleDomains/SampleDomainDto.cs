using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.SampleDomains
{
    public class SampleDomainDto
    {
        public SampleDomainDto()
        {
            Attribute = null!;
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }
        public string Name { get; set; }

        public static SampleDomainDto Create(Guid id, string attribute, string name)
        {
            return new SampleDomainDto
            {
                Id = id,
                Attribute = attribute,
                Name = name
            };
        }
    }
}