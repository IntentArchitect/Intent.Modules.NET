using System;
using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients.GetEfClientById
{
    public class GetEfClientByIdQuery : IRequest<EfClientDto>, IQuery
    {
        public GetEfClientByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}