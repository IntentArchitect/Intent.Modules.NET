using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace RichDomain.Application.People.GetPersonById
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