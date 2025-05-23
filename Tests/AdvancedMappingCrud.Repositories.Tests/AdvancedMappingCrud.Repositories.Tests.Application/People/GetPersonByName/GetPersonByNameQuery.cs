using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People.GetPersonByName
{
    public class GetPersonByNameQuery : IRequest<GetPersonByNamePersonDCDto>, IQuery
    {
        public GetPersonByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}