using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.QueryDtoParameter
{
    public class NestedQueryDto
    {
        public NestedQueryDto()
        {
            Numbers = null!;
        }

        public List<int> Numbers { get; set; }
        public string? NullableProp { get; set; }

        public static NestedQueryDto Create(List<int> numbers, string? nullableProp)
        {
            return new NestedQueryDto
            {
                Numbers = numbers,
                NullableProp = nullableProp
            };
        }
    }
}