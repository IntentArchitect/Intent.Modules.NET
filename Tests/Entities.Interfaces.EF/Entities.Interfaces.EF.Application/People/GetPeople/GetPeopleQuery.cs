using System.Collections.Generic;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People.GetPeople
{
    public class GetPeopleQuery : IRequest<List<PersonDto>>, IQuery
    {
        public GetPeopleQuery()
        {
        }
    }
}