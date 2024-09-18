using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Regions.CreateRegion
{
    public class CreateRegionCommand : IRequest<string>, ICommand
    {
        public CreateRegionCommand(string name, List<CreateRegionCountryDto> countries)
        {
            Name = name;
            Countries = countries;
        }

        public string Name { get; set; }
        public List<CreateRegionCountryDto> Countries { get; set; }
    }
}