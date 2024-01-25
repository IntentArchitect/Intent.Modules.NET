using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.People
{
    public class PersonCreateDto
    {
        public PersonCreateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static PersonCreateDto Create(string name)
        {
            return new PersonCreateDto
            {
                Name = name
            };
        }
    }
}