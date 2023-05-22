using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivileges
{
    public class GetPrivilegesQuery : IRequest<List<PrivilegeDto>>, IQuery
    {
        public GetPrivilegesQuery()
        {
        }
    }
}