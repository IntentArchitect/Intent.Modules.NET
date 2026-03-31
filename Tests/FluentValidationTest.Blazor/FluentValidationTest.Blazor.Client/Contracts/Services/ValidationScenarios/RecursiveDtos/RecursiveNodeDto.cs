using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client.Contracts.Services.ValidationScenarios.RecursiveDtos
{
    public class RecursiveNodeDto
    {
        public RecursiveNodeDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public int Level { get; set; }
        public string? OptionalCode { get; set; }
        public RecursiveNodeDto? Child { get; set; }

        public static RecursiveNodeDto Create(string name, int level, string? optionalCode, RecursiveNodeDto? child)
        {
            return new RecursiveNodeDto
            {
                Name = name,
                Level = level,
                OptionalCode = optionalCode,
                Child = child
            };
        }
    }
}