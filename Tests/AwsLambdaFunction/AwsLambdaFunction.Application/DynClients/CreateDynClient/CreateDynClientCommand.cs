using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients.CreateDynClient
{
    public class CreateDynClientCommand : IRequest<string>, ICommand
    {
        public CreateDynClientCommand(string name, string affiliateId)
        {
            Name = name;
            AffiliateId = affiliateId;
        }

        public string Name { get; set; }
        public string AffiliateId { get; set; }
    }
}