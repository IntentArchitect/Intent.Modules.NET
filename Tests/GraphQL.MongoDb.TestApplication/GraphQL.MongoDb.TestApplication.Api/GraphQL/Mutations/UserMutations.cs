using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Application.Users;
using GraphQL.MongoDb.TestApplication.Application.Users.CreateUser;
using GraphQL.MongoDb.TestApplication.Application.Users.DeleteUser;
using GraphQL.MongoDb.TestApplication.Application.Users.UpdateUser;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationType", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Api.GraphQL.Mutations
{
    [ExtendObjectType(OperationType.Mutation)]
    public class UserMutations
    {
        public async Task<string> CreateUser(
            CreateUserCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<UserDto> DeleteUser(
            DeleteUserCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<UserDto> UpdateUser(
            UpdateUserCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }
    }
}