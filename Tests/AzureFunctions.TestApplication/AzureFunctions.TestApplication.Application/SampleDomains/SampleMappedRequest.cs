using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
{
    public class SampleMappedRequest
    {
        public SampleMappedRequest()
        {
            Field = null!;
        }

        public string Field { get; set; }

        public static SampleMappedRequest Create(string field)
        {
            return new SampleMappedRequest
            {
                Field = field
            };
        }
    }
}