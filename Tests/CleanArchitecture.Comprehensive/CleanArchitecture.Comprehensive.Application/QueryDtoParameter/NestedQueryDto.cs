using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter
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