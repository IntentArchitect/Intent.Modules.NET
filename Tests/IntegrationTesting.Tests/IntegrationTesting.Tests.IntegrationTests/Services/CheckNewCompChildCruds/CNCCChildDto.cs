using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class CNCCChildDto
    {
        public CNCCChildDto()
        {
            Description = null!;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }

        public static CNCCChildDto Create(Guid checkNewCompChildCrudId, Guid id, string description)
        {
            return new CNCCChildDto
            {
                CheckNewCompChildCrudId = checkNewCompChildCrudId,
                Id = id,
                Description = description
            };
        }
    }
}