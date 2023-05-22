using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
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