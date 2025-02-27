using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers
{
    public class AddPlotDto
    {
        public AddPlotDto()
        {
            Line1 = null!;
            Line2 = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }

        public static AddPlotDto Create(string line1, string line2)
        {
            return new AddPlotDto
            {
                Line1 = line1,
                Line2 = line2
            };
        }
    }
}