using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Application.Privileges;
using GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivilegeById;
using GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivileges;
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
    public class PrivilegeQueries
    {
        public async Task<PrivilegeDto> GetPrivilegeById(
            string id,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetPrivilegeByIdQuery(id: id), cancellationToken);
        }

        public async Task<IReadOnlyList<PrivilegeDto>> GetPrivileges(
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetPrivilegesQuery(), cancellationToken);
        }
    }
}