using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{

    public class UpdateUserRoleDto
    {
        public UpdateUserRoleDto()
        {
        }

        public static UpdateUserRoleDto Create(Guid userId, string name, Guid id)
        {
            return new UpdateUserRoleDto
            {
                UserId = userId,
                Name = name,
                Id = id,
            };
        }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }

    }
}