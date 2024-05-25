using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People.GetPeople
{
    public class GetPeopleQuery : IRequest<List<PersonDto>>, IQuery
    {
        public GetPeopleQuery()
        {
        }
    }
}