using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation.UploadSelfRefDto
{
    public class UploadSelfRefDtoCommand : IRequest, ICommand
    {
        public UploadSelfRefDtoCommand(string entry, List<SelfRefDto> selfRefDtos)
        {
            Entry = entry;
            SelfRefDtos = selfRefDtos;
        }

        public string Entry { get; set; }
        public List<SelfRefDto> SelfRefDtos { get; set; }
    }
}