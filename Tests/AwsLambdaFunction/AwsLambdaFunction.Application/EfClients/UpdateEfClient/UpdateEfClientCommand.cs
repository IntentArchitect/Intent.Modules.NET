using System;
using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients.UpdateEfClient
{
    public class UpdateEfClientCommand : IRequest, ICommand
    {
        public UpdateEfClientCommand(Guid id, string name, Guid affiliateId)
        {
            Id = id;
            Name = name;
            AffiliateId = affiliateId;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AffiliateId { get; set; }
    }
}