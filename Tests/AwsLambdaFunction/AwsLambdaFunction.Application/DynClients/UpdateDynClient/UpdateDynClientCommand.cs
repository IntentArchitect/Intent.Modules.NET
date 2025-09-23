using AwsLambdaFunction.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients.UpdateDynClient
{
    public class UpdateDynClientCommand : IRequest, ICommand
    {
        public UpdateDynClientCommand(string id, string name, string affiliateId)
        {
            Id = id;
            Name = name;
            AffiliateId = affiliateId;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string AffiliateId { get; set; }
    }
}