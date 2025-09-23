using System.Collections.Generic;
using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients.GetDynClients
{
    public class GetDynClientsQuery : IRequest<List<DynClientDto>>, IQuery
    {
        public GetDynClientsQuery()
        {
        }
    }
}