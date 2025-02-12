using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren
{
    public class CreateParentWithAnemicChildAnemicChildDto
    {
        public CreateParentWithAnemicChildAnemicChildDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }

        public static CreateParentWithAnemicChildAnemicChildDto Create(string line1, string line2, string city)
        {
            return new CreateParentWithAnemicChildAnemicChildDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city
            };
        }
    }
}