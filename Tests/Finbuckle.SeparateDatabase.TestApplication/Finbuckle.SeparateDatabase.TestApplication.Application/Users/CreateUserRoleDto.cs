using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{

    public class CreateUserRoleDto
    {
        public CreateUserRoleDto()
        {
            Name = null!;
        }

        public static CreateUserRoleDto Create(string name)
        {
            return new CreateUserRoleDto
            {
                Name = name
            };
        }

        public string Name { get; set; }

    }
}