using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren
{
    public class GetParentWithAnemicChildrenQuery
    {
        public static GetParentWithAnemicChildrenQuery Create()
        {
            return new GetParentWithAnemicChildrenQuery
            {
            };
        }
    }
}