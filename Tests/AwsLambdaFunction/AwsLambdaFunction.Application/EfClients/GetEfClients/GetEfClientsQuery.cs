using System.Collections.Generic;
using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients.GetEfClients
{
    public class GetEfClientsQuery : IRequest<List<EfClientDto>>, IQuery
    {
        public GetEfClientsQuery()
        {
        }
    }
}