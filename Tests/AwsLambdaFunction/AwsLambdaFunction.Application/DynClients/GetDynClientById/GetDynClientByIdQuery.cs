using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients.GetDynClientById
{
    public class GetDynClientByIdQuery : IRequest<DynClientDto>, IQuery
    {
        public GetDynClientByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}