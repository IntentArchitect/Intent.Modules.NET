using System;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People.GetPersonById
{
    public class GetPersonByIdQuery : IRequest<PersonDto>, IQuery
    {
        public GetPersonByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}