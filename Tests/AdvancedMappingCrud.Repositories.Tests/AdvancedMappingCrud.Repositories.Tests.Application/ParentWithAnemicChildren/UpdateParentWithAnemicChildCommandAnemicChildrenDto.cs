using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren
{
    public class UpdateParentWithAnemicChildCommandAnemicChildrenDto
    {
        public UpdateParentWithAnemicChildCommandAnemicChildrenDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }

        public static UpdateParentWithAnemicChildCommandAnemicChildrenDto Create(
            Guid id,
            string line1,
            string line2,
            string city)
        {
            return new UpdateParentWithAnemicChildCommandAnemicChildrenDto
            {
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city
            };
        }
    }
}