using Intent.RoslynWeaver.Attributes;
using MediatR;
using Subscribe.MassTransit.DomainInteractionsRepro.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.CreateCatalogue
{
    public class CreateCatalogueCommand : IRequest, ICommand
    {
        public CreateCatalogueCommand(string name, string code, List<CreateCatalogueItemDto> catalogueItems)
        {
            Name = name;
            Code = code;
            CatalogueItems = catalogueItems;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public List<CreateCatalogueItemDto> CatalogueItems { get; set; }
    }
}