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