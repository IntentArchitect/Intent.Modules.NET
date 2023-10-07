using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserDto>, ICommand
    {
        public UpdateUserCommand(string id, string name, string surname, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
        }
        public string Id { get; private set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public void SetId(string id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }

    }
}