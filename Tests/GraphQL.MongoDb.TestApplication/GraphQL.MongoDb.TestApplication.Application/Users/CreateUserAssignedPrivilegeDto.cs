using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users
{
    public class CreateUserAssignedPrivilegeDto
    {
        public CreateUserAssignedPrivilegeDto()
        {
            PrivilegeId = null!;
        }

        public string PrivilegeId { get; set; }

        public static CreateUserAssignedPrivilegeDto Create(string privilegeId)
        {
            return new CreateUserAssignedPrivilegeDto
            {
                PrivilegeId = privilegeId
            };
        }
    }
}