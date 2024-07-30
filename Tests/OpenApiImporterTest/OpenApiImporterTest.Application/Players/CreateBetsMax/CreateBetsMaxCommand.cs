using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Players.CreateBetsMax
{
    [Authorize]
    public class CreateBetsMaxCommand : IRequest<MaxBetResultViewModel>, ICommand
    {
        public CreateBetsMaxCommand(string? id,
            PlayerType? player,
            decimal amount,
            PriceChangeType priceChange,
            List<BetItemViewModel> items,
            string playerId)
        {
            Id = id;
            Player = player;
            Amount = amount;
            PriceChange = priceChange;
            Items = items;
            PlayerId = playerId;
        }

        public string? Id { get; set; }
        public PlayerType? Player { get; set; }
        public decimal Amount { get; set; }
        public PriceChangeType PriceChange { get; set; }
        public List<BetItemViewModel> Items { get; set; }
        public string PlayerId { get; set; }
    }
}