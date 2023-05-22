using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.DeleteUser
{
    public class DeleteUserCommand : IRequest<UserDto>, ICommand
    {
        public DeleteUserCommand(string id)
        {
            Id = id;
        }
        public string Id { get; set; }

    }
}