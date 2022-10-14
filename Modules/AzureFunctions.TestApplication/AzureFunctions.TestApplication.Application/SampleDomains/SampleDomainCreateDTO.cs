using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
{

    public class SampleDomainCreateDTO
    {
        public SampleDomainCreateDTO()
        {
        }

        public static SampleDomainCreateDTO Create(
            )
        {
            return new SampleDomainCreateDTO
            {
            };
        }

    }
}