using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Countries
{
    public class StateDto
    {
        public StateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }

        public static StateDto Create(Guid id, string name, Guid countryId)
        {
            return new StateDto
            {
                Id = id,
                Name = name,
                CountryId = countryId
            };
        }
    }
}