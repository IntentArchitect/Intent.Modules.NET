using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class OneFiveDto
    {
        public OneFiveDto()
        {
            FiveName5 = null!;
        }

        public string FiveName5 { get; set; }
        public Guid Id { get; set; }

        public static OneFiveDto Create(string fiveName5, Guid id)
        {
            return new OneFiveDto
            {
                FiveName5 = fiveName5,
                Id = id
            };
        }
    }
}