using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren
{
    public class GetParentWithAnemicChildByIdQuery
    {
        public Guid Id { get; set; }

        public static GetParentWithAnemicChildByIdQuery Create(Guid id)
        {
            return new GetParentWithAnemicChildByIdQuery
            {
                Id = id
            };
        }
    }
}