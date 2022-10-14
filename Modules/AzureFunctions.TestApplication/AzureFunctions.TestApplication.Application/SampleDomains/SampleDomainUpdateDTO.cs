using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
{

    public class SampleDomainUpdateDTO
    {
        public SampleDomainUpdateDTO()
        {
        }

        public static SampleDomainUpdateDTO Create(
            )
        {
            return new SampleDomainUpdateDTO
            {
            };
        }

    }
}