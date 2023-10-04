using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace RichDomain.Application.People.GetPeople
{
    public class GetPeopleQuery : IRequest<List<PersonDto>>, IQuery
    {
        public GetPeopleQuery()
        {
        }
    }
}