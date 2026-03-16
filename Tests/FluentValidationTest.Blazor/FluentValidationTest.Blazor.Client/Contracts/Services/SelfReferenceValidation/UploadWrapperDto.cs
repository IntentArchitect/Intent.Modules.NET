using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client.Contracts.Services.SelfReferenceValidation
{
    public class UploadWrapperDto
    {
        public UploadWrapperDto()
        {
            Entry = null!;
            SelfRefDtos = [];
        }

        public string Entry { get; set; }
        public List<SelfRefDto> SelfRefDtos { get; set; }

        public static UploadWrapperDto Create(string entry, List<SelfRefDto> selfRefDtos)
        {
            return new UploadWrapperDto
            {
                Entry = entry,
                SelfRefDtos = selfRefDtos
            };
        }
    }
}