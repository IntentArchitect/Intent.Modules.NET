using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Application.Privileges;
using GraphQL.MongoDb.TestApplication.Application.Privileges.CreatePrivilege;
using GraphQL.MongoDb.TestApplication.Application.Privileges.DeletePrivilege;
using GraphQL.MongoDb.TestApplication.Application.Privileges.UpdatePrivilege;
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
    public class PrivilegeMutations
    {
        public async Task<PrivilegeDto> CreatePrivilege(
            CreatePrivilegeCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<PrivilegeDto> DeletePrivilege(
            DeletePrivilegeCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<PrivilegeDto> UpdatePrivilege(
            UpdatePrivilegeCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }
    }
}