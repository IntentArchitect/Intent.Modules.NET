using System;
using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients.CreateEfClient
{
    public class CreateEfClientCommand : IRequest<Guid>, ICommand
    {
        public CreateEfClientCommand(string name, Guid affiliateId)
        {
            Name = name;
            AffiliateId = affiliateId;
        }

        public string Name { get; set; }
        public Guid AffiliateId { get; set; }
    }
}