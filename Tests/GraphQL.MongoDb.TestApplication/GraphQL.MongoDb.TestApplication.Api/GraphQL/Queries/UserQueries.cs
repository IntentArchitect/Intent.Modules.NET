using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Application.Users;
using GraphQL.MongoDb.TestApplication.Application.Users.GetUserById;
using GraphQL.MongoDb.TestApplication.Application.Users.GetUsers;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class UserQueries
    {
        public async Task<UserDto> GetUserById(string id, CancellationToken cancellationToken, [Service] ISender mediator)
        {
            return await mediator.Send(new GetUserByIdQuery { Id = id }, cancellationToken);
        }

        public async Task<IReadOnlyList<UserDto>> GetUsers(CancellationToken cancellationToken, [Service] ISender mediator)
        {
            return await mediator.Send(new GetUsersQuery(), cancellationToken);
        }
    }
}