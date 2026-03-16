using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client.Contracts.Services.SelfReferenceValidation
{
    public class SelfRefDto
    {
        public SelfRefDto()
        {
            Entry = null!;
            SelfRefDtos = [];
        }

        public string Entry { get; set; }
        public List<SelfRefDto> SelfRefDtos { get; set; }

        public static SelfRefDto Create(string entry, List<SelfRefDto> selfRefDtos)
        {
            return new SelfRefDto
            {
                Entry = entry,
                SelfRefDtos = selfRefDtos
            };
        }
    }
}