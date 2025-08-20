using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class GetOneByIdQuery
    {
        public Guid Id { get; set; }

        public static GetOneByIdQuery Create(Guid id)
        {
            return new GetOneByIdQuery
            {
                Id = id
            };
        }
    }
}