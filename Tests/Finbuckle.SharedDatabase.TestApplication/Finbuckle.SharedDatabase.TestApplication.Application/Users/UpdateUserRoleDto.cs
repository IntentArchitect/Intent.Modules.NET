using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Users
{

    public class UpdateUserRoleDto
    {
        public UpdateUserRoleDto()
        {
        }

        public static UpdateUserRoleDto Create(string name, Guid userId, Guid id)
        {
            return new UpdateUserRoleDto
            {
                Name = name,
                UserId = userId,
                Id = id
            };
        }

        public string Name { get; set; }

        public Guid UserId { get; set; }

        public Guid Id { get; set; }

    }
}