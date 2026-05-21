using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards.DeleteGiftCard
{
    public class DeleteGiftCardCommand : IRequest, ICommand
    {
        public DeleteGiftCardCommand(string cardCode)
        {
            CardCode = cardCode;
        }

        public string CardCode { get; set; }
    }
}