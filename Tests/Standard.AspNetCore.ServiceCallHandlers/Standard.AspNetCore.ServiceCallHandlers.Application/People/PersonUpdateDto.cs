using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.People
{
    public class PersonUpdateDto
    {
        public PersonUpdateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static PersonUpdateDto Create(Guid id, string name)
        {
            return new PersonUpdateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}