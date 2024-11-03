using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.NET8.Application.SampleDomains
{
    public class SampleDomainCreateDto
    {
        public SampleDomainCreateDto()
        {
            Attribute = null!;
            Name = null!;
        }

        public string Attribute { get; set; }
        public string Name { get; set; }

        public static SampleDomainCreateDto Create(string attribute, string name)
        {
            return new SampleDomainCreateDto
            {
                Attribute = attribute,
                Name = name
            };
        }
    }
}