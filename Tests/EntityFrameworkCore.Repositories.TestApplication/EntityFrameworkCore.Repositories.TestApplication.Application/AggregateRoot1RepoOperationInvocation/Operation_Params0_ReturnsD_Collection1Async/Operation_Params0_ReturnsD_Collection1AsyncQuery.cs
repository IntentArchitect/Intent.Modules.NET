using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AggregateRoot1RepoOperationInvocation.Operation_Params0_ReturnsD_Collection1Async
{
    public class Operation_Params0_ReturnsD_Collection1AsyncQuery : IRequest<List<SpResultDto>>, IQuery
    {
        public Operation_Params0_ReturnsD_Collection1AsyncQuery()
        {
        }
    }
}