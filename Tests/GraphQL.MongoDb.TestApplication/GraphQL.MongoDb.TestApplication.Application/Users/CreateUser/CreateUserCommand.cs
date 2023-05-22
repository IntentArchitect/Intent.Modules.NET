using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<string>, ICommand
    {
        public CreateUserCommand(string name,
            string surname,
            string email,
            List<CreateUserAssignedPrivilegeDto> assignedPrivileges)
        {
            Name = name;
            Surname = surname;
            Email = email;
            AssignedPrivileges = assignedPrivileges;
        }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public List<CreateUserAssignedPrivilegeDto> AssignedPrivileges { get; set; }

    }
}