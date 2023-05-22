using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDto>, IQuery
    {
        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
        public string Id { get; set; }

    }
}