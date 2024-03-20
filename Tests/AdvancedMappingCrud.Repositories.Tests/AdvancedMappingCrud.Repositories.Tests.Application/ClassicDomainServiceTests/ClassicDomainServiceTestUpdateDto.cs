using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ClassicDomainServiceTests
{
    public class ClassicDomainServiceTestUpdateDto
    {
        public ClassicDomainServiceTestUpdateDto()
        {
        }

        public Guid Id { get; set; }

        public static ClassicDomainServiceTestUpdateDto Create(Guid id)
        {
            return new ClassicDomainServiceTestUpdateDto
            {
                Id = id
            };
        }
    }
}