using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Optionals
{
    public class GetOptionalByIdQuery
    {
        public Guid Id { get; set; }

        public static GetOptionalByIdQuery Create(Guid id)
        {
            return new GetOptionalByIdQuery
            {
                Id = id
            };
        }
    }
}