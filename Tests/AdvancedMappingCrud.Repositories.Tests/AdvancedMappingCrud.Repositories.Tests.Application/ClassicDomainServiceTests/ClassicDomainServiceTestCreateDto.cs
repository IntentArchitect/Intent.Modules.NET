using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ClassicDomainServiceTests
{
    public class ClassicDomainServiceTestCreateDto
    {
        public ClassicDomainServiceTestCreateDto()
        {
        }

        public static ClassicDomainServiceTestCreateDto Create()
        {
            return new ClassicDomainServiceTestCreateDto
            {
            };
        }
    }
}