using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Plurals
{
    public class PluralsUpdateDto
    {
        public PluralsUpdateDto()
        {
        }

        public Guid Id { get; set; }

        public static PluralsUpdateDto Create(Guid id)
        {
            return new PluralsUpdateDto
            {
                Id = id
            };
        }
    }
}