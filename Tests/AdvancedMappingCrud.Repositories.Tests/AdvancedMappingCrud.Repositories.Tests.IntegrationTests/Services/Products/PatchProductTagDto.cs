using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products
{
    public class PatchProductTagDto
    {
        public string? Name { get; set; }
        public string? Value { get; set; }

        public static PatchProductTagDto Create(string? name, string? value)
        {
            return new PatchProductTagDto
            {
                Name = name,
                Value = value
            };
        }
    }
}