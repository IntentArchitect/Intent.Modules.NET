using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Users
{

    public class UpdateUserRoleDto
    {
        public UpdateUserRoleDto()
        {
        }

        public static UpdateUserRoleDto Create(
            string name,
            Guid id)
        {
            return new UpdateUserRoleDto
            {
                Name = name,
                Id = id,
            };
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

    }
}